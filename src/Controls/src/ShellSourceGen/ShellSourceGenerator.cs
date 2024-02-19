using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Maui.Controls.ShellSourceGen;

[Generator(LanguageNames.CSharp)]
public class ShellSourceGenerator : IIncrementalGenerator
{
	const string AutoGeneratedHeaderText = """
		//------------------------------------------------------------------------------
		// <auto-generated>
		//     This code was generated by a .NET MAUI source generator.
		//
		//     Changes to this file may cause incorrect behavior and will be lost if
		//     the code is regenerated.
		// </auto-generated>
		//------------------------------------------------------------------------------
		""";

	const string QueryPropertyAttributeFullName = "Microsoft.Maui.Controls.QueryPropertyAttribute";

	const string QueryPropertyHelper = $$"""
		{{AutoGeneratedHeaderText}}
		#nullable enable
		using System;
		using System.Collections.Generic;
		using System.Net;

		namespace Microsoft.Maui.Controls.SourceGen;

		public static class QueryPropertyHelper
		{
			public static bool TryGetValue<T>(string queryId, IDictionary<string, object?> query, IDictionary<string, object?> oldQuery, out T? value)
			{
				if (query.TryGetValue(queryId, out object? obj))
				{
					value = obj is T valueOfExpectedType
						? valueOfExpectedType
						: (T?)Convert.ChangeType(obj, typeof(T));

					return true;
				}

				if (oldQuery.TryGetValue(queryId, out _))
				{
					value = default;
					return true;
				}

				value = default;
				return false;
			}

			public static bool TryGetStringValue(string queryId, IDictionary<string, object?> query, IDictionary<string, object?> oldQuery, out string? value)
			{
				if (query.TryGetValue(queryId, out object? obj))
				{
					value = obj is string str
						? WebUtility.UrlDecode(str)
						: (string?)Convert.ChangeType(obj, typeof(string));

					return true;
				}

				if (oldQuery.TryGetValue(queryId, out _))
				{
					value = null;
					return true;
				}

				value = null;
				return false;
			}

			public static Exception MissingValueException(string queryId, Type type, string propertyName)
			{
				return new InvalidOperationException($"Query parameter '{queryId}' is missing for non-nullable property '{type}.{propertyName}'.");
			}
		}
		""";

	private record QueryAttributableToGenerate(INamedTypeSymbol ClassSymbol, QueryProperty[] Properties, Diagnostic[] Diagnostics);
	private record QueryProperty(string QueryId, IPropertySymbol Property);

	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{
		initializationContext.RegisterPostInitializationOutput(ctx =>
			ctx.AddSource("QueryPropertyHelper.g.cs", SourceText.From(QueryPropertyHelper, Encoding.UTF8)));

		IncrementalValuesProvider<QueryAttributableToGenerate?> classesWithQueryPropertyAttributes =
			initializationContext.SyntaxProvider
				.ForAttributeWithMetadataName(
					QueryPropertyAttributeFullName,
					predicate: static (s, _) => s is ClassDeclarationSyntax,
					transform: static (ctx, _) => GetQueryAttributableToGenerate(ctx))
				.Where(static m => m is not null);

		initializationContext.RegisterSourceOutput(classesWithQueryPropertyAttributes, GenerateQueryPropertyAttributablePartial);
	}

