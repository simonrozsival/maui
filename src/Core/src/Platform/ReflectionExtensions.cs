#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Maui.Platform
{
	internal static class ReflectionExtensions
	{
		public static FieldInfo? GetField(
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] this Type type,
			Func<FieldInfo, bool> predicate)
		{
			Type? t = type;
			while (t != null)
			{
				foreach (FieldInfo f in t.GetTypeInfo().DeclaredFields)
				{
					if (predicate(f))
					{
						return f;
					}
				}

				t = t.BaseType;
			}

			return null;
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