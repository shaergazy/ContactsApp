﻿using ContactsApp.Models;
using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

public class ShipmentViewModel : INotifyPropertyChanged
{
    private readonly IEasyPostService _easyPostService;
    private readonly IAddressService _addressService;
    private readonly IShipmentService _shipmentService;

    public ParcelDto Parcel { get; set; }

    public ObservableCollection<string> Carriers { get; set; }
    public string SelectedCarrier { get; set; }

    public ObservableCollection<string> Services { get; set; }
    public string SelectedService { get; set; }

    private ObservableCollection<AddressModel> _addresses;
    public ObservableCollection<AddressModel> Addresses
    {
        get => _addresses;
        set
        {
            _addresses = value;
            OnPropertyChanged(nameof(Addresses));
        }
    }

    private ObservableCollection<AddressModel> _fromAddresses;
    public ObservableCollection<AddressModel> FromAddresses
    {
        get => _fromAddresses;
        set
        {
            _fromAddresses = value;
            OnPropertyChanged(nameof(FromAddresses));
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

    public ICommand CreateLabelCommand { get; }
    public ICommand CancelCommand { get; }

    public ShipmentViewModel(IEasyPostService easyPostService, IAddressService addressService, IShipmentService shipmentService) 
    {
        _easyPostService = easyPostService;
        _addressService = addressService;
        _shipmentService = shipmentService;

        Addresses = new ObservableCollection<AddressModel>();
        FromAddresses = new ObservableCollection<AddressModel>();

        Parcel = new ParcelDto();
        Carriers = new ObservableCollection<string> { "USPS", "FedEx" };
        Services = new ObservableCollection<string> { "Ground", "Express" };

        CreateLabelCommand = new RelayCommand(async () => await CreateLabel());
        CancelCommand = new RelayCommand(Cancel);

        _ = LoadAddresses();
    }

    private async Task LoadAddresses()
    {
        try
        {
            var addresses = await _addressService.GetAll();

            Addresses.Clear();
            FromAddresses.Clear();

            foreach (var address in addresses)
            {
                Addresses.Add(address);
                FromAddresses.Add(address);
            }

            OnPropertyChanged(nameof(Addresses));
            OnPropertyChanged(nameof(FromAddresses));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке адресов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task CreateLabel()
    {
        try
        {
            var shipment = await _easyPostService.CreateShipmentLabel(Parcel, SelectedCarrier, SelectedService, SelectedAddress, SelectedFromAddress);

            var parcel = shipment.Parcel;

            var shipmentModel = new ShipmentModel
            {
                TrackingNumber = shipment.TrackingCode,
                Carrier = SelectedCarrier,
                Service = SelectedService,
                ToAddress = shipment.ToAddress.Id,
                FromAddress = shipment.FromAddress.Id,
                ShipmentId = shipment.Id,
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

            OpenTrackingWindow(shipment.TrackingCode);
            CloseWindow(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось создать лейбл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenTrackingWindow(string trackingNumber)
    {
        var trackingWindow = new TrackingWindow();
        var trackingViewModel = new TrackingViewModel(_easyPostService)
        {
            TrackingCode = trackingNumber
        };
        trackingWindow.DataContext = trackingViewModel;
        trackingWindow.Show();
    }

    private void Cancel()
    {
        CloseWindow(false);
    }

    private void CloseWindow(bool result)
    {
        if (Application.Current.Windows.OfType<ShipmentWindow>().FirstOrDefault() is ShipmentWindow window)
        {
            window.DialogResult = result;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
