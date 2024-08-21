using ContactsApp.Services.DTOs;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            if (!string.IsNullOrWhiteSpace(TrackingCode))
            {
                Tracker = await _easyPostService.GetTrackerDataAsync(TrackingCode);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
