using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace Microsoft.Maui.Controls.Internals
{
	internal static class ConversionHelpers
	{
		static readonly Dictionary<Type, TypeConverter> KnownTypeConverters = new Dictionary<Type, TypeConverter>
		{
			{ typeof(Uri), new UriTypeConverter() },
			{ typeof(Easing), new Maui.Converters.EasingTypeConverter() },
			{ typeof(Maui.Graphics.Color), new ColorTypeConverter() },
		};

		static readonly Dictionary<Type, IValueConverter> KnownIValueConverters = new Dictionary<Type, IValueConverter>
		{
			{ typeof(string), new ToStringValueConverter() },
		};

		// more or less the encoding of this, without the need to reflect
		// http://msdn.microsoft.com/en-us/library/y5b434w4.aspx
		static readonly Dictionary<Type, Type[]> SimpleConvertTypes = new Dictionary<Type, Type[]>
		{
			{ typeof(sbyte), new[] { typeof(string), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(byte), new[] { typeof(string), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(short), new[] { typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(ushort), new[] { typeof(string), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(int), new[] { typeof(string), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(uint), new[] { typeof(string), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(long), new[] { typeof(string), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(char), new[] { typeof(string), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
			{ typeof(float), new[] { typeof(string), typeof(double) } },
			{ typeof(ulong), new[] { typeof(string), typeof(float), typeof(double), typeof(decimal) } },
		};

		static readonly Type[] DecimalTypes = { typeof(float), typeof(decimal), typeof(double) };

		internal static bool TryConvert(ref object value, BindableProperty targetProperty, Type convertTo, bool toTarget)
		{
			if (value == null)
				return !convertTo.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(convertTo) != null;
			try
			{
				if ((toTarget && targetProperty.TryConvert(ref value)) || (!toTarget && convertTo.IsInstanceOfType(value)))
					return true;
			}
			catch (InvalidOperationException)
			{ //that's what TypeConverters ususally throw
				return false;
			}

			object original = value;
			try
			{
				convertTo = Nullable.GetUnderlyingType(convertTo) ?? convertTo;

				var stringValue = value as string ?? string.Empty;
				// see: https://bugzilla.xamarin.com/show_bug.cgi?id=32871
				// do not canonicalize "*.[.]"; "1." should not update bound BindableProperty
				if (stringValue.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal) && DecimalTypes.Contains(convertTo))
				{
					value = original;
					return false;
				}

				// do not canonicalize "-0"; user will likely enter a period after "-0"
				if (stringValue == "-0" && DecimalTypes.Contains(convertTo))
				{
					value = original;
					return false;
				}

				value = Convert.ChangeType(value, convertTo, CultureInfo.CurrentCulture);

				return true;
			}
			catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is InvalidOperationException || ex is OverflowException)
			{
				value = original;
				return false;
			}
		}

		internal static bool TryConvert(ref object value, Type returnType)
		{
			if (value == null)
				return !returnType.IsValueType || returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>);

			Type valueType = value.GetType();

			// already the same type, no need to convert
			if (returnType == valueType)
				return true;

			// Dont support arbitrary IConvertible by limiting which types can use this
			if (SimpleConvertTypes.TryGetValue(valueType!, out Type[]? convertibleTo) && convertibleTo!.Contains(returnType))
			{
				value = Convert.ChangeType(value!, returnType)!;
				return true;
			}
			if (KnownTypeConverters.TryGetValue(returnType, out TypeConverter? typeConverterTo) && typeConverterTo!.CanConvertFrom(valueType))
			{
				value = typeConverterTo.ConvertFromInvariantString(value.ToString()!)!;
				return true;
			}
			if (returnType.IsAssignableFrom(valueType))
				return true;

			var cast = returnType.GetImplicitConversionOperator(fromType: valueType, toType: returnType) ?? valueType.GetImplicitConversionOperator(fromType: valueType, toType: returnType);
			if (cast != null)
			{
				value = cast.Invoke(null, new[] { value! })!;
				return true;
			}
			if (KnownIValueConverters.TryGetValue(returnType, out IValueConverter? valueConverter))
			{
				value = valueConverter.Convert(value, returnType, null, CultureInfo.CurrentUICulture)!;
				return true;
			}

			return false;
		}

		internal static bool TryConvert<TValue, TTarget>(TValue input, out TTarget? output)
		{
			if (input is null)
			{
				output = default;
				return !typeof(TTarget).IsValueType || (typeof(TTarget).IsGenericType && typeof(TTarget).GetGenericTypeDefinition() == typeof(Nullable<>));
			}

			if (input is TTarget target)
			{
				output = target;
				return true;
			}

			// TODO is the compiler smart enough to trim this method if TValue == TTarget?

			if (SimpleConvertTypes.TryGetValue(typeof(TValue), out Type[]? convertibleTo) && convertibleTo is not null && Array.IndexOf(convertibleTo, typeof(TTarget)) != -1)
			{
				output = (TTarget)Convert.ChangeType(input, typeof(TTarget));
				return true;
			}

			if (KnownTypeConverters.TryGetValue(typeof(TTarget), out TypeConverter? typeConverterTo) && typeConverterTo is not null && typeConverterTo.CanConvertFrom(typeof(TValue)))
			{
				output = typeConverterTo.ConvertFromInvariantString(input.ToString()!) is TTarget converted ? converted : default;
				return true;
			}

			if (typeof(TTarget).IsAssignableFrom(typeof(TValue)))
			{
				output = (TTarget)(object)input;
				return true;
			}

			// if we still couldn't convert it, let's just try casting those two values
			// this could invoke implicit or explicit cast operators
			try
			{
				output = (TTarget)(object)input;
				return true;
			}
			catch (InvalidCastException)
			{
				// we gave it a try
			}

			if (KnownIValueConverters.TryGetValue(typeof(TTarget), out IValueConverter? valueConverter) && valueConverter is not null)
			{
				output = valueConverter.Convert(input!, typeof(TTarget), null, CultureInfo.CurrentUICulture) is TTarget converted ? converted : default;
				return true;
			}

			try
			{
				Type convertTo = Nullable.GetUnderlyingType(typeof(TTarget)) ?? typeof(TTarget);

				var stringValue = input as string ?? string.Empty;
				// see: https://bugzilla.xamarin.com/show_bug.cgi?id=32871
				// do not canonicalize "*.[.]"; "1." should not update bound BindableProperty
				if (stringValue.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal) && DecimalTypes.IndexOf(convertTo) != -1)
				{
					output = default;
					return false;
				}

				// do not canonicalize "-0"; user will likely enter a period after "-0"
				if (stringValue == "-0" && DecimalTypes.IndexOf(convertTo) != -1)
				{
					output = default;
					return false;
				}

				output = (TTarget)Convert.ChangeType(input, convertTo, CultureInfo.CurrentCulture);
				return true;
			}
			catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is InvalidOperationException || ex is OverflowException)
			{
				// we gave it a try
			}

			output = default;
			return false;
		}
	}
}