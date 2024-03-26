using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Maui.Controls.BindingSourceGen;

[Generator(LanguageNames.CSharp)]
public class BindingSourceGenerator : IIncrementalGenerator
{
	// TODO:
	// Diagnostics
	// Edge cases
	// Optimizations
	// Add diagnostic when lack of usings prevents code from determining the return type of lambda.
	// Do not process Binding(..., string);

	static int _idCounter = 0;
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var bindingsWithDiagnostics = context.SyntaxProvider.CreateSyntaxProvider(
			predicate: static (node, _) => IsSetBindingMethod(node),
			transform: static (ctx, t) => GetBindingForGeneration(ctx, t)
		)
		.Where(static binding => binding != null)
		.Select(static (binding, t) => binding!)
		.WithTrackingName("BindingsWithDiagnostics")
		.Collect();

		context.RegisterSourceOutput(bindingsWithDiagnostics, (spc, bindings) =>
		{
			var codeWriter = new BindingCodeWriter();

			foreach (var binding in bindings)
			{
				codeWriter.AddBinding(binding);
			}

			spc.AddSource("GeneratedBindableObjectExtensions.g.cs", codeWriter.GenerateCode());
		});
	}

	static bool IsSetBindingMethod(SyntaxNode node)
	{
		return node is InvocationExpressionSyntax invocation
			&& invocation.Expression is MemberAccessExpressionSyntax method
			&& method.Name.Identifier.Text == "SetBinding";
	}

	static bool IsNullable(ITypeSymbol type)
	{
		if (type.IsValueType)
		{
			return false; // TODO: Fix
		}
		if (type.NullableAnnotation == NullableAnnotation.Annotated)
		{
			return true;
		}
		return false;
	}

	static Binding? GetBindingForGeneration(GeneratorSyntaxContext context, CancellationToken t)
	{
		var invocation = (InvocationExpressionSyntax)context.Node;

		var method = invocation.Expression as MemberAccessExpressionSyntax;


		var sourceCodeLocation = new SourceCodeLocation(
			context.Node.SyntaxTree.FilePath,
			method!.Name.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
			method!.Name.GetLocation().GetLineSpan().StartLinePosition.Character + 1
		);

		var argumentList = invocation.ArgumentList.Arguments;
		var getter = argumentList[1].Expression;


		if (getter is not LambdaExpressionSyntax lambda)
		{
			return null; // TODO: Optimize
		}


		if (context.SemanticModel.GetSymbolInfo(lambda).Symbol is not IMethodSymbol symbol)
		{
			return null;
		}; // TODO

		var inputType = symbol.Parameters[0].Type;
		var inputTypeGlobalPath = inputType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		var outputType = symbol.ReturnType;
		var outputTypeGlobalPath = symbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		var inputTypeIsGenericParameter = inputType.Kind == SymbolKind.TypeParameter;
		var outputTypeIsGenericParameter = outputType.Kind == SymbolKind.TypeParameter;

		var parts = new List<PathPart>();
		ParsePath(lambda.Body, context, parts);

		return new Binding(
			Id: ++_idCounter,
			Location: sourceCodeLocation,
			SourceType: new TypeName(inputTypeGlobalPath, IsNullable(inputType), inputTypeIsGenericParameter),
			PropertyType: new TypeName(outputTypeGlobalPath, IsNullable(outputType), outputTypeIsGenericParameter),
			Path: parts.ToArray(),
			GenerateSetter: true //TODO: Implement
		);
	}

	static void ParsePath(CSharpSyntaxNode? expressionSyntax, GeneratorSyntaxContext context, List<PathPart> parts)
	{
		if (expressionSyntax is IdentifierNameSyntax identifier)
		{
			var member = identifier.Identifier.Text;
			var typeInfo = context.SemanticModel.GetTypeInfo(identifier).Type;
			if (typeInfo == null)
			{
				return;
			}; // TODO
			var isNullable = IsNullable(typeInfo);
			parts.Add(new PathPart(member, isNullable));
		}
		else if (expressionSyntax is MemberAccessExpressionSyntax memberAccess)
		{
			var member = memberAccess.Name.Identifier.Text;
			var typeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type;
			if (typeInfo == null)
			{
				return;
			};
			ParsePath(memberAccess.Expression, context, parts); //TODO: Nullable
			parts.Add(new PathPart(member, false));
			return;
		}
		else if (expressionSyntax is ElementAccessExpressionSyntax elementAccess)
		{
			var member = elementAccess.Expression.ToString();
			var typeInfo = context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type;
			if (typeInfo == null)
			{
				return;
			}; // TODO
			parts.Add(new PathPart(member, false, elementAccess.ArgumentList.Arguments[0].Expression)); //TODO: Nullable
			ParsePath(elementAccess.Expression, context, parts);
		}
		else if (expressionSyntax is ConditionalAccessExpressionSyntax conditionalAccess)
		{
			ParsePath(conditionalAccess.Expression, context, parts);
			ParsePath(conditionalAccess.WhenNotNull, context, parts);
			return;
		}
		else if (expressionSyntax is MemberBindingExpressionSyntax memberBinding)
		{
			var member = memberBinding.Name.Identifier.Text;
			parts.Add(new PathPart(member, false)); //TODO: Nullable
			return;
		}
		else if (expressionSyntax is ParenthesizedExpressionSyntax parenthesized)
		{
			ParsePath(parenthesized.Expression, context, parts);
			return;
		}
		else
		{
			return;
		}
	}
}

public sealed record Binding(
	int Id,
	SourceCodeLocation Location,
	TypeName SourceType,
	TypeName PropertyType,
	PathPart[] Path,
	bool GenerateSetter);

public sealed record SourceCodeLocation(string FilePath, int Line, int Column);

public sealed record TypeName(string GlobalName, bool IsNullable, bool IsGenericParameter)
{
	public override string ToString()
		=> IsNullable
			? $"{GlobalName}?"
			: GlobalName;
}

public sealed record PathPart(string Member, bool IsNullable, object? Index = null)
{
	public string MemberName
		=> Index is not null
			? $"{Member}[{Index}]"
			: Member;

	public string PartGetter
		=> Index switch
		{
			string str => $"[\"{str}\"]",
			int num => $"[{num}]",
			null => $".{MemberName}",
			_ => throw new NotSupportedException(),
		};
}
