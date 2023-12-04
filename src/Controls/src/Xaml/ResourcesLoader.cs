using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Maui.Controls;

[assembly: Dependency(typeof(Microsoft.Maui.Controls.Xaml.ResourcesLoader))]
namespace Microsoft.Maui.Controls.Xaml
{
	class ResourcesLoader : IResourcesLoader
	{
		[RequiresUnreferencedCode("It might not be possible to load arbitrary XAML file at runtime. Ensure all XAML files are compiled.")]
		public T CreateFromResource<T>(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo) where T : new()
		{
			if (!FeatureFlags.IsXamlLoadingEnabled)
			{
				throw new InvalidOperationException("XAML parsing at runtime is not supported. Ensure all XAML files are compiled.");
			}

			var rd = new T();

			var resourceLoadingResponse = Maui.Controls.Internals.ResourceLoader.ResourceProvider2?.Invoke(new Maui.Controls.Internals.ResourceLoader.ResourceLoadingQuery
			{
				AssemblyName = assembly.GetName(),
				ResourcePath = resourcePath,
				Instance = rd,
			});

			var alternateResource = resourceLoadingResponse?.ResourceContent;
			if (alternateResource != null)
			{
				XamlLoader.Load(rd, alternateResource, resourceLoadingResponse.UseDesignProperties);
				return rd;
			}

			var resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, resourcePath);
			if (resourceId == null)
				throw new XamlParseException($"Resource '{resourcePath}' not found.", lineInfo);

			using (var stream = assembly.GetManifestResourceStream(resourceId))
			{
				if (stream == null)
					throw new XamlParseException($"No resource found for '{resourceId}'.", lineInfo);
				using (var reader = new StreamReader(stream))
				{
					rd.LoadFromXaml(reader.ReadToEnd(), assembly);
					return rd;
				}
			}
		}

		public string GetResource(string resourcePath, Assembly assembly, object target, IXmlLineInfo lineInfo)
		{
			var resourceLoadingResponse = Maui.Controls.Internals.ResourceLoader.ResourceProvider2?.Invoke(new Maui.Controls.Internals.ResourceLoader.ResourceLoadingQuery
			{
				AssemblyName = assembly.GetName(),
				ResourcePath = resourcePath,
				Instance = target
			});

			var alternateResource = resourceLoadingResponse?.ResourceContent;
			if (alternateResource != null)
				return alternateResource;

			var resourceId = XamlResourceIdAttribute.GetResourceIdForPath(assembly, resourcePath);
			if (resourceId == null)
				throw new XamlParseException($"Resource '{resourcePath}' not found.", lineInfo);

			using (var stream = assembly.GetManifestResourceStream(resourceId))
			{
				if (stream == null)
					throw new XamlParseException($"No resource found for '{resourceId}'.", lineInfo);
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
	}
}