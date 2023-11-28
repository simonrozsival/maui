#nullable disable
using System;
using System.Globalization;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	internal class ContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var presenter = parameter as ContentPresenter;

			if (value is View view)
			{
				return ConfigureView(view, presenter);
			}

			if (value is string textContent)
			{
				return ConvertToLabel(textContent, presenter);
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		static View ConfigureView(View view, ContentPresenter presenter)
		{
			if (view is ITextElement && HasTemplateAncestor(presenter, typeof(ITextElement)))
			{
				BindTextProperties(view);
			}

			if (view is IFontElement && HasTemplateAncestor(presenter, typeof(IFontElement)))
			{
				BindFontProperties(view);
			}

			return view;
		}

		static Label ConvertToLabel(string textContent, ContentPresenter presenter)
		{
			var label = new Label
			{
				Text = textContent
			};

			if (HasTemplateAncestor(presenter, typeof(ITextElement)))
			{
				BindTextProperties(label);
			}

			if (HasTemplateAncestor(presenter, typeof(IFontElement)))
			{
				BindFontProperties(label);
			}

			return label;
		}

		static void BindTextProperties(BindableObject content)
		{
			BindProperty<ITextElement, Color>(content, TextElement.TextColorProperty, static element => element.TextColor);
			BindProperty<ITextElement, double>(content, TextElement.CharacterSpacingProperty, static element => element.CharacterSpacing);
			BindProperty<ITextElement, TextTransform>(content, TextElement.TextTransformProperty, static element => element.TextTransform);
		}

		static void BindFontProperties(BindableObject content)
		{
			BindProperty<IFontElement, FontAttributes>(content, FontElement.FontAttributesProperty, static element => element.FontAttributes);
			BindProperty<IFontElement, double>(content, FontElement.FontSizeProperty, static element => element.FontSize);
			BindProperty<IFontElement, string>(content, FontElement.FontFamilyProperty, static element => element.FontFamily);
		}

		static void BindProperty<TSource, TProperty>(BindableObject content, BindableProperty property, Func<TSource, TProperty> getter)
		{
			if (content.IsSet(property) || content.GetIsBound(property))
			{
				// Don't override the property if user has already set it
				return;
			}

			content.SetBinding(property,
				TypedBinding<TSource>.Create<TProperty>(property, getter,
					source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(TSource))));
		}

		static bool HasTemplateAncestor(ContentPresenter presenter, Type type)
		{
			var parent = presenter?.Parent;

			while (parent != null)
			{
				if (type.IsAssignableFrom(parent.GetType()))
				{
					return true;
				}

				if (parent is ContentView)
				{
					break;
				}

				parent = parent.Parent;
			}

			return false;
		}
	}
}