using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using EasyPost;
using EasyPost.Models.API;
using Microsoft.Extensions.Options;

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

        public async Task<string> CreateShipmentLabel(ParcelDto dto, string carrier, string service, AddressModel toAddress, AddressModel fromAddress)
        {
            try
            {
                // Получение адресов
                //Address toAddresss = await _client.Address.Retrieve(toAddress.AddressId);
                //Address fromAddresss = await _client.Address.Retrieve(fromAddress.AddressId);

                EasyPost.Parameters.Shipment.Create parameters = new()
                {
                    ToAddress = new EasyPost.Parameters.Address.Create
                    {
                        Name = "Dr. Steve Brule",
                        Street1 = toAddress.Street,
                        //Street2 = toAddress.,
                        City = toAddress.City,
                        State = toAddress.State,
                        Country = toAddress.Country,
                        Zip = toAddress.Zip,
                    },
                    FromAddress = new EasyPost.Parameters.Address.Create
                    {
                        Company = "EasyPost",
                        Street1 = fromAddress.Street,
                        //Street2 = fromAddress,
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

                if (shipment == null || shipment.Rates == null || !shipment.Rates.Any())
                {
                    throw new Exception("Failed to retrieve rates for shipment.");
                }

                return shipment.TrackingCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании отправления: {ex.Message}");
                throw;
            }
        }

    }
}
