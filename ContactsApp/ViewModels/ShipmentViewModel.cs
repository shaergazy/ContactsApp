using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class ShipmentViewModel : INotifyPropertyChanged
    {
        private readonly IShipmentService _shipmentService;

        public double PackageWeight { get; set; }
        public double PackageLength { get; set; }
        public double PackageWidth { get; set; }
        public double PackageHeight { get; set; }

        public ObservableCollection<string> Carriers { get; set; }
        public string SelectedCarrier { get; set; }

        public ObservableCollection<string> Services { get; set; }
        public string SelectedService { get; set; }

        public ObservableCollection<AddressModel> Addresses { get; set; }
        public AddressModel SelectedAddress { get; set; }

        public ICommand CreateLabelCommand { get; }
        public ICommand CancelCommand { get; }

        public ShipmentViewModel(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
            Carriers = new ObservableCollection<string> { "USPS", "FedEx" };
            Services = new ObservableCollection<string> { "Ground", "Express" };
            Addresses = new ObservableCollection<AddressModel>(); // Assume this will be filled from a service

            CreateLabelCommand = new RelayCommand(async () => await CreateLabel());
            CancelCommand = new RelayCommand(Cancel);
        }

        private async Task CreateLabel()
        {
            //// Assume there is a method to create shipment label
            //var success = await _shipmentService.CreateShipmentLabel(PackageWeight, PackageLength, PackageWidth, PackageHeight, SelectedCarrier, SelectedService, SelectedAddress);

            //if (success)
            //{
            //    MessageBox.Show("Label created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //    CloseWindow(true);
            //}
            //else
            //{
            //    MessageBox.Show("Failed to create label.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
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
}
