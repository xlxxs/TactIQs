using System.Configuration;
using System.Data;
using System.Windows;
using TactIQ.Miscellaneous;

namespace TactIQ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Erstellen der Datenbank bei Programmstart
            DatabaseBuilder.Initialize();
        }
    }

}
