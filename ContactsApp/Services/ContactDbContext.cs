using ContactsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactsApp.Services
{
    public class ContactsDbContext : DbContext
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
            : base(options)
        {
        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AddressModel> Addresses { get; set; }
        public DbSet<ShipmentModel> Shipments { get; set; }
        public DbSet<ParcelModel> Parcels { get; set; }
    }
}
