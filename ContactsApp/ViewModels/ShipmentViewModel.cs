using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using EasyPost.Models.API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

public class ShipmentViewModel : INotifyPropertyChanged
{
    private readonly IEasyPostService _easyPostService;
    private readonly IAddressService _addressService;
    private readonly IShipmentService _shipmentService;

    public ParcelDto Parcel { get; set; } = new ParcelDto();
    public ObservableCollection<string> Carriers { get; set; } = new ObservableCollection<string> { "USPS", "FedEx", "DHL Express", "UPS" };
    public ObservableCollection<string> Services { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<Rate> Rates { get; set; } = new ObservableCollection<Rate>();
    public ObservableCollection<AddressModel> Addresses { get; set; } = new ObservableCollection<AddressModel>();
    public ObservableCollection<AddressModel> FromAddresses { get; set; } = new ObservableCollection<AddressModel>();

    private string _selectedCarrier;
    public string SelectedCarrier
    {
        get => _selectedCarrier;
        set
        {
            _selectedCarrier = value;
            OnPropertyChanged(nameof(SelectedCarrier));
            _ = LoadServicesForCarrierAsync();
            _ = UpdateRatesAsync();
        }
    }

    private string _selectedService;
    public string SelectedService
    {
        get => _selectedService;
        set
        {
            _selectedService = value;
            OnPropertyChanged(nameof(SelectedService));
            _ = UpdateRatesAsync();
        }
    }

    private AddressModel _selectedFromAddress;
    public AddressModel SelectedFromAddress
    {
        get => _selectedFromAddress;
        set
        {
            _selectedFromAddress = value;
            OnPropertyChanged(nameof(SelectedFromAddress));
        }
    }

    private AddressModel _selectedAddress;
    public AddressModel SelectedAddress
    {
        get => _selectedAddress;
        set
        {
            _selectedAddress = value;
            OnPropertyChanged(nameof(SelectedAddress));
        }
    }

    private string _shipmentId;
    public string ShipmentId
    {
        get => _shipmentId;
        set
        {
            _shipmentId = value;
            OnPropertyChanged(nameof(ShipmentId));
        }
    }

    public ICommand CreateShipmentCommand { get; }
    public ICommand BuyLabelCommand { get; }
    public ICommand CancelCommand { get; }

    public ShipmentViewModel(IEasyPostService easyPostService, IAddressService addressService, IShipmentService shipmentService)
    {
        _easyPostService = easyPostService;
        _addressService = addressService;
        _shipmentService = shipmentService;

        CreateShipmentCommand = new RelayCommand(async () => await CreateShipmentAsync());
        BuyLabelCommand = new RelayCommand(async () => await BuyLabelAsync());
        CancelCommand = new RelayCommand(Cancel);

        _ = LoadAddressesAsync();
    }

    private async Task LoadAddressesAsync()
    {
        try
        {
            var addresses = await _addressService.GetAll();
            UpdateCollection(Addresses, addresses);
            UpdateCollection(FromAddresses, addresses);
        }
        catch (Exception ex)
        {
            ShowError("Failed to load addresses", ex);
        }
    }

    private async Task LoadServicesForCarrierAsync()
    {
        try
        {
            Services.Clear();

            if (!string.IsNullOrEmpty(SelectedCarrier))
            {
                var services = await _easyPostService.GetServicesForCarrierAsync(SelectedCarrier);
                UpdateCollection(Services, services.Select(s => s.Name));
            }
        }
        catch (Exception ex)
        {
            ShowError("Failed to load services for carrier", ex);
        }
    }

    private async Task CreateShipmentAsync()
    {
        try
        {
            var shipment = await _easyPostService.CreateShipment(Parcel, SelectedCarrier, SelectedService, SelectedAddress, SelectedFromAddress);
            ShipmentId = shipment.Id;
            UpdateRates(shipment.Rates);
        }
        catch (Exception ex)
        {
            ShowError("Failed to create shipment", ex);
        }
    }

    private void UpdateRates(List<Rate> rates)
    {
        Rates.Clear();

        if (rates != null)
        {
            var filteredRates = rates.Where(rate =>
              (string.IsNullOrEmpty(SelectedCarrier) || rate.Carrier == SelectedCarrier) &&
              (string.IsNullOrEmpty(SelectedService) || rate.Service == SelectedService)).ToList();

            UpdateCollection(Rates, filteredRates);
        }
    }

    private async Task UpdateRatesAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(ShipmentId))
            {
                var shipment = await _easyPostService.GetShipmentById(ShipmentId);
                UpdateRates(shipment.Rates);
            }
        }
        catch (Exception ex)
        {
            ShowError("Failed to update rates", ex);
        }
    }

    private async Task BuyLabelAsync()
    {
        try
        {
            var selectedRate = Rates.FirstOrDefault();

            if (selectedRate == null)
            {
                MessageBox.Show("No rate selected for label purchase.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var shipment = await _easyPostService.BuyShipment(ShipmentId, selectedRate.Id);
            var parcel = shipment.Parcel;

            var shipmentModel = new ShipmentModel
            {
                TrackingNumber = shipment.TrackingCode,
                Carrier = selectedRate.Carrier,
                Service = selectedRate.Service,
                ToAddress = shipment.ToAddress.Id,
                FromAddress = shipment.FromAddress.Id,
                ShipmentId = shipment.Id,
                PostageLabelUrl = shipment.PostageLabel.LabelUrl,
                Parcel = new ParcelModel
                {
                    Height = parcel.Height,
                    Weight = parcel.Weight,
                    Length = parcel.Length,
                    Width = parcel.Width,
                    ParcelId = parcel.Id,
                }
            };

            await _shipmentService.AddAsync(shipmentModel);

            MessageBox.Show($"Label created successfully. Tracking Number: {shipment.TrackingCode}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            OpenTrackingWindow(shipment.TrackingCode, shipment.PostageLabel.LabelUrl);
        }
        catch (Exception ex)
        {
            ShowError("Failed to buy label", ex);
        }
    }

    private void OpenTrackingWindow(string trackingNumber, string postageLabelUrl)
    {
        var trackingWindow = new TrackingWindow();
        var trackingViewModel = new TrackingViewModel(_easyPostService)
        {
            TrackingCode = trackingNumber,
            PostageLabelUrl = postageLabelUrl,
        };
        trackingWindow.DataContext = trackingViewModel;
        trackingWindow.Show();
    }

    private void Cancel() => CloseWindow(false);

    private void CloseWindow(bool result)
    {
        if (Application.Current.Windows.OfType<ShipmentWindow>().FirstOrDefault() is ShipmentWindow window)
        {
            window.DialogResult = result;
        }
    }

    private static void UpdateCollection<T>(ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    private static void ShowError(string message, Exception ex)
    {
        MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
