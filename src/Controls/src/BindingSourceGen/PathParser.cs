
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Microsoft.Maui.Controls.BindingSourceGen;

internal class PathParser
{
    internal PathParser(GeneratorSyntaxContext context)
    {
        Context = context;
    }

    private GeneratorSyntaxContext Context { get; }

    internal (Diagnostic[] diagnostics, List<IPathPart> parts) ParsePath(CSharpSyntaxNode? expressionSyntax)
    {
        return expressionSyntax switch
        {
            IdentifierNameSyntax _ => ([], new List<IPathPart>()),
            MemberAccessExpressionSyntax memberAccess => HandleMemberAccessExpression(memberAccess),
            ElementAccessExpressionSyntax elementAccess => HandleElementAccessExpression(elementAccess),
            ConditionalAccessExpressionSyntax conditionalAccess => HandleConditionalAccessExpression(conditionalAccess),
            MemberBindingExpressionSyntax memberBinding => HandleMemberBindingExpression(memberBinding),
            ParenthesizedExpressionSyntax parenthesized => ParsePath(parenthesized.Expression),
            BinaryExpressionSyntax asExpression when asExpression.Kind() == SyntaxKind.AsExpression => HandleBinaryExpression(asExpression),
            _ => HandleDefaultCase(),
        };
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
    {
        var (diagnostics, parts) = ParsePath(memberAccess.Expression);
        if (diagnostics.Length > 0)
        {
            return (diagnostics, parts);
        }

        var member = memberAccess.Name.Identifier.Text;
        IPathPart part = new MemberAccess(member);
        parts.Add(part);
        return (diagnostics, parts);
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleElementAccessExpression(ElementAccessExpressionSyntax elementAccess)
    {
        var (diagnostics, parts) = ParsePath(elementAccess.Expression);
        if (diagnostics.Length > 0)
        {
            return (diagnostics, parts);
        }

        var argumentList = elementAccess.ArgumentList.Arguments;
        if (argumentList.Count != 1)
        {
            return (new Diagnostic[] { DiagnosticsFactory.UnableToResolvePath(elementAccess.GetLocation()) }, parts);
        }

        var indexExpression = argumentList[0].Expression;
        object? indexValue = Context.SemanticModel.GetConstantValue(indexExpression).Value;
        if (indexValue is null)
        {
            return (new Diagnostic[] { DiagnosticsFactory.UnableToResolvePath(elementAccess.GetLocation()) }, parts);
        }

        var name = GetIndexerName(elementAccess);
        IPathPart part = new IndexAccess(name, indexValue);
        parts.Add(part);

        return (diagnostics, parts);
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleConditionalAccessExpression(ConditionalAccessExpressionSyntax conditionalAccess)
    {
        var (diagnostics, parts) = ParsePath(conditionalAccess.Expression);
        if (diagnostics.Length > 0)
        {
            return (diagnostics, parts);
        }

        var (diagnosticNotNull, partsNotNull) = ParsePath(conditionalAccess.WhenNotNull);
        if (diagnosticNotNull.Length > 0)
        {
            return (diagnosticNotNull, partsNotNull);
        }

        parts.AddRange(partsNotNull);
        return (diagnostics, parts);
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleMemberBindingExpression(MemberBindingExpressionSyntax memberBinding)
    {
        var member = memberBinding.Name.Identifier.Text;
        IPathPart part = new MemberAccess(member);
        part = new ConditionalAccess(part);

        return ([], new List<IPathPart>([part]));
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleBinaryExpression(BinaryExpressionSyntax asExpression)
    {
        var (diagnostics, parts) = ParsePath(asExpression.Left);
        if (diagnostics.Length > 0)
        {
            return (diagnostics, parts);
        }

        var castTo = asExpression.Right;
        var typeInfo = Context.SemanticModel.GetTypeInfo(castTo).Type;
        if (typeInfo == null)
        {
            return (new Diagnostic[] { DiagnosticsFactory.UnableToResolvePath(asExpression.GetLocation()) }, new List<IPathPart>());
        };

        parts.Add(new Cast(BindingGenerationUtilities.CreateTypeDescriptionForCast(typeInfo)));
        return (diagnostics, parts);
    }

    private (Diagnostic[] diagnostics, List<IPathPart> parts) HandleDefaultCase()
    {
        return (new Diagnostic[] { DiagnosticsFactory.UnableToResolvePath(Context.Node.GetLocation()) }, new List<IPathPart>());
    }

    private string GetIndexerName(ElementAccessExpressionSyntax elementAccess)
    {
        const string DefaultName = "Item";

        var typeSymbol = Context.SemanticModel.GetTypeInfo(elementAccess.Expression).Type;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return DefaultName;
        }

        var defaultMemberAttribute = GetAttribute(typeSymbol, "DefaultMemberAttribute");
        if (defaultMemberAttribute != null)
        {
            return GetAttributeValue(defaultMemberAttribute);
        }


        var symbol = Context.SemanticModel.GetSymbolInfo(elementAccess).Symbol;
        if (symbol is IPropertySymbol propertySymbol)
        {
            var indexerNameAttr = GetAttribute(propertySymbol, "IndexerNameAttribute");

            if (indexerNameAttr != null)
            {
                return GetAttributeValue(indexerNameAttr);
            }
        }

        return DefaultName;

        AttributeData? GetAttribute(ISymbol symbol, string attributeName)
        {
            return symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name == attributeName);
        }

        string GetAttributeValue(AttributeData attribute)
        {
            return (attribute.ConstructorArguments.Length > 0 ? attribute.ConstructorArguments[0].Value as string : null) ?? DefaultName;
        }
    }
}