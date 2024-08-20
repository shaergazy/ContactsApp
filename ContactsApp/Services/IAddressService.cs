using ContactsApp.Models;
using System.Collections.ObjectModel;

namespace ContactsApp.Services
{
    public interface IAddressService
    {
        Task<ObservableCollection<AddressModel>> GetAllAddressesAsync();
    }
}
