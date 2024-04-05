using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.Maui.Controls.BindingSourceGen;
using System.Runtime.Loader;
using Xunit;
using System.Collections.Immutable;

internal static class SourceGenHelpers
{
    private static readonly CSharpParseOptions ParseOptions = new CSharpParseOptions(LanguageVersion.Preview).WithFeatures(
                [new KeyValuePair<string, string>("InterceptorsPreviewNamespaces", "Microsoft.Maui.Controls.Generated")]);
    
    internal static CodeWriterBinding GetBinding(string source)
    {
        var results = Run(source).Results.Single();
        var steps = results.TrackedSteps;

        Assert.Empty(results.Diagnostics);

        return (CodeWriterBinding)steps["Bindings"][0].Outputs[0].Value;
    }

    internal static GeneratorDriverRunResult Run(string source)
    {
        var inputCompilation = CreateCompilation(source);

        var compilerErrors = inputCompilation.GetDiagnostics().Where(i => i.Severity == DiagnosticSeverity.Error);

        if (compilerErrors.Any())
        {
            var errorMessages = compilerErrors.Select(error => error.ToString());
            throw new Exception("Compilation errors: " + string.Join("\n", errorMessages));
        }

        var generator = new BindingSourceGenerator();
        var sourceGenerator = generator.AsSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(
            [sourceGenerator],
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true),
            parseOptions: ParseOptions);

        var result = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics).GetRunResult();
        
        // Console.WriteLine("Generated code:");
        // Console.Write(result.Results.Single().GeneratedSources.Single().SourceText.ToString());
        
        var generatedCodeDiagnostic = compilation.GetDiagnostics();
        if (generatedCodeDiagnostic.Any())
        {
            var errorMessages = generatedCodeDiagnostic.Select(error => error.ToString());
            throw new Exception("Generated code compilation errors: " + string.Join("\n", errorMessages));
        }

        return result;
    }
    
    // issue https://github.com/dotnet/roslyn/issues/69906
    
    internal static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source, ParseOptions, path: @"Path\To\Program.cs")],
            [
                MetadataReference.CreateFromFile(typeof(Microsoft.Maui.Controls.BindableObject).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("System.Runtime")).Location),
            ],
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
            .WithNullableContextOptions(NullableContextOptions.Enable));
}