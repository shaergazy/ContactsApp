using ContactsApp.Models;
using EasyPost;
using EasyPost.Models.API;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly string _apiKey;

        public ShipmentService(string apiKey)
        {
            _apiKey = apiKey;
            ClientManager.SetCurrent(_apiKey);
        }

        public async Task<bool> CreateShipmentLabel(double weight, double length, double width, double height, string carrier, string service, AddressModel address)
        {
            try
            {
                var fromAddress = new EasyPost.Address()
                {
                    Street1 = "YOUR_SENDER_STREET",
                    City = "YOUR_SENDER_CITY",
                    State = "YOUR_SENDER_STATE",
                    Zip = "YOUR_SENDER_ZIP",
                    Country = "YOUR_SENDER_COUNTRY"
                };

                var toAddress = new EasyPost.Address()
                {
                    Street1 = address.Street,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Country = "US"
                };

                var parcel = new Parcel()
                {
                    Weight = weight,
                    Length = length,
                    Width = width,
                    Height = height
                };

                var shipment = new Shipment()
                {
                    ToAddress = toAddress,
                    FromAddress = fromAddress,
                    Parcel = parcel
                };

                await shipment.Create();

                // Filter by carrier and service
                var rate = shipment.Rates.Find(r => r.Carrier == carrier && r.Service == service);
                if (rate == null)
                    return false;

                shipment.Buy(rate);

                // Save the shipment details or label URL if needed
                var labelUrl = shipment.PostageLabel.LabelUrl;

                // Here you would save the shipment details to your database
                return true;
            }
            catch
            {
                // Handle exceptions (e.g., log error)
                return false;
            }
        }
    }
}
