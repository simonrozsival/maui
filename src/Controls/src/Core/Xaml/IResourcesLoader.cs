#nullable disable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.Maui.Controls
{
	interface IResourcesLoader
	{
		[RequiresUnreferencedCode("It might not be possible to load arbitrary XAML file at runtime. Ensure all XAML files are compiled.")]
		T CreateFromResource<T>(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo) where T : new();
		string GetResource(string resourcePath, Assembly assembly, object target, IXmlLineInfo lineInfo);
	}
}