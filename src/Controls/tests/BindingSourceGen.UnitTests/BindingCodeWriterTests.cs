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
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A"),
                new ConditionalAccess(new MemberAccess("B")),
                new ConditionalAccess(new MemberAccess("C")),
            ],
            GenerateSetter: true));

        var code = codeWriter.GenerateCode();
        AssertExtensions.CodeIsEqual(
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
                using System.CodeDom.Compiler;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                file sealed class InterceptsLocationAttribute : Attribute
                {
                    public InterceptsLocationAttribute(string filePath, int line, int column)
                    {
                        FilePath = filePath;
                        Line = line;
                        Column = column;
                    }
            
                    public string FilePath { get; }
                    public int Line { get; }
                    public int Column { get; }
                }
            }

            namespace Microsoft.Maui.Controls.Generated
            {
                using System;
                using System.CodeDom.Compiler;
                using System.Runtime.CompilerServices;
                using Microsoft.Maui.Controls.Internals;

                {{BindingCodeWriter.GeneratedCodeAttribute}}
                file static class GeneratedBindableObjectExtensions
                {
            
                    {{BindingCodeWriter.GeneratedCodeAttribute}}
                    [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
                    public static void SetBinding1(
                        this BindableObject bindableObject,
                        BindableProperty bindableProperty,
                        Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                        BindingMode mode = BindingMode.Default,
                        IValueConverter? converter = null,
                        object? converterParameter = null,
                        string? stringFormat = null,
                        object? source = null,
                        object? fallbackValue = null,
                        object? targetNullValue = null)
                    {
                        Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                        if (ShouldUseSetter(mode, bindableProperty))
                        {
                            setter = static (source, value) => 
                            {
                                if (source.A is {} p0
                                    && p0.B is {} p1)
                                {
                                    p1.C = value;
                                }
                            };
                        }
                        
                        var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                            getter: source => (getter(source), true),
                            setter,
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
                        bindableObject.SetBinding(bindableProperty, binding);
                    }

                    private static bool ShouldUseSetter(BindingMode mode, BindableProperty bindableProperty)
                        => mode == BindingMode.OneWayToSource
                            || mode == BindingMode.TwoWay
                            || (mode == BindingMode.Default
                                && (bindableProperty.DefaultBindingMode == BindingMode.OneWayToSource
                                    || bindableProperty.DefaultBindingMode == BindingMode.TwoWay));
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
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A"),
                new ConditionalAccess(new MemberAccess("B")),
                new ConditionalAccess(new MemberAccess("C")),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertExtensions.CodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bindableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                if (ShouldUseSetter(mode, bindableProperty))
                {
                    setter = static (source, value) => 
                    {
                        if (source.A is {} p0
                            && p0.B is {} p1)
                        {
                            p1.C = value;
                        }
                    };
                }

                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: source => (getter(source), true),
                    setter,
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

                bindableObject.SetBinding(bindableProperty, binding);
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
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsValueType: false, IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A"),
                new MemberAccess("B"),
                new MemberAccess("C"),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertExtensions.CodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bindableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                if (ShouldUseSetter(mode, bindableProperty))
                {
                    setter = static (source, value) => 
                    {
                        source.A.B.C = value;
                    };
                }

                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: source => (getter(source), true),
                    setter,
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

                bindableObject.SetBinding(bindableProperty, binding);
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
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false, IsValueType: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false, IsValueType: false),
            Path: [
                new MemberAccess("A"),
                new MemberAccess("B"),
                new MemberAccess("C"),
            ],
            GenerateSetter: false));

        var code = codeBuilder.ToString();
        AssertExtensions.CodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bindableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                if (ShouldUseSetter(mode, bindableProperty))
                {
                    throw new InvalidOperationException("Cannot set value on the source object.");
                }

                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: source => (getter(source), true),
                    setter,
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

                bindableObject.SetBinding(bindableProperty, binding);
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
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new IndexAccess("Item", new NumericIndex(12)),
                new ConditionalAccess(new IndexAccess("Indexer", new StringIndex("Abc"))),
                new IndexAccess("Item", new NumericIndex(0)),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();
        AssertExtensions.CodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bindableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                if (ShouldUseSetter(mode, bindableProperty))
                {
                    setter = static (source, value) => 
                    {
                        if (source[12] is {} p0)
                        {
                            p0["Abc"][0] = value;
                        }
                    };
                }

                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: source => (getter(source), true),
                    setter,
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

                bindableObject.SetBinding(bindableProperty, binding);
            }
            """,
            code);
    }

    [Fact]
    public void CorrectlyFormatsBindingWithCasts()
    {
        var codeBuilder = new BindingCodeWriter.BidningInterceptorCodeBuilder();
        codeBuilder.AppendSetBindingInterceptor(id: 1, new CodeWriterBinding(
            Location: new SourceCodeLocation(FilePath: @"Path\To\Program.cs", Line: 20, Column: 30),
            SourceType: new TypeDescription("global::MyNamespace.MySourceClass", IsNullable: false, IsGenericParameter: false),
            PropertyType: new TypeDescription("global::MyNamespace.MyPropertyClass", IsNullable: false, IsGenericParameter: false),
            Path: [
                new MemberAccess("A"),
                new Cast(new TypeDescription("X", IsValueType: false, IsNullable: false, IsGenericParameter: false)),
                new ConditionalAccess(new MemberAccess("B")),
                new Cast(new TypeDescription("Y", IsValueType: false, IsNullable: false, IsGenericParameter: false)),
                new ConditionalAccess(new MemberAccess("C")),
                new Cast(new TypeDescription("Z", IsValueType: true, IsNullable: true, IsGenericParameter: false)),
                new ConditionalAccess(new MemberAccess("D")),
            ],
            GenerateSetter: true));

        var code = codeBuilder.ToString();

        AssertExtensions.CodeIsEqual(
            $$"""
            {{BindingCodeWriter.GeneratedCodeAttribute}}
            [InterceptsLocationAttribute(@"Path\To\Program.cs", 20, 30)]
            public static void SetBinding1(
                this BindableObject bindableObject,
                BindableProperty bindableProperty,
                Func<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass> getter,
                BindingMode mode = BindingMode.Default,
                IValueConverter? converter = null,
                object? converterParameter = null,
                string? stringFormat = null,
                object? source = null,
                object? fallbackValue = null,
                object? targetNullValue = null)
            {
                Action<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>? setter = null;
                if (ShouldUseSetter(mode, bindableProperty))
                {
                    setter = static (source, value) => 
                    {
                        if (source.A is X p0
                            && p0.B is Y p1
                            && p1.C is Z p2)
                        {
                            p2.D = value;
                        }
                    };
                }

                var binding = new TypedBinding<global::MyNamespace.MySourceClass, global::MyNamespace.MyPropertyClass>(
                    getter: source => (getter(source), true),
                    setter,
                    handlers: new Tuple<Func<global::MyNamespace.MySourceClass, object?>, string>[]
                    {
                        new(static source => source, "A"),
                        new(static source => (source.A as X), "B"),
                        new(static source => ((source.A as X)?.B as Y), "C"),
                        new(static source => (((source.A as X)?.B as Y)?.C as Z?), "D"),
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

                bindableObject.SetBinding(bindableProperty, binding);
            }
            """,
            code);
    }

}
