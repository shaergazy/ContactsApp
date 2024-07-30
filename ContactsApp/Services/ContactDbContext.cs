using ContactsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactsApp.Services
{
    public class ContactsDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public ContactsDbContext(DbContextOptions<ContactsDbContext> options)
            : base(options)
        {
        }
    }
}
