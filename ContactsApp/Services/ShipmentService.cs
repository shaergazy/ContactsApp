using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using EasyPost;
using EasyPost.Models.API;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ContactsApp.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly Client _client;

        public ShipmentService(IOptions<EasyPostSettings> settings)
        {
            var apiKey = settings.Value.ApiKey;
            _client = new Client(new ClientConfiguration(apiKey));
        }

        public async Task CreateShipmentLabel(ParcelDto dto, AddressModel toAddress, AddressModel fromAddress)
        {
            try
            {
                var parameters = new EasyPost.Parameters.Shipment.Create
                {
                    ToAddress = new EasyPost.Parameters.Address.Create
                    {
                        Street1 = toAddress.Street,
                        City = toAddress.City,
                        State = toAddress.State,
                        Country = toAddress.Country,
                        Zip = toAddress.Zip,
                    },
                    FromAddress = new EasyPost.Parameters.Address.Create
                    {
                        Street1 = fromAddress.Street,
                        City = fromAddress.City,
                        State = fromAddress.State,
                        Country = fromAddress.Country,
                        Zip = fromAddress.Zip,
                    },
                    Parcel = new EasyPost.Parameters.Parcel.Create
                    {
                        Length = dto.Length,
                        Width = dto.Width,
                        Height = dto.Height,
                        Weight = dto.Weight,
                    },
                };

                Shipment shipment = await _client.Shipment.Create(parameters);

                Console.WriteLine(JsonConvert.SerializeObject(shipment, Formatting.Indented));
            }
            catch
            {
            }
        }
    }
}
