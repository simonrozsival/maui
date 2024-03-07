using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Maui.Controls.Xaml
{
	[ContentProperty(nameof(Items))]
	[AcceptEmptyServiceProvider]
	// TODO: We need to compile array extension otherwise apps with x:Array will see AOT warnings coming from MAUI
	// - https://github.com/dotnet/maui/issues/20745
#if !NETSTANDARD
	[RequiresDynamicCode(TrimmerConstants.XamlRuntimeParsingNotSupportedWarning)]
#endif
	public class ArrayExtension : IMarkupExtension<Array>
	{
		public ArrayExtension()
		{
			Items = new List<object>();
		}

		public IList Items { get; }

		public Type Type { get; set; }

		public Array ProvideValue(IServiceProvider serviceProvider)
		{
			if (Type == null)
				throw new InvalidOperationException("Type argument mandatory for x:Array extension");

			if (Items == null)
				return null;

			var array = Array.CreateInstance(Type, Items.Count);
			for (var i = 0; i < Items.Count; i++)
				((IList)array)[i] = Items[i];

			return array;
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<Array>).ProvideValue(serviceProvider);
		}
	}
}