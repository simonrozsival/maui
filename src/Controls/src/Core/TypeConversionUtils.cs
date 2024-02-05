#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace Microsoft.Maui.Controls
{
    internal static class TypeConversionUtils
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

        public static bool TryConvert(ref object? value, Type targetType)
        {

			if (value is null)
            {
				return !targetType.IsValueType || targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

			Type valueType = value.GetType();
			if (targetType == valueType)
            {
				return true;
            }

			// Dont support arbitrary IConvertible by limiting which types can use this
			if (SimpleConvertTypes.TryGetValue(valueType, out Type[]? convertibleTo) && Array.IndexOf(convertibleTo, targetType) != -1)
			{
                Console.WriteLine($"TypeConversionUtils.TryConvert - simple convert type: targetType={targetType}, value={value}");
				value = Convert.ChangeType(value, targetType);
				return true;
			}
			else if (KnownTypeConverters.TryGetValue(targetType, out TypeConverter? typeConverterTo) && typeConverterTo.CanConvertFrom(valueType))
			{
                Console.WriteLine($"TypeConversionUtils.TryConvert - known type converter: targetType={targetType}, value={value}");
				value = typeConverterTo.ConvertFromInvariantString(value!.ToString()!);
				return true;
			}
			else if (targetType.IsAssignableFrom(valueType))
            {
				return true;
            }

            // developer has the option to add custom conversions
            Console.WriteLine($"TypeConversionUtils.TryConvert - use type conversion? ({value} ({value.GetType()}) -> {targetType})");
            var conversionService = Application.Current?.FindMauiContext()?.Services.GetService<TypeConversionService>();
            if (conversionService is not null && conversionService.TryConvert(ref value, targetType))
            {
                return true;
            }

			if (KnownIValueConverters.TryGetValue(targetType, out IValueConverter? valueConverter))
			{
				value = valueConverter.Convert(value, targetType, null, CultureInfo.CurrentUICulture);
				return true;
			}

            return false;
        }
    }
}