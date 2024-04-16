using Microsoft.CodeAnalysis;
using Xunit;

namespace BindingSourceGen.UnitTests;


public class IncrementalGenerationTests
{
    [Fact]
    public void DoesNotRegenerateCodeWhenNoChanges()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (string s) => s.Length);
        """;

        var inputCompilation = SourceGenHelpers.CreateCompilation(source);
        var driver = SourceGenHelpers.CreateDriver();

        var result = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out Compilation compilation, out _).GetRunResult().Results.Single();

        var steps = result.TrackedSteps;

        var reasons = steps.SelectMany(step => step.Value).SelectMany(x => x.Outputs).Select(x => x.Reason);
        Assert.All(reasons, reason => Assert.Equal(IncrementalStepRunReason.New, reason));


        // Run again with the same source
        result = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out compilation, out _).GetRunResult().Results.Single();

        steps = result.TrackedSteps;

        reasons = steps.SelectMany(step => step.Value).SelectMany(x => x.Outputs).Select(x => x.Reason);
        Assert.All(reasons, reason => Assert.Equal(IncrementalStepRunReason.Cached, reason));
    }

}