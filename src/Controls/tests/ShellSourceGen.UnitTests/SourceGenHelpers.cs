using System.Reflection;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Microsoft.Maui.Controls.ShellSourceGen;

internal static class SourceGenHelpers
{
    internal static GeneratorDriverRunResult Run(string source)
    {
        var inputCompilation = CreateCompilation(source);
        var driver = CSharpGeneratorDriver.Create(new ShellSourceGenerator());
        return driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _).GetRunResult();
    } 
    
    // TODO use real Microsoft.Maui.Controls reference instead of the stub
    internal static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(stubs) },
            // new[] { MetadataReference.CreateFromFile(typeof(Microsoft.Maui.Controls.QueryPropertyAttribute).GetTypeInfo().Assembly.Location) },
            new[] { MetadataReference.CreateFromFile(typeof(string).GetTypeInfo().Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

    private const string stubs = """
        #nullable enable
        using System;
        using System.Collections.Generic;

        namespace Microsoft.Maui.Controls
        {
            [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
            public class QueryPropertyAttribute : Attribute
            {
                public string Name { get; }
                public string QueryId { get; }

                public QueryPropertyAttribute(string name, string queryId)
                {
                    Name = name;
                    QueryId = queryId;
                }
            }

            public interface IQueryAttributable
            {
                void ApplyQueryAttributes(IDictionary<string, object?> query);
            }
        }
        """;
}
