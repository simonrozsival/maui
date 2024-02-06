# Feature Switches

Certain features of MAUI can be enabled or disabled using feature switches. The easiest way to control the features is by putting the corresponding MSBuild property into the app's project file. Disabling unnecessary features can help reducing the app size when combined with the [`full` trimming mode](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options).

| MSBuild Property Name | AppContext Setting | Description |
|-|-|-|
| MauiXamlRuntimeParsingSupport | Microsoft.Maui.RuntimeFeature.IsXamlRuntimeParsingSupported | When disabled, all XAML loading at runtime will throw an exception. This will affect usage of APIs such as the `LoadFromXaml` extension method. This feature can be safely turned off when all XAML resources are compiled using XamlC (see [XAML compilation](https://learn.microsoft.com/en-us/dotnet/maui/xaml/xamlc)). This feature is enabled by default for all configurations except for NativeAOT. |
| MauiQueryPropertyAttributeSupport | Microsoft.Maui.RuntimeFeature.IsQueryPropertyAttributeSupported | When disabled, using `[QueryProperty]` attributes will throw an exception when navigating to the page. Instead of using `[QueryProperty]`, implement the `IQueryAttributable` interface on all affected pages and binding context objects. This feature is enabled by default for all configurations except for NativeAOT. |

## MauiXamlRuntimeParsingSupport

When this feature is disabled, the following APIs are affected:
- [`LoadFromXaml` extension methods](https://learn.microsoft.com/en-us/dotnet/maui/xaml/runtime-load) will throw runtime exceptions.
- [Disabling XAML compilation](https://learn.microsoft.com/en-us/dotnet/maui/xaml/xamlc#disable-xaml-compilation) using `[XamlCompilation(XamlCompilationOptions.Skip)]` on pages and controls or whole assemblies will cause runtime exceptions.

## MauiQueryPropertyAttributeSupport

When this feature is disabled, the following functionality is affected:
- Navigating to pages annotated with [`QueryPropertyAttribute`](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation?view=net-maui-8.0#process-navigation-data-using-query-property-attributes) will throw runtime exceptions.
