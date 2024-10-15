using Mono.Cecil;

namespace Microsoft.Maui.Controls.Build.Tasks
{
	static class FieldReferenceExtensions
	{
		public static FieldReference ResolveGenericParameters(this FieldReference self, TypeReference declaringTypeRef)
		{
			var fieldType = self.FieldType;
			if (fieldType.IsGenericParameter)
			{
				var genericParameter = (GenericParameter)fieldType;
				fieldType = ((GenericInstanceType)declaringTypeRef).GenericArguments[genericParameter.Position];
			}
			var fieldReference = new FieldReference(self.Name, fieldType)
			{
				DeclaringType = declaringTypeRef
			};
			return fieldReference;
		}

		public static bool HasDoesNotInheritDataTypeAttribute(this FieldReference field, ModuleDefinition module, ILContext context)
			=> field.Resolve().HasCustomAttributes(module.ImportReference(context.Cache, ("Microsoft.Maui.Controls", "Microsoft.Maui.Controls.Xaml", "DoesNotInheritDataTypeAttribute")));
	}
}