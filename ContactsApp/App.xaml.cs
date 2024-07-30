using ContactsApp.Services;
using ContactsApp.ViewModels;
using ContactsApp.Views;
using System.Windows;
using Unity;

namespace ContactsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceExtension.ConfigureServices();
            var container = new UnityContainer();
            container.RegisterType<IContactService, ContactService>();
            container.RegisterType<MainVM>();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.DataContext = container.Resolve<MainVM>();
            mainWindow.Show();
        }
    }

}
