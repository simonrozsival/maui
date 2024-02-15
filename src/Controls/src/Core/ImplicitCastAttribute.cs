#nullable enable

using System;

namespace Microsoft.Maui.Controls
{
	[Accelerator.RegisteredCasts]
	partial class Accelerator
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
#pragma warning disable CS0618
				collection.RegisterCast<string, Accelerator>(static x => x);
#pragma warning restore CS0618
			}
		}
	}

	[DoubleCollection.RegisteredCasts]
	partial class DoubleCollection
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<double[], DoubleCollection>(static x => x);
				collection.RegisterCast<float[], DoubleCollection>(static x => x);
			}
		}
	}

	[FileImageSource.RegisteredCasts]
	partial class FileImageSource
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<FileImageSource, string>(static x => x);
				collection.RegisterCast<string, FileImageSource>(static x => x);
			}
		}
	}

	[FormattedString.RegisteredCasts]
	partial class FormattedString
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<string, FormattedString>(static x => (FormattedString)x);
			}
		}
	}

	[ImageSource.RegisteredCasts]
	partial class ImageSource
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<string, ImageSource>(static x => x);
				collection.RegisterCast<Uri, ImageSource>(static x => x);
			}
		}
	}

	[PointCollection.RegisteredCasts]
	partial class PointCollection
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<global::Microsoft.Maui.Graphics.Point[], PointCollection>(static x => x);
			}
		}
	}

	[TemplatedPage.RegisteredCasts]
	partial class TemplatedPage
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<TemplatedPage, ShellContent>(static x => x);
				collection.RegisterCast<TemplatedPage, ShellItem>(static x => x);
				collection.RegisterCast<TemplatedPage, ShellSection>(static x => x);
			}
		}
	}

	[Brush.RegisteredCasts]
	partial class Brush
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<Brush, global::Microsoft.Maui.Graphics.Paint>(static x => x);
				collection.RegisterCast<global::Microsoft.Maui.Graphics.Color, Brush>(static x => x);
				collection.RegisterCast<global::Microsoft.Maui.Graphics.Paint, Brush>(static x => x);
			}
		}
	}

	[ShellContent.RegisteredCasts]
	partial class ShellContent
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<ShellContent, ShellSection>(static x => x);
				collection.RegisterCast<TemplatedPage, ShellContent>(static x => x);
			}
		}
	}

	[ShellItem.RegisteredCasts]
	partial class ShellItem
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<MenuItem, ShellItem>(static x => x);
				collection.RegisterCast<ShellContent, ShellItem>(static x => x);
				collection.RegisterCast<ShellSection, ShellItem>(static x => x);
				collection.RegisterCast<TemplatedPage, ShellItem>(static x => x);
			}
		}
	}

	[ShellNavigationState.RegisteredCasts]
	partial class ShellNavigationState
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<Uri, ShellNavigationState>(static x => x);
				collection.RegisterCast<string, ShellNavigationState>(static x => x);
			}
		}
	}

	[WebViewSource.RegisteredCasts]
	partial class WebViewSource
	{
		private sealed class RegisteredCasts : RegisteredCastsAttribute
		{
			protected internal override void RegisterCasts(ImplicitCastCollectionBuilder collection)
			{
				collection.RegisterCast<Uri, WebViewSource>(static x => x);
				collection.RegisterCast<string, WebViewSource>(static x => x);
			}
		}
	}
}
