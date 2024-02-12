﻿#nullable disable

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public abstract partial class Layout
	{
		internal static new void RemapForControls()
		{
		}

		/// <summary>
		/// The associated property mapper for this layout.
		/// </summary>
		[Obsolete("Use LayoutHandler.Mapper instead.")]
		public static IPropertyMapper<IView, IViewHandler> ControlsLayoutMapper = new PropertyMapper<IView, IViewHandler>(ControlsVisualElementMapper);
	}
}
