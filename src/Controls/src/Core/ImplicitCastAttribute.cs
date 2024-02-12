#nullable enable

using System;

namespace Microsoft.Maui.Controls
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	internal abstract class ImplicitCastsAttribute : Attribute
	{
		public virtual bool TryCastTo(ref object value, Type targetType) => false;
		public virtual bool TryCastFrom(ref object value) => false;
	}

	[Accelerator.ImplicitCasts]
	partial class Accelerator
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is string str)
				{
#pragma warning disable CS0618
					value = (Accelerator)str;
#pragma warning restore CS0618
					return true;
				}
				
				return false;
			}
		}
	}

	[DoubleCollection.ImplicitCasts]
	partial class DoubleCollection
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is double[] doubles)
				{
					value = (DoubleCollection)doubles;
					return true;
				}
				else if (value is float[] floats)
				{
					value = (DoubleCollection)floats;
					return true;
				}

				return false;
			}
		}
	}

	[FileImageSource.ImplicitCasts]
	partial class FileImageSource
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastTo(ref object value, Type targetType)
			{
				if (value is not FileImageSource fileImageSource)
				{
					return false;
				}

				if (targetType == typeof(string))
				{
					value = (string)fileImageSource;
					return true;
				}

				return false;
			}

			public override bool TryCastFrom(ref object value)
			{
				if (value is string file)
				{
					value = (FileImageSource)file;
					return true;
				}

				return false;
			}
		}
	}

	[FormattedString.ImplicitCasts]
	partial class FormattedString
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is string str)
				{
					value = (FormattedString)str;
					return true;
				}

				return false;
			}
		}
	}

	[ImageSource.ImplicitCasts]
	partial class ImageSource
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is string source)
				{
					value = (ImageSource)source;
					return true;
				}
				else if (value is Uri uri)
				{
					value = (ImageSource)uri;
					return true;
				}

				return false;
			}
		}
	}

	[PointCollection.ImplicitCasts]
	partial class PointCollection
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is global::Microsoft.Maui.Graphics.Point[] points)
				{
					value = (PointCollection)points;
					return true;
				}

				return false;
			}
		}
	}

	[TemplatedPage.ImplicitCasts]
	partial class TemplatedPage
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastTo(ref object value, Type targetType)
			{
				if (value is not TemplatedPage templatedPage)
				{
					return false;
				}

				if (targetType == typeof(ShellContent))
				{
					value = (ShellContent)templatedPage;
					return true;
				}
				if (targetType == typeof(ShellItem))
				{
					value = (ShellItem)templatedPage;
					return true;
				}
				if (targetType == typeof(ShellSection))
				{
					value = (ShellSection)templatedPage;
					return true;
				}

				return false;
			}
		}
	}

	[Brush.ImplicitCasts]
	partial class Brush
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastTo(ref object value, Type targetType)
			{
				if (value is not Brush brush)
				{
					return false;
				}

				if (targetType == typeof(Brush))
				{
					return true;
				}
				if (targetType == typeof(global::Microsoft.Maui.Graphics.Paint))
				{
					value = (global::Microsoft.Maui.Graphics.Paint)brush;
					return true;
				}

				return false;
			}

			public override bool TryCastFrom(ref object value)
			{
				if (value is Brush brush)
				{
					return true;
				}
				if (value is global::Microsoft.Maui.Graphics.Paint paint)
				{
					value = (Brush)paint;
					return true;
				}
				if (value is global::Microsoft.Maui.Graphics.Color color)
				{
					value = (Brush)color;
					return true;
				}

				return false;
			}
		}
	}

	[ShellContent.ImplicitCasts]
	partial class ShellContent
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastTo(ref object value, Type targetType)
			{
				if (value is not ShellContent shellContent)
				{
					return false;
				}
				
				if (targetType == typeof(ShellSection))
				{
					value = (ShellSection)shellContent;
					return true;
				}

				return false;
			}

			public override bool TryCastFrom(ref object value)
			{
				if (value is TemplatedPage templatedPage)
				{
					value = (ShellContent)templatedPage;
					return true;
				}

				return false;
			}
		}
	}

	[ShellItem.ImplicitCasts]
	partial class ShellItem
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is MenuItem menuItem)
				{
					value = (ShellItem)menuItem;
					return true;
				}
				if (value is ShellContent shellContent)
				{
					value = (ShellItem)shellContent;
					return true;
				}
				if (value is ShellSection shellSection)
				{
					value = (ShellItem)shellSection;
					return true;
				}
				if (value is TemplatedPage templatedPage)
				{
					value = (ShellItem)templatedPage;
					return true;
				}

				return false;
			}
		}
	}

	[ShellNavigationState.ImplicitCasts]
	partial class ShellNavigationState
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is Uri uri)
				{
					value = (ShellNavigationState)uri;
					return true;
				}
				if (value is string str)
				{
					value = (ShellNavigationState)str;
					return true;
				}

				return false;
			}
		}
	}

	[WebViewSource.ImplicitCasts]
	partial class WebViewSource
	{
		private sealed class ImplicitCasts : ImplicitCastsAttribute
		{
			public override bool TryCastFrom(ref object value)
			{
				if (value is Uri uri)
				{
					value = (UrlWebViewSource)uri;
					return true;
				}
				if (value is string str)
				{
					value = (UrlWebViewSource)str;
					return true;
				}

				return false;
			}
		}
	}
}
