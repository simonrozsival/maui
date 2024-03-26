using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;


namespace BindingSourceGen.UnitTests;


public class BindingRepresentationGenTests
{
    [Fact]
    public void GenerateSimpleBinding()
    {
        var source = """
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (string s) => s.Length);
        """;

        var result = SourceGenHelpers.Run(source);
        var results = result.Results.Single();
        var steps = results.TrackedSteps;
        var actualBinding = (Binding)steps["Syntax"][0].Outputs[0].Value;

        var sourceCodeLocation = new SourceCodeLocation("", 2, 7);

        var expectedBinding = new Binding(
                1,
                sourceCodeLocation,
                new TypeName("string", false, false),
                new TypeName("int", false, false),
                [
                    new PathPart("s", false),
                    new PathPart("Length", false),
                ],
                true
            );
        Assert.Equivalent(expectedBinding, actualBinding);
    }

}