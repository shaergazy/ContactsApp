using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsApp.Services
{
    public static class ServiceExtension
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ContactsDbContext>(options =>
                options.UseSqlServer("Server=.; Database=Kinopoisk;Integrated Security=false;User ID=sa;Password=1;TrustServerCertificate=True;"));

            serviceCollection.AddTransient<IContactService, ContactService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
