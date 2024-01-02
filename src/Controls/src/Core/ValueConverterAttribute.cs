#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Maui.Controls
{
#pragma warning disable RS0016
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class ValueConverterAttribute : Attribute
	{
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		public Type Type { get; set; }

		public ValueConverterAttribute(
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
		{
			Type = type;
		}

		public IValueConverter CreateValueConverter()
			=> Activator.CreateInstance(Type) as IValueConverter ?? throw new InvalidOperationException($"Could not create instance of IValueConverter {Type}");
	}
#pragma warning restore RS0016
}
