using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Maui.Controls.BindingSourceGen;

internal class DiagnosticFactory
{
    public static Diagnostic ClassIsNotPartial(ClassDeclarationSyntax classDeclaration)
        => Diagnostic.Create(
            new DiagnosticDescriptor(
                "MAUIG2001",
                "Class is not partial",
                "The class '{0}' is not partial. The generated code will not be able to extend it.",
                "SourceGeneration",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            classDeclaration.Keyword.GetLocation(),
            classDeclaration.Identifier.Text);

    // private static Diagnostic CreateAttributeDiagnostic(
    //     AttributeData attribute,
    //     string id,
    //     string title,
    //     string message,
    //     DiagnosticSeverity severity,
    //     params string[] parameters)
    // {
    //     var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
    //     var propertyName = attribute.ConstructorArguments[0].Value?.ToString();
    //     var descriptor = new DiagnosticDescriptor(id, title, message, "SourceGeneration", severity, isEnabledByDefault: true);
    //     return Diagnostic.Create(descriptor, location, parameters);
    // }

    // private static string GetPropertyName(AttributeData attribute) => GetConstructorArgument(attribute, position: 0);
    // private static string GetQueryId(AttributeData attribute) => GetConstructorArgument(attribute, position: 1);

    // private static string GetConstructorArgument(AttributeData attribute, int position)
    //     => attribute.ConstructorArguments[position].Value?.ToString() ?? string.Empty;
}
