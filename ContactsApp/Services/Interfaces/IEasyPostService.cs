using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using EasyPost.Models.API;

namespace ContactsApp.Services.Interfaces
{
    public interface IEasyPostService
    {
        Task<Shipment> CreateShipmentLabel(ParcelDto dto, string carrier, string service, AddressModel address, AddressModel fromAddress);
        Task<TrackerDto> GetTrackerDataAsync(string trackingCode);
        Task<Address> CreateAndVerifyAddress(AddressModel address);
    }
}