	private static void GenerateQueryPropertyAttributablePartial(SourceProductionContext sourceProductionContext, QueryAttributableToGenerate? attributable)
	{
		if (attributable is null)
		{
			return;
		}

		foreach (var diagnostic in attributable.Diagnostics)
		{
			sourceProductionContext.ReportDiagnostic(diagnostic);
		}

		var nestedTypes = new Stack<string> {  };

		// TODO this doesn't work well when some of the parent types is generic
		// we need to use similar strategy to https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/

		INamedTypeSymbol? type = attributable.ClassSymbol;
		while (type is not null)
		{
			var name = type.Name;
			if (type.IsGenericType)
			{
				var typeParameters = type.TypeParameters.Select(t => t.Name);
				name = $"{name}<{string.Join(", ", typeParameters)}>";
			}

			nestedTypes.Push(name);
			type = type.ContainingType;
		}

		var sourceBuilder = new StringBuilder();

		sourceBuilder.AppendLine(AutoGeneratedHeaderText);
		sourceBuilder.AppendLine("#nullable enable");
		sourceBuilder.AppendLine();

		if (!attributable.ClassSymbol.ContainingNamespace.IsGlobalNamespace)
		{
			sourceBuilder.AppendLine($"namespace {attributable.ClassSymbol.ContainingNamespace.ToDisplayString()};");
			sourceBuilder.AppendLine();
		}

		var indentation = "";
		while (nestedTypes.Count > 1)
		{
			sourceBuilder.AppendLine($"{indentation}partial class {nestedTypes.Pop()}");
			sourceBuilder.AppendLine($"{indentation}{{");
			indentation += "\t";
		}

		var typeName = nestedTypes.Pop();

		// sourceBuilder.AppendLine($"{indentation}partial class {typeName} : global::Microsoft.Maui.Controls.IQueryPropertyAttributable");
		// sourceBuilder.AppendLine($"{indentation}{{");
		// sourceBuilder.AppendLine($"{indentation}\tpublic void SetQueryProperties(global::System.Collections.Generic.IDictionary<string, object?> query, global::System.Collections.Generic.IDictionary<string, object?> oldQuery)");
		// sourceBuilder.AppendLine($"{indentation}\t{{");

		sourceBuilder.AppendLine($"{indentation}partial class {typeName} : global::Microsoft.Maui.Controls.IQueryAttributable");
		sourceBuilder.AppendLine($"{indentation}{{");
		sourceBuilder.AppendLine($"{indentation}\tpublic void ApplyQueryAttributes(global::System.Collections.Generic.IDictionary<string, object?> query)");
		sourceBuilder.AppendLine($"{indentation}\t{{");
		sourceBuilder.AppendLine($"{indentation}\t\tvar oldQuery = new global::System.Collections.Generic.Dictionary<string, object?>();");
		sourceBuilder.AppendLine();

		foreach (var property in attributable.Properties)
		{
			var nextValueVariableName = $"next{property.Property.Name}";

			sourceBuilder.Append($"{indentation}\t\tif (");
			if (property.Property.Type.Name == "String" && property.Property.Type.ContainingNamespace.Name == "System")
			{
				sourceBuilder.Append($"global::Microsoft.Maui.Controls.SourceGen.QueryPropertyHelper.TryGetStringValue(\"{property.QueryId}\", query, oldQuery, out string? {nextValueVariableName})");
			}
			else
			{
				var unannotatedTypeName = property.Property.Type.ToString();
				if (unannotatedTypeName.EndsWith("?"))
				{
					unannotatedTypeName = unannotatedTypeName.Substring(0, unannotatedTypeName.Length - 1);
				}

				sourceBuilder.Append($"global::Microsoft.Maui.Controls.SourceGen.QueryPropertyHelper.TryGetValue<{unannotatedTypeName}>(\"{property.QueryId}\", query, oldQuery, out {unannotatedTypeName}? {nextValueVariableName})");
			}

			sourceBuilder.AppendLine(")");
			sourceBuilder.AppendLine($"{indentation}\t\t{{");

			if (property.Property.Type.NullableAnnotation == NullableAnnotation.NotAnnotated)
			{
				if (property.Property.Type.IsReferenceType)
				{
					sourceBuilder.AppendLine($"{indentation}\t\t\tif ({nextValueVariableName} is null)");
				}
				else
				{
					sourceBuilder.AppendLine($"{indentation}\t\t\tif (!{nextValueVariableName}.HasValue)");
				}
				sourceBuilder.AppendLine($"{indentation}\t\t\t{{");
				sourceBuilder.AppendLine($"{indentation}\t\t\t\tthrow global::Microsoft.Maui.Controls.SourceGen.QueryPropertyHelper.MissingValueException(\"{property.QueryId}\", typeof({typeName}), nameof({property.Property.Name}));");
				sourceBuilder.AppendLine($"{indentation}\t\t\t}}");
				sourceBuilder.AppendLine();
			}

			if (property.Property.Type.IsReferenceType)
			{
				sourceBuilder.AppendLine($"{indentation}\t\t\tthis.{property.Property.Name} = {nextValueVariableName};");
			}
			else
			{
				sourceBuilder.AppendLine($"{indentation}\t\t\tthis.{property.Property.Name} = {nextValueVariableName}.Value;");
			}

			sourceBuilder.AppendLine($"{indentation}\t\t}}");
		}

		sourceBuilder.AppendLine($"{indentation}\t}}"); // end of SetQueryProperties
		sourceBuilder.AppendLine($"{indentation}}}"); // end of class

		// close all nested types
		while (indentation.Length > 0)
		{
			indentation = indentation.Substring(1);
			sourceBuilder.AppendLine($"{indentation}}}");
		}

		sourceProductionContext.AddSource(
			$"{attributable.ClassSymbol.Name}.IQueryPropertyAttributable.g.cs",
			SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
	}

	private static QueryAttributableToGenerate? GetQueryAttributableToGenerate(GeneratorAttributeSyntaxContext context)
	{
		var semanticModel = context.SemanticModel;
		var classDeclarationSyntax = (ClassDeclarationSyntax)context.TargetNode;
		var classSymbol = (INamedTypeSymbol)context.TargetSymbol;

		// TODO: should we maybe check if the class already has an implementation of `ApplyQueryAttributes(IDictionary<object, string?>)` instead?
		if (classSymbol.AllInterfaces.Any(i => i.Name == "IQueryPropertyAttributable" && i.ContainingNamespace.Name == "Microsoft.Maui.Controls"))
		{
			// if the class already implements IQueryPropertyAttributable, skip it
			return null;
		}

		var properties = new List<QueryProperty>();
		var diagnostics = new List<Diagnostic>();

		foreach (AttributeData attribute in context.Attributes)
		{
			var (propertyName, queryId) = GetConstructorArguments(attribute);

			if (queryId is null || propertyName is null)
			{
				diagnostics.Add(DiagnosticFactory.InvalidPropertyNameOrQueryId(attribute));
			}
			else if (FindProperty(classSymbol, propertyName) is not IPropertySymbol property)
			{
				diagnostics.Add(DiagnosticFactory.QueryPropertyDoesNotExist(attribute));
			}
			else if (property.SetMethod is null)
			{
				diagnostics.Add(DiagnosticFactory.QueryPropertyDoesNotHaveSetter(attribute));
			}
			else if (properties.Any(p => p.Property.Name == propertyName))
			{
				diagnostics.Add(DiagnosticFactory.QueryPropertyAlreadyUsed(attribute));
			}
			else
			{
				properties.Add(new QueryProperty(queryId, property));
			}
		}

		// TODO will this work with caching or should we transform the properties and diagnostics to a custom record type?
		return new QueryAttributableToGenerate(classSymbol, properties.ToArray(), diagnostics.ToArray());
	}

	private static (string?, string?) GetConstructorArguments(AttributeData attribute)
		=> (attribute.ConstructorArguments[0].Value?.ToString(), attribute.ConstructorArguments[1].Value?.ToString());

	private static IPropertySymbol? FindProperty(INamedTypeSymbol classSymbol, string propertyName)
		=> classSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(prop => prop.Name == propertyName);
}
