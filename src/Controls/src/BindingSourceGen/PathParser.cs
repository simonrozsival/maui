
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Microsoft.Maui.Controls.BindingSourceGen;

internal class PathParser
{
    internal static bool ParsePath(CSharpSyntaxNode? expressionSyntax, GeneratorSyntaxContext context, List<IPathPart> parts)
    {
        if (expressionSyntax is IdentifierNameSyntax)
        {
            return true;
        }
        else if (expressionSyntax is MemberAccessExpressionSyntax memberAccess)
        {
            var member = memberAccess.Name.Identifier.Text;
            if (!ParsePath(memberAccess.Expression, context, parts))
            {
                return false;
            }

            IPathPart part = new MemberAccess(member);

            parts.Add(part);
            return true;
        }
        else if (expressionSyntax is ElementAccessExpressionSyntax elementAccess)
        {
            var argumentList = elementAccess.ArgumentList.Arguments;
            if (argumentList.Count != 1)
            {
                return false;
            }
            var indexExpression = argumentList[0].Expression;
            IIndex? indexValue = context.SemanticModel.GetConstantValue(indexExpression).Value switch
            {
                int i => new NumericIndex(i),
                string s => new StringIndex(s),
                _ => null
            };

            if (indexValue is null)
            {
                return false;
            }

            if (!ParsePath(elementAccess.Expression, context, parts))
            {
                return false;
            }

            var defaultMemberName = "Item"; // TODO we need to check the value of the `[DefaultMemberName]` attribute on the member type
            IPathPart part = new IndexAccess(defaultMemberName, indexValue);
            parts.Add(part);
            return true;
        }
        else if (expressionSyntax is ConditionalAccessExpressionSyntax conditionalAccess)
        {
            return ParsePath(conditionalAccess.Expression, context, parts) &&
            ParsePath(conditionalAccess.WhenNotNull, context, parts);
        }
        else if (expressionSyntax is MemberBindingExpressionSyntax memberBinding)
        {
            var member = memberBinding.Name.Identifier.Text;
            IPathPart part = new MemberAccess(member);
            part = new ConditionalAccess(part);
            parts.Add(part);
            return true;
        }
        else if (expressionSyntax is ParenthesizedExpressionSyntax parenthesized)
        {
            return ParsePath(parenthesized.Expression, context, parts);
        }
        else if (expressionSyntax is BinaryExpressionSyntax asExpression && asExpression.Kind() == SyntaxKind.AsExpression)
        {
            var castTo = asExpression.Right;
            var typeInfo = context.SemanticModel.GetTypeInfo(castTo).Type;
            if (typeInfo == null)
            {
                return false;
            };

            if (!ParsePath(asExpression.Left, context, parts))
            {
                return false;
            }

            int last = parts.Count - 1;
            parts[last] = new Cast(parts[last], BindingGenerationUtilities.CreateTypeDescriptionForCast(typeInfo));
            return true;
        }
        else
        {
            return false;
        }
    }
}