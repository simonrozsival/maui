using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;

namespace BindingSourceGen.UnitTests;

internal static class AssertExtensions
{
    internal static void CodeIsEqual(string expectedCode, string actualCode)
    {
        var expectedLines = SplitCode(expectedCode);
        var actualLines = SplitCode(actualCode);

        foreach (var (expectedLine, actualLine) in expectedLines.Zip(actualLines))
        {
            Assert.Equal(expectedLine, actualLine);
        }
    }

    internal static void BindingsAreEqual(CodeWriterBinding expectedBinding, CodeWriterBinding actualBinding)
    {
        //TODO: Change arrays to custom collections implementing IEquatable
        Assert.Equal(expectedBinding.Path, actualBinding.Path);
        Assert.Equivalent(expectedBinding, actualBinding, strict: true);
    }

    private static IEnumerable<string> SplitCode(string code)
        => code.Split(Environment.NewLine)
            .Select(static line => line.Trim())
            .Where(static line => !string.IsNullOrWhiteSpace(line));
}