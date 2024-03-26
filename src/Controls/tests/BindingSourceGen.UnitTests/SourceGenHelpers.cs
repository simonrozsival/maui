using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.Maui.Controls.BindingSourceGen;

internal static class SourceGenHelpers
{
    internal static GeneratorDriverRunResult Run(string source)
    {
        var inputCompilation = CreateCompilation(source);
        var generator = new BindingSourceGenerator();
        var sourceGenerator = generator.AsSourceGenerator();
        var driver = CSharpGeneratorDriver.Create([sourceGenerator], driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));
        return driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _).GetRunResult();
    } 
    internal static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source)],
            [
                MetadataReference.CreateFromFile(typeof(Microsoft.Maui.Controls.BindableObject).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            ],
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}