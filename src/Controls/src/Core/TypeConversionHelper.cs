#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls
{
	internal static class TypeConversionHelper
	{
		internal static bool TryConvert(ref object value, Type targetType)
		{
			Type valueType = value.GetType();

			if (TryGetTypeConverter(valueType, out var converter) && converter is not null && converter.CanConvertTo(targetType))
			{
				value = converter.ConvertTo(value, targetType) ?? throw new InvalidOperationException($"The {converter.GetType()} returned null when converting {valueType} to {targetType}");
				return true;
			}

			if (TryGetTypeConverter(targetType, out converter) && converter is not null && converter.CanConvertFrom(valueType))
			{
				value = converter.ConvertFrom(value) ?? throw new InvalidOperationException($"The {converter.GetType()} returned null when converting from {valueType}");
				return true;
			}

			if (RuntimeFeature.IsImplicitCastOperatorsUsageViaReflectionSupported)
			{
				if (TryConvertUsingImplicitCastOperator(value, targetType, out var convertedValue))
				{
					value = convertedValue;
					return true;
				}
			}
			else
			{
				WarnIfImplicitOperatorIsAvailable(value, targetType);
			}

			return false;
		}

		private static bool TryGetTypeConverter(Type type, [NotNullWhen(true)] out TypeConverter? converter)
			=> TypeConversionAppBuilderExtensions.TypeConverters.TryGetValue(type, out converter)
				|| type.TryGetTypeConverter(out converter);

		[RequiresUnreferencedCode("The method uses reflection to find implicit conversion operators. " +
			"It is not possible to guarantee that trimming does not remove some of the implicit operators. " +
			"Consider preserving op_Implicit methods through DynamicDependency or DynamicallyAccessedMembers attributes.",
			Url = "https://learn.microsoft.com/dotnet/core/deploying/trimming/prepare-libraries-for-trimming")]
		private static bool TryConvertUsingImplicitCastOperator(object value, Type targetType, [NotNullWhen(true)] out object? result)
		{
			Type valueType = value.GetType();
			MethodInfo? opImplicit = GetImplicitConversionOperator(valueType, fromType: valueType, toType: targetType)
										?? GetImplicitConversionOperator(targetType, fromType: valueType, toType: targetType);

			object? convertedValue = opImplicit?.Invoke(null, new[] { value });
			if (convertedValue is not null)
			{
				result = convertedValue;
				return true;
			}

			result = null;
			return false;

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:RequiresUnreferencedCode",
				Justification = "We add our own RequiresUnreferencedCode attribute to the parent method.")]
			static MethodInfo? GetImplicitConversionOperator(Type onType, Type fromType, Type toType)
			{
				var bindingAttr = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
				IEnumerable<MethodInfo?>? mis = null;
				try
				{
					mis = new[] { onType.GetMethod("op_Implicit", bindingAttr, null, new[] { fromType }, null) };
				}
				catch (AmbiguousMatchException)
				{
					mis = new List<MethodInfo>();
					foreach (var mi in onType.GetMethods(bindingAttr))
					{
						if (mi.Name != "op_Implicit")
							break; // TODO this is a bug (should be continue), but should we fix it and potentially break existing code?
						var parameters = mi.GetParameters();
						if (parameters.Length == 0)
							continue;
						if (!parameters[0].ParameterType.IsAssignableFrom(fromType))
							continue;
						((List<MethodInfo>)mis).Add(mi);
					}
				}

				foreach (var mi in mis)
				{
					if (mi == null)
						continue;
					if (!mi.IsSpecialName)
						continue;
					if (!mi.IsPublic)
						continue;
					if (!mi.IsStatic)
						continue;
					if (!toType.IsAssignableFrom(mi.ReturnType))
						continue;

					return mi;
				}
				return null;
			}
		}

		private static void WarnIfImplicitOperatorIsAvailable(object value, Type targetType)
		{
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
				Justification = "The method tries finding implicit cast operators reflection to help developers " +
					"catch the cases where they are missing type converters during development mostly in debug builds. " +
					"The method is expected not to find anything when the app code is trimmed.")]
			bool HasImplicitOperatorConversion()
			{
				return TryConvertUsingImplicitCastOperator(value, targetType, out _);
			}

			if (HasImplicitOperatorConversion())
			{
				// If we reach this point, it means that the implicit cast operator exists, but we are not allowed to use it. This can happen for example in debug builds
				// when the app is not trimmed. This is the best effort to help developers catch the cases where they are missing type converters during development.
				// Unforutnately, we cannot just add a build warning at this moment.
				var valueType = value.GetType();
				Application.Current?.FindMauiContext()?.CreateLogger(nameof(TypeConversionHelper))?.LogWarning(
					"It is not possible to convert value of type {valueType} to {targetType} via an implicit cast " +
					"because this feature is disabled. You should add a type converter that will implement this conversion and attach it to either of " +
					"these types using the [TypeConverter] attribute or using the ConfigureTypeConversions method on MauiAppBuilder. Alternatively, you " +
					"can enable this feature by setting the MauiImplicitCastOperatorsUsageViaReflectionSupport MSBuild property to true in your project file. " +
					"Note: this feature is not compatible with trimming and with NativeAOT.", valueType, targetType);
			}
		}
	}
}
