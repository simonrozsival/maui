#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Maui.Platform
{
	internal static class ReflectionExtensions
	{
		private const DynamicallyAccessedMemberTypes allFields = DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields;
		
		public static FieldInfo? GetField([DynamicallyAccessedMembers(allFields)] this Type type, Func<FieldInfo, bool> predicate)
		{
			return GetFields(type).FirstOrDefault(predicate);
		}

		public static IEnumerable<FieldInfo> GetFields([DynamicallyAccessedMembers(allFields)] this Type? type)
		{
			while (type != null)
			{
				foreach (FieldInfo f in type.GetFields())
					yield return f;
				type = type.BaseType;
			}
		}

		internal static object[]? GetCustomAttributesSafe(this Assembly assembly, Type attrType)
		{
			try
			{
				return assembly.GetCustomAttributes(attrType, true);
			}
			catch (FileNotFoundException)
			{
				// Sometimes the previewer doesn't actually have everything required for these loads to work
				// TODO: Register the exception in the Log when we have the Logger ported
			}

			return null;
		}

		public static bool IsInstanceOfType(this Type self, object o)
		{
			return self.IsAssignableFrom(o.GetType());
		}
	}
}