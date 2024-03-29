using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;


namespace BindingSourceGen.UnitTests;


public class BindingRepresentationGenTests
{
    [Fact]
    public void GenerateSimpleBinding()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (string s) => s.Length);
        """;

        var actualBinding = SourceGenHelpers.GetBinding(source) with { Id = 0 }; // TODO: Improve indexing of bindings
        var expectedBinding = new CodeWriterBinding(
                0,
                new SourceCodeLocation("", 3, 7),
                new TypeName("string", false, false),
                new TypeName("int", false, false),
                [
                    new PathPart("s", false),
                    new PathPart("Length", false),
                ],
                true
            );

        //TODO: Change arrays to custom collections implementing IEquatable
        Assert.Equal(expectedBinding.Path, actualBinding.Path);
        Assert.Equivalent(expectedBinding, actualBinding, strict: true);
    }

    [Fact]
    public void GenerateBindingWithNestedProperties()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (Button b) => b.Text.Length);
        """;

        var actualBinding = SourceGenHelpers.GetBinding(source) with { Id = 0 }; // TODO: Improve indexing of bindings
        var expectedBinding = new CodeWriterBinding(
                0,
                new SourceCodeLocation("", 3, 7),
                new TypeName("global::Microsoft.Maui.Controls.Button", false, false),
                new TypeName("int", false, false),
                [
                    new PathPart("b", false),
                    new PathPart("Text", false),
                    new PathPart("Length", false),
                ],
                true
            );

        //TODO: Change arrays to custom collections implementing IEquatable
        Assert.Equal(expectedBinding.Path, actualBinding.Path);
        Assert.Equivalent(expectedBinding, actualBinding, strict: true);
    }

    [Fact]
    public void GenerateBindingWithNullableElementInPath()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (Foo f) => f.Button?.Text.Length);

        class Foo
        {
            public Button? Button { get; set; }
        }
        """;

        var actualBinding = SourceGenHelpers.GetBinding(source) with { Id = 0 }; // TODO: Improve indexing of bindings
        var expectedBinding = new CodeWriterBinding(
                0,
                new SourceCodeLocation("", 3, 7),
                new TypeName("global::Foo", false, false),
                new TypeName("int", true, false),
                [
                    new PathPart("f", false),
                    new PathPart("Button", true),
                    new PathPart("Text", false),
                    new PathPart("Length", false),
                ],
                true
            );

        //TODO: Change arrays to custom collections implementing IEquatable
        Assert.Equal(expectedBinding.Path, actualBinding.Path);
        Assert.Equivalent(expectedBinding, actualBinding, strict: true);

    }

    [Fact]
    public void GenerateBindingWithNullableSource()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (Button? b) => b?.Text.Length);
        """;

        var actualBinding = SourceGenHelpers.GetBinding(source) with { Id = 0 }; // TODO: Improve indexing of bindings
        var expectedBinding = new CodeWriterBinding(
                0,
                new SourceCodeLocation("", 3, 7),
                new TypeName("global::Microsoft.Maui.Controls.Button", true, false),
                new TypeName("int", true, false),
                [
                    new PathPart("b", true),
                    new PathPart("Text", false),
                    new PathPart("Length", false),
                ],
                true
            );

        //TODO: Change arrays to custom collections implementing IEquatable
        Assert.Equal(expectedBinding.Path, actualBinding.Path);
        Assert.Equivalent(expectedBinding, actualBinding, strict: true);

    }
}