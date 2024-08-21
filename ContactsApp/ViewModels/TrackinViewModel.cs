using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class TrackingViewModel : INotifyPropertyChanged
    {
        private readonly IEasyPostService _easyPostService;
        private TrackerDto _tracker;

        public string TrackingCode { get; set; }

        public TrackerDto Tracker
        {
            get => _tracker;
            set
            {
                _tracker = value;
                OnPropertyChanged();
            }
        }

        public ICommand GetTrackingInfoCommand { get; }

        public TrackingViewModel(IEasyPostService easyPostService)
        {
            _easyPostService = easyPostService;
            GetTrackingInfoCommand = new RelayCommand(async () => await GetTrackingInfo());
        }

        private async Task GetTrackingInfo()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TrackingCode))
                {
                    Tracker = await _easyPostService.GetTrackerDataAsync(TrackingCode);
                    if (Tracker == null)
                    {
                        MessageBox.Show("Tracking information not found. Please check the tracking number and try again.", "Tracking Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Tracking number cannot be empty. Please enter a valid tracking number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while retrieving tracking information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
