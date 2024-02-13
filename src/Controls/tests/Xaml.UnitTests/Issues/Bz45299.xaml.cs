using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Graphics;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public class Bz45299Control : ContentView
	{
		public static readonly BindableProperty PortraitLayoutProperty =
		   BindableProperty.Create(nameof(PortraitLayout), typeof(Bz45299OrientationLayout), typeof(Bz45299Control));
		public Bz45299OrientationLayout PortraitLayout
		{
			get { return (Bz45299OrientationLayout)GetValue(PortraitLayoutProperty); }
			set { this.SetValue(PortraitLayoutProperty, value); }
		}

	}

	public class Bz45299OrientationLayout : BindableObject
	{
		public static readonly BindableProperty SizeProperty =
		   BindableProperty.Create(nameof(Size), typeof(Bz45299UISize), typeof(Bz45299OrientationLayout), Bz45299UISize.Zero);
		public Bz45299UISize Size
		{
			get { return (Bz45299UISize)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}

		public static readonly BindableProperty SpacingProperty =
		   BindableProperty.Create(nameof(Spacing), typeof(Bz45299UILength), typeof(Bz45299OrientationLayout), Bz45299UILength.Zero);
		public Bz45299UILength Spacing
		{
			get { return (Bz45299UILength)GetValue(SpacingProperty); }
			set { SetValue(SpacingProperty, value); }
		}

		public static readonly BindableProperty CountProperty =
		   BindableProperty.Create(nameof(Count), typeof(int), typeof(Bz45299OrientationLayout), 1);
		public int Count
		{
			get { return (int)GetValue(CountProperty); }
			set { SetValue(CountProperty, value); }
		}
	}

	[System.ComponentModel.TypeConverter(typeof(Bz45299UILengthTypeConverter))]
	[Bz45299UILength.ImplicitCasts]
	public class Bz45299UILength
	{
		public static Bz45299UILength Zero => new Bz45299UILength { Value = 0 };

		public double Value { get; set; }

		public static implicit operator string(Bz45299UILength uiLength) => uiLength.Value.ToString();
		public static implicit operator double(Bz45299UILength uiLength) => uiLength.Value;

		public static implicit operator Bz45299UILength(string value) => Zero;
		public static implicit operator Bz45299UILength(long value) => Zero;
		public static implicit operator Bz45299UILength(ulong value) => Zero;
		public static implicit operator Bz45299UILength(int value) => Zero;
		public static implicit operator Bz45299UILength(uint value) => Zero;
		public static implicit operator Bz45299UILength(double value) => Zero;
		public static implicit operator Bz45299UILength(float value) => Zero;

		private sealed class ImplicitCasts : BaseImplicitCastsAttribute
		{
			public override bool TryCastTo(ref object value, Type toType)
			{
				if (value is not Bz45299UILength uiLength)
					return false;

				if (toType == typeof(string))
				{
					value = uiLength.Value.ToString();
					return true;
				}
				if (toType == typeof(double))
				{
					value = uiLength.Value;
					return true;
				}

				return false;
			}

			public override bool TryCastFrom(ref object value)
			{
				if (value is string str)
				{
					value = (Bz45299UILength)str;
					return true;
				}
				if (value is long l)
				{
					value = (Bz45299UILength)l;
					return true;
				}
				if (value is ulong ul)
				{
					value = (Bz45299UILength)ul;
					return true;
				}
				if (value is int i)
				{
					value = (Bz45299UILength)i;
					return true;
				}
				if (value is uint ui)
				{
					value = (Bz45299UILength)ui;
					return true;
				}
				if (value is double d)
				{
					value = (Bz45299UILength)d;
					return true;
				}
				if (value is float f)
				{
					value = (Bz45299UILength)f;
					return true;
				}

				return false;
			}
		}
	}

	public class Bz45299UILengthTypeConverter : TypeConverter
	{
		static readonly Type StringType = typeof(string);
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType != StringType)
				return false;

			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => Bz45299UILength.Zero;
	}

	[System.ComponentModel.TypeConverter(typeof(Bz45299UISizeTypeConverter))]
	public class Bz45299UISize
	{
		public static Bz45299UISize Zero => new Bz45299UISize { Width = 0, Height = 0 };

		public Bz45299UILength Width { get; set; }
		public Bz45299UILength Height { get; set; }

		public static implicit operator Bz45299UISize(string value) => Zero;
		public static implicit operator Size(Bz45299UISize uiSize) => new Size(uiSize.Width, uiSize.Height);
		public static implicit operator Bz45299UISize(Size size) => new Bz45299UISize { Width = size.Width, Height = size.Height };
	}

	public class Bz45299UISizeTypeConverter : TypeConverter
	{
		private static readonly Type StringType = typeof(string);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType != StringType)
				return false;

			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => Bz45299UISize.Zero;
	}

	public partial class Bz45299 : ContentPage
	{
		public Bz45299() => InitializeComponent();
		public Bz45299(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[TestCase(true)]
			[TestCase(false)]
			public void XamlCCustomTypeConverter(bool useCompiledXaml)
			{
				var p = new Bz45299(useCompiledXaml);
				Assert.AreEqual(0d, p.ctrl.PortraitLayout.Spacing.Value);
			}
		}
	}
}