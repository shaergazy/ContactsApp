using ContactsApp.Models;
using EasyPost.Models.API;
using System.Collections.ObjectModel;

namespace ContactsApp.Services.Interfaces
{
    public interface IShipmentService : IGenericService<ShipmentModel, int> 
    {
        void ExportToPdf(ObservableCollection<Shipment> shipments, string filePath);
        void ExportToCsv(ObservableCollection<Shipment> shipments, string filePath);
    }
}