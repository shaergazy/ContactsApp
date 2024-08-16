using ContactsApp.Models;

namespace ContactsApp.Services
{
    public interface IShipmentService
    {
        Task<bool> CreateShipmentLabel(double weight, double length, double width, double height, string carrier, string service, AddressModel address);
    }
}
