#nullable enable
using System;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
#pragma warning disable RS0016
	public class ImageSourceValueConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			Debug.Assert(targetType == typeof(ImageSource));

			return value switch
			{
				ImageSource imageSource => imageSource,
				Uri uri => (ImageSource)uri,
				string str => (ImageSource)str,
				_ => null,
			};
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
#pragma warning restore RS0016
}
