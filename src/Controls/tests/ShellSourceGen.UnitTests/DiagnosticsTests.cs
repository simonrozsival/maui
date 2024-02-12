using Microsoft.Maui.Controls.ShellSourceGen;

namespace ShellSourceGen.UnitTests;

public class DiagnosticsTests
{
    [Fact]
    public void ReportsWarningWhenPropertyDoesNotExist()
    {
        var source = """
            using Microsoft.Maui.Controls;

            [QueryProperty("a", "A")]
            public partial class MyPage : ContentPage
            {
                public string B { get; set; }
            }
            """;

        var result = SourceGenHelpers.Run(source);

        Assert.Single(result.Diagnostics);
        Assert.Equal("MAUIG2002", result.Diagnostics[0].Id);
    }

    [Fact]
    public void ReportsWarningWhenPropertyDoesNotHaveSetter()
    {
        var source = """
            using Microsoft.Maui.Controls;

            [QueryProperty("a", "A")]
            public partial class MyPage : ContentPage
            {
                public string A { get; }
            }
            """;

        var result = SourceGenHelpers.Run(source);

        Assert.Single(result.Diagnostics);
        Assert.Equal("MAUIG2003", result.Diagnostics[0].Id);
    }

    [Fact]
    public void ReportsWarningWhenPropertyIsUsedRepeatedly()
    {
        var source = """
            using Microsoft.Maui.Controls;

            [QueryProperty("a", "A")]
            [QueryProperty("b", "A")]
            public partial class MyPage : ContentPage
            {
                public string A { get; set; }
            }
            """;

        var result = SourceGenHelpers.Run(source);

        Assert.Single(result.Diagnostics);
        Assert.Equal("MAUIG2004", result.Diagnostics[0].Id);
    }

    [Fact]
    public void NoDiagnosticsWhenInputIsValid()
    {
        var source = """
            using Microsoft.Maui.Controls;

            [QueryProperty("a", "A")]
            [QueryProperty("b", "B")]
            public partial class MyPage : ContentPage
            {
                public string A { get; set; }
                public int B { get; set; }
            }
            """;

        var result = SourceGenHelpers.Run(source);

        Assert.Empty(result.Diagnostics);
    }
}