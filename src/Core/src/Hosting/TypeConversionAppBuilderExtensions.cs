#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Maui.Hosting
{
	// TODO make this public
	internal static class TypeConversionAppBuilderExtensions
	{
		internal static Dictionary<Type, TypeConverter> TypeConverters = new();

		public static MauiAppBuilder ConfigureTypeConversions(this MauiAppBuilder builder, Action<ITypeConversionBuilder> configureDelegate)
		{
			var typeConversionBuilder = new TypeConversionBuilder();

			configureDelegate(typeConversionBuilder);

			foreach (var kvp in typeConversionBuilder.Converters)
			{
				TypeConverters[kvp.Key] = kvp.Value;
			}

			return builder;
		}

		private class TypeConversionBuilder : ITypeConversionBuilder
		{
			internal Dictionary<Type, TypeConverter> Converters { get; } = new();

			public ITypeConversionBuilder AddTypeConverter<T, TConverter>()
				where TConverter : TypeConverter, new()
			{
				Converters[typeof(T)] = new TConverter();
				return this;
			}

			public ITypeConversionBuilder AddTypeConverter<T>(Func<TypeConverter> createConverter)
			{
				Converters[typeof(T)] = createConverter();
				return this;
			}
		}
	}

	// TODO make this public
	internal interface ITypeConversionBuilder
	{
		ITypeConversionBuilder AddTypeConverter<T, TConverter>() where TConverter : TypeConverter, new();
		ITypeConversionBuilder AddTypeConverter<T>(Func<TypeConverter> createConverter);
	}
}
