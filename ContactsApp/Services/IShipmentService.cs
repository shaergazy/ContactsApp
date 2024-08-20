using ContactsApp.Models;
using ContactsApp.Services.DTOs;

namespace ContactsApp.Services
{
    public interface IShipmentService
    {
        Task<string> CreateShipmentLabel(ParcelDto dto, string carrier, string service, AddressModel address, AddressModel fromAddress);
    }
}
