using Microsoft.CodeAnalysis;
using Xunit;

namespace BindingSourceGen.UnitTests;


public class IncrementalGenerationTests
{
    [Fact]
    public void CompilingTheSameSourceResultsInEqualModels()
    {
        var source = """
        using Microsoft.Maui.Controls;
        var label = new Label();
        label.SetBinding(Label.RotationProperty, static (string s) => s.Length);
        """;

        var inputCompilation1 = SourceGenHelpers.CreateCompilation(source);
        var driver1 = SourceGenHelpers.CreateDriver();
        var result1 = driver1.RunGenerators(inputCompilation1).GetRunResult().Results.Single();

        var inputCompilation2 = SourceGenHelpers.CreateCompilation(source);
        var driver2 = SourceGenHelpers.CreateDriver();
        var result2 = driver2.RunGenerators(inputCompilation2).GetRunResult().Results.Single();

        Assert.Equal(result1.TrackedSteps.Count, result2.TrackedSteps.Count);
        foreach (var (step1, step2) in result1.TrackedSteps.Zip(result2.TrackedSteps))
        {
            Assert.Equal(step1.Key, step2.Key);
            var value1 = step1.Value;
            var value2 = step2.Value;
            foreach (var (runStep1, runStep2) in value1.Zip(value2))
            {
                foreach (var (output1, output2) in runStep1.Outputs.Zip(runStep2.Outputs))
                {
                    Assert.Equal(output1.Reason, output2.Reason);
                    Assert.Equal(output1.Value, output2.Value);
                }
            }
        }
    }

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