using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.Maui.Controls.ShellSourceGen;

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

   public static Diagnostic QueryPropertyDoesNotExist(AttributeData attribute)
        => CreateAttributePropertyNameDiagnostic(
            attribute,
            "MAUIG2002",
            "Query property does not exist",
            "The query property '{0}' does not exist on the target class.",
            DiagnosticSeverity.Error);

    public static Diagnostic QueryPropertyDoesNotHaveSetter(AttributeData attribute)
        => CreateAttributePropertyNameDiagnostic(
            attribute,
            "MAUIG2003",
            "Query property does not have a setter",
            "The query property '{0}' does not have a setter.",
            DiagnosticSeverity.Error);

    public static Diagnostic QueryPropertyAlreadyUsed(AttributeData attribute)
        => CreateAttributePropertyNameDiagnostic(
            attribute,
            "MAUIG2004",
            "Query property already used",
            "The property '{0}' has already been used in a different QueryPropertyAttribute on the target class.",
            DiagnosticSeverity.Warning);

    private static Diagnostic CreateAttributePropertyNameDiagnostic(
        AttributeData attribute,
        string id,
        string title,
        string message,
        DiagnosticSeverity severity)
    {
        var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(); // TODO: is this the right location?
        var propertyName = attribute.ConstructorArguments[1].Value?.ToString();

        return Diagnostic.Create(
            new DiagnosticDescriptor(id, title, message, "SourceGeneration", severity, isEnabledByDefault: true),
            location,
            propertyName);
    }
}