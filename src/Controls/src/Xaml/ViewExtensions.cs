//
// ViewExtensions.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Mobile Inception
// Copyright (c) 2013 Xamarin, Inc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Maui.Controls.Xaml
{
	public static class Extensions
	{
		[RequiresUnreferencedCode("It might not be possible to load arbitrary XAML file at runtime. Ensure all XAML files are compiled.")]
		public static TXaml LoadFromXaml<TXaml>(this TXaml view, Type callingType)
		{
			if (!FeatureFlags.IsXamlLoadingEnabled)
			{
				throw new InvalidOperationException("XAML loading at runtime is disabled. Ensure all XAML files are compiled.");
			}

			XamlLoader.Load(view, callingType);
			return view;
		}

		[RequiresUnreferencedCode("It might not be possible to load arbitrary XAML file at runtime. Ensure all XAML files are compiled.")]
		public static TXaml LoadFromXaml<TXaml>(this TXaml view, string xaml)
		{
			if (!FeatureFlags.IsXamlLoadingEnabled)
			{
				throw new InvalidOperationException("XAML loading at runtime is disabled. Ensure all XAML files are compiled.");
			}

			XamlLoader.Load(view, xaml);
			return view;
		}

		[RequiresUnreferencedCode("It might not be possible to load arbitrary XAML file at runtime. Ensure all XAML files are compiled.")]
		internal static TXaml LoadFromXaml<TXaml>(this TXaml view, string xaml, Assembly rootAssembly)
		{
			if (!FeatureFlags.IsXamlLoadingEnabled)
			{
				throw new InvalidOperationException("XAML loading at runtime is disabled. Ensure all XAML files are compiled.");
			}

			XamlLoader.Load(view, xaml, rootAssembly);
			return view;
		}
	}
}