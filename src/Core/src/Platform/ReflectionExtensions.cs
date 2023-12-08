#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Maui.Platform
{
	internal static class ReflectionExtensions
	{
		public static FieldInfo? GetField([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] this Type type, Func<FieldInfo, bool> predicate)
		{
			return GetFields(type).FirstOrDefault(predicate);
		}

		public static IEnumerable<FieldInfo> GetFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] this Type? type)
		{
			while (type != null)
			{
				var declaredFields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				foreach (FieldInfo field in declaredFields)
					yield return field;
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