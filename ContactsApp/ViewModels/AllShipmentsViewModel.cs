using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels.Commands;
using EasyPost.Models.API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class AllShipmentsViewModel : INotifyPropertyChanged
    {
        private readonly IEasyPostService _easyPostService;
        private readonly IShipmentService _shipmentService;
        private bool _isLoading;

        public ObservableCollection<Shipment> Shipments { get; set; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand SortByPriceCommand { get; }
        public ICommand SortByCreatedAtCommand { get; }
        public ICommand SortByUpdatedAtCommand { get; }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private bool _isPriceSortedAscending = true;
        private bool _isCreatedAtSortedAscending = true;
        private bool _isUpdatedAtSortedAscending = true;

        public AllShipmentsViewModel(IEasyPostService easyPostService, IShipmentService shipmentService)
        {
            _easyPostService = easyPostService;
            _shipmentService = shipmentService;
            Shipments = new ObservableCollection<Shipment>();
            ExportToPdfCommand = new RelayCommand(ExportToPdf);
            ExportToCsvCommand = new RelayCommand(ExportToCsv);
            SortByPriceCommand = new RelayCommand(SortByPrice);
            SortByCreatedAtCommand = new RelayCommand(SortByCreatedAt);
            SortByUpdatedAtCommand = new RelayCommand(SortByUpdatedAt);
            LoadAllShipments();
        }

        private async void LoadAllShipments()
        {
            IsLoading = true;
            try
            {
                var shipments = await _easyPostService.GetAllShipmentsAsync();
                Shipments.Clear();
                foreach (var shipment in shipments)
                {
                    Shipments.Add(shipment);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load shipments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SortByPrice()
        {
            if (_isPriceSortedAscending)
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderBy(s => s.SelectedRate.Price));
            else
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderByDescending(s => s.SelectedRate.Price));

            _isPriceSortedAscending = !_isPriceSortedAscending;
            OnPropertyChanged(nameof(Shipments));
        }

        private void SortByCreatedAt()
        {
            if (_isCreatedAtSortedAscending)
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderBy(s => s.CreatedAt));
            else
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderByDescending(s => s.CreatedAt));

            _isCreatedAtSortedAscending = !_isCreatedAtSortedAscending;
            OnPropertyChanged(nameof(Shipments));
        }

        private void SortByUpdatedAt()
        {
            if (_isUpdatedAtSortedAscending)
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderBy(s => s.UpdatedAt));
            else
                Shipments = new ObservableCollection<Shipment>(Shipments.OrderByDescending(s => s.UpdatedAt));

            _isUpdatedAtSortedAscending = !_isUpdatedAtSortedAscending;
            OnPropertyChanged(nameof(Shipments));
        }

        private void ExportToPdf()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "Shipments.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _shipmentService.ExportToPdf(Shipments, saveFileDialog.FileName);
            }
        }

        private void ExportToCsv()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = "Shipments.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _shipmentService.ExportToCsv(Shipments, saveFileDialog.FileName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
