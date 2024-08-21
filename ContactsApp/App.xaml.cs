using ContactsApp.Services;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels;
using ContactsApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace ContactsApp
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IHost _host;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var cs = _configuration.GetConnectionString("Default");
                    services.AddDbContext<ContactsDbContext>(x => x.UseSqlServer(_configuration.GetConnectionString("Default")));
                    services.AddScoped<IContactService, ContactService>();
                    services.AddScoped<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                    services.AddScoped<IEasyPostService, EasyPostService>();
                    services.AddScoped<IAddressService, AddressService>();
                    services.AddScoped<IShipmentService, ShipmentService>();
                    services.AddScoped<ShipmentViewModel>();
                    services.AddTransient<ShipmentWindow>();
                    services.Configure<EasyPostSettings>(_configuration.GetSection("EasyPost"));
                })
                .Build();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
