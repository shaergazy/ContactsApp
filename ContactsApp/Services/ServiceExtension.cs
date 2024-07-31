using ContactsApp.ViewModels;
using ContactsApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsApp.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            RegisterConnectionString(services, configuration);
            services.AddScoped<IContactService, ContactService>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<AddEditContactViewModel>();

            services.AddTransient<MainWindow>();
            return services;
        }
        public static void RegisterConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ContactsDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("Default")));
        }
    }
}
