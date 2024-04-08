
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

    internal GeneratorSyntaxContext Context { get; }

    internal (List<Diagnostic> diagnostics, LinkedList<IPathPart> parts) ParsePath(CSharpSyntaxNode? expressionSyntax)
    {
        if (expressionSyntax is IdentifierNameSyntax)
        {
            return (new List<Diagnostic>(), new LinkedList<IPathPart>());
        }
        else if (expressionSyntax is MemberAccessExpressionSyntax memberAccess)
        {
            var (diagnostics, parts) = ParsePath(memberAccess.Expression);
            if (diagnostics.Count > 0)
            {
                return (diagnostics, parts);
            }

            var member = memberAccess.Name.Identifier.Text;
            IPathPart part = new MemberAccess(member);
            parts.AddLast(part);
            return (diagnostics, parts);
        }
        else if (expressionSyntax is ElementAccessExpressionSyntax elementAccess)
        {
            var (diagnostics, parts) = ParsePath(elementAccess.Expression);
            if (diagnostics.Count > 0)
            {
                return (diagnostics, parts);
            }

            var argumentList = elementAccess.ArgumentList.Arguments;
            if (argumentList.Count != 1)
            {
                return (new List<Diagnostic> { DiagnosticsFactory.UnableToResolvePath(elementAccess.GetLocation()) }, parts);
            }

            var indexExpression = argumentList[0].Expression;
            IIndex? indexValue = Context.SemanticModel.GetConstantValue(indexExpression).Value switch
            {
                int i => new NumericIndex(i),
                string s => new StringIndex(s),
                _ => null
            };

            if (indexValue is null)
            {
                return (new List<Diagnostic> { DiagnosticsFactory.UnableToResolvePath(elementAccess.GetLocation()) }, parts);
            }

            var defaultMemberName = "Item"; // TODO we need to check the value of the `[DefaultMemberName]` attribute on the member type
            IPathPart part = new IndexAccess(defaultMemberName, indexValue);
            parts.AddLast(part);

            return (diagnostics, parts);
        }
        else if (expressionSyntax is ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var (diagnostics, parts) = ParsePath(conditionalAccess.Expression);
            if (diagnostics.Count > 0)
            {
                return (diagnostics, parts);
            }

            var (diagnosticNotNull, partsNotNull) = ParsePath(conditionalAccess.WhenNotNull);
            if (diagnosticNotNull.Count > 0)
            {
                return (diagnosticNotNull, partsNotNull);
            }

            while (partsNotNull.Count > 0)
            {
                parts.AddLast(partsNotNull.First.Value);
                partsNotNull.RemoveFirst();
            }

            return (diagnostics, parts);
        }
        else if (expressionSyntax is MemberBindingExpressionSyntax memberBinding)
        {
            var member = memberBinding.Name.Identifier.Text;
            IPathPart part = new MemberAccess(member);
            part = new ConditionalAccess(part);

            return (new List<Diagnostic>(), new LinkedList<IPathPart>([part]));
        }
        else if (expressionSyntax is ParenthesizedExpressionSyntax parenthesized)
        {
            return ParsePath(parenthesized.Expression);
        }
        else if (expressionSyntax is BinaryExpressionSyntax asExpression && asExpression.Kind() == SyntaxKind.AsExpression)
        {
            var (diagnostics, parts) = ParsePath(asExpression.Left);
            if (diagnostics.Count > 0)
            {
                return (diagnostics, parts);
            }

            var castTo = asExpression.Right;
            var typeInfo = Context.SemanticModel.GetTypeInfo(castTo).Type;
            if (typeInfo == null)
            {
                return (new List<Diagnostic> { DiagnosticsFactory.UnableToResolvePath(asExpression.GetLocation()) }, new LinkedList<IPathPart>());
            };


            var last = parts.Last;
            parts.RemoveLast();
            parts.AddLast(new Cast(last.Value, BindingGenerationUtilities.CreateTypeDescriptionForCast(typeInfo)));
            return (diagnostics, parts);
        }
        else
        {
            return (new List<Diagnostic> { DiagnosticsFactory.UnableToResolvePath(Location.None) }, new LinkedList<IPathPart>());
        }
    }
}