using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using EasyPost;
using EasyPost.Models.API;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ContactsApp.Services
{
    public class EasyPostService : IEasyPostService
    {
        private readonly Client _client;
        private readonly ILogger<EasyPostService> _logger;

        public EasyPostService(IOptions<EasyPostSettings> settings, ILogger<EasyPostService> logger)
        {
            var apiKey = settings.Value.ApiKey;
            _client = new Client(new ClientConfiguration(apiKey));
            _logger = logger;
        }

        public async Task<Shipment> CreateShipmentLabel(ParcelDto dto, string carrier, string service, AddressModel toAddressModel, AddressModel fromAddressModel)
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

                return shipment;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating shipment: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<TrackerDto> GetTrackerDataAsync(string trackingCode)
        {
            if (string.IsNullOrWhiteSpace(trackingCode))
            {
                _logger.LogWarning("Tracking code is null or empty.");
                throw new ArgumentException("Tracking code cannot be null or empty.", nameof(trackingCode));
            }

            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving tracker data for code {trackingCode}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<Parcel> CreateParcel(ParcelDto dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Error creating parcel: {ex.Message}", ex);
                throw;
            }
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
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying address: {ex.Message}", ex);
                throw;
            }
        }
    }
}
