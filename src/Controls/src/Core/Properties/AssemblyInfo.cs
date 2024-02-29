using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.StyleSheets;
using Compatibility = Microsoft.Maui.Controls.Compatibility;

[assembly: InternalsVisibleTo("iOSUnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.ControlGallery")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Android")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.iOS")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Windows")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Tizen")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Core.Design")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Core.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Android.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Android.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.UAP.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Xaml")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Maps")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Maps.iOS")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Maps.iOS.Classic")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Maps.Android")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility.Maps.Tizen")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Xaml.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.FlexLayout.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Material")]

[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.iOS.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Android.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Windows.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.macOS.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.iOS.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Android.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.DeviceTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Loader")] // Microsoft.Maui.Controls.Loader.dll, Microsoft.Maui.Controls.Internals.ResourceLoader.ResourceProvider, kzu@microsoft.com
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.HotReload.Forms")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.UITest.Validator")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Build.Tasks")]
[assembly: InternalsVisibleTo("Microsoft.Maui")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Pages")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Pages.UnitTests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.CarouselView")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Foldable")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Foldable.UnitTests")]
[assembly: InternalsVisibleTo("WinUI.UITests")]
[assembly: InternalsVisibleTo("Microsoft.Maui.Controls.DeviceTests")]

[assembly: InternalsVisibleTo("CommunityToolkit.Maui")]
[assembly: InternalsVisibleTo("CommunityToolkit.Maui.Core")]
[assembly: InternalsVisibleTo("CommunityToolkit.Maui.UnitTests")]
[assembly: InternalsVisibleTo("CommunityToolkit.Maui.Markup")]
[assembly: InternalsVisibleTo("CommunityToolkit.Maui.Markup.UnitTests")]

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: Preserve]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Microsoft.Maui.Controls.Shapes")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Microsoft.Maui.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Microsoft.Maui", AssemblyName = "Microsoft.Maui")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Microsoft.Maui.Graphics", AssemblyName = "Microsoft.Maui.Graphics")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui/design", "Microsoft.Maui.Controls.Shapes")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui/design", "Microsoft.Maui.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui/design", "Microsoft.Maui", AssemblyName = "Microsoft.Maui")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui/design", "Microsoft.Maui.Graphics", AssemblyName = "Microsoft.Maui.Graphics")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "Microsoft.Maui.Controls.Xaml", AssemblyName = "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "Microsoft.Maui.Controls.Xaml", AssemblyName = "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]

[assembly: XmlnsPrefix("http://schemas.microsoft.com/dotnet/2021/maui", "maui")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/dotnet/2021/maui/design", "d")]