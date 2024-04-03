using System.Linq;
using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;

namespace BindingSourceGen.UnitTests;

public class BindingCodeWriterTests
{
    [Fact]
    public void BuildsWholeDocument()
    {
        var codeWriter = new BindingCodeWriter();
        codeWriter.AddBinding(new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A", IsNullable: true),
                new MemberAccess("B", IsNullable: false),
                new MemberAccess("C", IsNullable: true),
            ],
            GenerateSetter: true));

        var code = codeWriter.GenerateCode();
        AssertCodeIsEqual(
            $$"""
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     This code was generated by a .NET MAUI source generator.
            //
            //     Changes to this file may cause incorrect behavior and will be lost if
            //     the code is regenerated.
            // </auto-generated>
            //------------------------------------------------------------------------------
            #nullable enable

            namespace System.Runtime.CompilerServices
            {
                using System;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                file sealed class InterceptsLocationAttribute(string filePath, int line, int column) : Attribute
                {
                }
            }

            namespace Microsoft.Maui.Controls.Generated
            {
                using System;
                using System.CodeDom.Compiler;
                using System.Runtime.CompilerServices;
                using Microsoft.Maui.Controls.Internal;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                file static class GeneratedBindableObjectExtensions
                {
            
                    {{BindingCodeWriter.GeneratedCodeAttribute}}
                    [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
                    public static void SetBinding1(
                        this BindableObject bindableObject,
                        BindableProperty bidnableProperty,
                        Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                        BindingMode mode = BindingMode.Default,
                        IValueConverter? converter = null,
                        object? converterParameter = null,
                        string? stringFormat = null,
                        object? source = null,
                        object? fallbackValue = null,
                        object? targetNullValue = null)
                    {
                        var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                            getter: static source => (getter(source), true),
                            setter: static (source, value) => 
                            {
                                if (source.A?.B is null)
                                {
                                    return;
                                }
                                source.A.B.C = value;
                            },
                            handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                            {
                                new(static source => source, "A"),
                                new(static source => source.A, "B"),
                                new(static source => source.A?.B, "C"),
                            })
                        {
                            Mode = mode,
                            Converter = converter,
                            ConverterParameter = converterParameter,
                            StringFormat = stringFormat,
                            Source = source,
                            FallbackValue = fallbackValue,
                            TargetNullValue = targetNullValue
                        };
                        bindableObject.SetBinding(bidnableProperty, binding);
                    }
                }
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsSimpleBinding()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A", IsNullable: true),
                new MemberAccess("B", IsNullable: false),
                new MemberAccess("C", IsNullable: true),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertCodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bidnableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: static source => (getter(source), true),
                    setter: static (source, value) => 
                    {
                        if (source.A?.B is null)
                        {
                            return;
                        }
                        source.A.B.C = value;
                    },
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "A"),
                        new(static source => source.A, "B"),
                        new(static source => source.A?.B, "C"),
                    })
                {
                    Mode = mode,
                    Converter = converter,
                    ConverterParameter = converterParameter,
                    StringFormat = stringFormat,
                    Source = source,
                    FallbackValue = fallbackValue,
                    TargetNullValue = targetNullValue
                };
                bindableObject.SetBinding(bidnableProperty, binding);
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithoutAnyNullablesInPath()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A", IsNullable: false),
                new MemberAccess("B", IsNullable: false),
                new MemberAccess("C", IsNullable: false),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertCodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bidnableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: static source => (getter(source), true),
                    setter: static (source, value) => 
                    {
                        source.A.B.C = value;
                    },
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "A"),
                        new(static source => source.A, "B"),
                        new(static source => source.A.B, "C"),
                    })
                {
                    Mode = mode,
                    Converter = converter,
                    ConverterParameter = converterParameter,
                    StringFormat = stringFormat,
                    Source = source,
                    FallbackValue = fallbackValue,
                    TargetNullValue = targetNullValue
                };
                bindableObject.SetBinding(bidnableProperty, binding);
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithoutSetter()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A", IsNullable: false),
                new MemberAccess("B", IsNullable: false),
                new MemberAccess("C", IsNullable: false),
            ],
            GenerateSetter: false));

        var code = codeBuilder.ToString();
        AssertCodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bidnableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: static source => (getter(source), true),
                    setter: null,
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "A"),
                        new(static source => source.A, "B"),
                        new(static source => source.A.B, "C"),
                    })
                {
                    Mode = mode,
                    Converter = converter,
                    ConverterParameter = converterParameter,
                    StringFormat = stringFormat,
                    Source = source,
                    FallbackValue = fallbackValue,
                    TargetNullValue = targetNullValue
                };

                bindableObject.SetBinding(bidnableProperty, binding);
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithIndexers()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new IndexAccess("Item", IsNullable: true, Index: 12),
                new IndexAccess("Indexer", IsNullable: false, Index: "Abc"),
                new IndexAccess("Item", IsNullable: false, Index: 0)
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertCodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bidnableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: static source => (getter(source), true),
                    setter: static (source, value) => 
                    {
                        if (source[12]?["Abc"] is null)
                        {
                            return;
                        }
                        source[12]["Abc"][0] = value;
                    },
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "Item[12]"),
                        new(static source => source[12], "Indexer[Abc]"),
                        new(static source => source[12]?["Abc"], "Item[0]"),
                    })
                {
                    Mode = mode,
                    Converter = converter,
                    ConverterParameter = converterParameter,
                    StringFormat = stringFormat,
                    Source = source,
                    FallbackValue = fallbackValue,
                    TargetNullValue = targetNullValue
                };

                bindableObject.SetBinding(bidnableProperty, binding);
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsSimpleCast()
    {
        var generatedCode = BindingCodeWriter.BidningInterceptorCodeBuilder.GenerateConditionalPathAccess(
            variableName: "source",
            path: [
                new Cast(new MemberAccess("A", IsNullable: true), TargetType: new TypeName("X", IsNullable: false, IsGenericParameter: false, IsValueType: false)),
                new MemberAccess("B", IsNullable: false),
            ],
            depth: 2);

        Assert.Equal("(source.A as X)?.B", generatedCode);
    }

    [Fact]
    public void CorrectlyFormatsSimpleCastOfValueTypes()
    {
        var generatedCode = BindingCodeWriter.BidningInterceptorCodeBuilder.GenerateConditionalPathAccess(
            variableName: "source",
            path: [
                new Cast(new MemberAccess("A", IsNullable: true), TargetType: new TypeName("X", IsNullable: false, IsGenericParameter: false, IsValueType: true)),
                new MemberAccess("B", IsNullable: false),
            ],
            depth: 2);

        Assert.Equal("(source.A as X?)?.B", generatedCode);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithCasts()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeName("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeName("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new Cast(new MemberAccess("A", IsNullable: true), TargetType: new TypeName("X", IsNullable: false, IsGenericParameter: false, IsValueType: false)),
                new Cast(new MemberAccess("B", IsNullable: true), TargetType: new TypeName("Y", IsNullable: false, IsGenericParameter: false, IsValueType: false)),
                new Cast(new MemberAccess("C", IsNullable: false), TargetType: new TypeName("Z", IsNullable: false, IsGenericParameter: false, IsValueType: true)),
                new MemberAccess("D", IsNullable: false),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertCodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bidnableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: static source => (getter(source), true),
                    setter: static (source, value) => 
                    {
                        if (((source.A as X)?.B as Y)?.C as Z? is null)
                        {
                            return;
                        }
                        ((Z?)((Y)((X)source.A).B).C).D = value;
                    },
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "A"),
                        new(static source => source.A as X, "B"),
                        new(static source => (source.A as X)?.B as Y, "C"),
                        new(static source => ((source.A as X)?.B as Y)?.C as Z?, "D"),
                    })
                {
                    Mode = mode,
                    Converter = converter,
                    ConverterParameter = converterParameter,
                    StringFormat = stringFormat,
                    Source = source,
                    FallbackValue = fallbackValue,
                    TargetNullValue = targetNullValue
                };

                bindableObject.SetBinding(bidnableProperty, binding);
            }
            """,
            code);
    }

    private static void AssertCodeIsEqual(string expectedCode, string actualCode)
    {
        var expectedLines = SplitCode(expectedCode);
        var actualLines = SplitCode(actualCode);

        foreach (var (expectedLine, actualLine) in expectedLines.Zip(actualLines))
        {
            Assert.Equal(expectedLine, actualLine);
        }
    }

    private static IEnumerable<string> SplitCode(string code)
        => code.Split(Environment.NewLine)
            .Select(static line => line.Trim())
            .Where(static line => !string.IsNullOrWhiteSpace(line));
}
