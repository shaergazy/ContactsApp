using ContactsApp.Models;
using ContactsApp.Services.DTOs;

namespace ContactsApp.Services
{
    public interface IShipmentService
    {
        Task CreateShipmentLabel(ParcelDto dto, AddressModel address, AddressModel fromAddress);
    }
}
