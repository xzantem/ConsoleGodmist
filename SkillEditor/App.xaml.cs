using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace SkillEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set the culture to Polish (uses comma as decimal separator)
        CultureInfo polishCulture = new CultureInfo("pl-PL");
        CultureInfo.DefaultThreadCurrentCulture = polishCulture;
        CultureInfo.DefaultThreadCurrentUICulture = polishCulture;
    }
}