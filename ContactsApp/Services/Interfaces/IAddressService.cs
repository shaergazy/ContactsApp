using ContactsApp.Models;
using System.Collections.ObjectModel;

namespace ContactsApp.Services.Interfaces
{
    public interface IAddressService : IGenericService<AddressModel, int>
    {
        Task<ObservableCollection<AddressModel>> GetAll();
    }
}
