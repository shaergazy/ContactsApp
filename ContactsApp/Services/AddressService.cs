using ContactsApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace ContactsApp.Services
{
    public class AddressService : IAddressService
    {
        private readonly ContactsDbContext _dbContext;

        public AddressService(ContactsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ObservableCollection<AddressModel>> GetAllAddressesAsync()
        {
            var addresses = await _dbContext.Addresses.ToListAsync();
            return new ObservableCollection<AddressModel>(addresses);
        }
    }
}
