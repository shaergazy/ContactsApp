using ContactsApp.Models;
using ContactsApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace ContactsApp.Services
{
    public class AddressService : GenericService<AddressModel, int>, IAddressService
    {
        private readonly ContactsDbContext _dbContext;

        public AddressService(ContactsDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ObservableCollection<AddressModel>> GetAll()
        {
            var addresses = _dbContext.Addresses.ToList();
            return new ObservableCollection<AddressModel>(addresses);
        }
    }
}
