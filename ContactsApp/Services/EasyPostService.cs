using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using EasyPost;
using EasyPost.Models.API;
using Microsoft.Extensions.Options;

namespace ContactsApp.Services
{
    public class EasyPostService : IEasyPostService
    {
        private readonly Client _client;

        public EasyPostService(IOptions<EasyPostSettings> settings)
        {
            var apiKey = settings.Value.ApiKey;
            _client = new Client(new ClientConfiguration(apiKey));
        }

        public async Task<string> CreateShipmentLabel(ParcelDto dto, string carrier, string service, AddressModel toAddressModel, AddressModel fromAddressModel)
        {
            try
            {
                Address toAddress = await _client.Address.Retrieve(toAddressModel.AddressId);
                Address fromAddress = await _client.Address.Retrieve(fromAddressModel.AddressId);

                EasyPost.Parameters.Shipment.Create parameters = new()
                {
                    ToAddress = toAddress,
                    FromAddress = fromAddress,
                    Parcel = new EasyPost.Parameters.Parcel.Create
                    {
                        Length = dto.Length,
                        Width = dto.Width,
                        Height = dto.Height,
                        Weight = dto.Weight,
                    },
                };

                Shipment shipment = await _client.Shipment.Create(parameters);
                Rate rate = shipment.LowestRate();
                EasyPost.Parameters.Shipment.Buy buyParameters = new(rate);

                shipment = await _client.Shipment.Buy(shipment.Id, buyParameters);

                EasyPost.Parameters.Tracker.All trackerParameters = new()
                {
                    PageSize = 5
                };

                return shipment.TrackingCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании отправления: {ex.Message}");
                throw;
            }
        }

        public async Task<TrackerDto> GetTrackerDataAsync(string trackingCode)
        {
            var tracker = await _client.Tracker.Retrieve(trackingCode);
            if (tracker == null)
                throw new Exception("Tracker does not exist");
            var dto = new TrackerDto
            {
                Carrier = tracker.Carrier,
                PublicUrl = tracker.PublicUrl,
                TrackingCode = trackingCode,
                Status = tracker.Status,
                StatusDetail = tracker.StatusDetail,
                CreatedAt = tracker.CreatedAt,
                UpdatedAt = tracker.UpdatedAt,
            };
            return dto;
        }

        public async Task<Parcel> CreateParcel(ParcelDto dto)
        {
            var parcel = new EasyPost.Parameters.Parcel.Create
            {
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,
                Weight = dto.Weight,
            };
            return await _client.Parcel.Create(parcel);
        }

        public async Task<Address> CreateAndVerifyAddress(AddressModel address)
        {
            try
            {
                EasyPost.Parameters.Address.Create parameters = new()
                {
                    Street1 = address.Street,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Country = address.Country,
                };

                Address verifiedAddress = await _client.Address.CreateAndVerify(parameters);
                return verifiedAddress;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

