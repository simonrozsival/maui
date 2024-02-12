using System;
using System.Globalization;

namespace Microsoft.Maui.Controls
{
	internal interface IWrappedValue
	{
		object Value { get; }
		Type Type { get; }
	}
}
