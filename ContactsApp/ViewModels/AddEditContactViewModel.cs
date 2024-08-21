using ContactsApp.Models;
using ContactsApp.Services.Interfaces;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class AddEditContactViewModel : INotifyPropertyChanged
    {
        private readonly IContactService _contactService;
        private readonly IEasyPostService _easyPostService;
        private Contact _contact;

        public Contact Contact
        {
            get => _contact;
            set
            {
                _contact = value;
                OnPropertyChanged(nameof(Contact));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditContactViewModel(IContactService contactService, IEasyPostService easyPostService)
        {
            _contactService = contactService;
            Contact = new Contact { Address = new Models.AddressModel() };
            _easyPostService = easyPostService;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void Save()
        {
            if (await ValidateContact())
            {
                if (_contactService.IsUnique(Contact))
                {
                    if (Contact.Id == 0)
                    {
                        await _contactService.AddAsync(Contact);
                    }
                    else
                    {
                        _contactService.Update(Contact);
                    }
                    MessageBox.Show("Saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseWindow(true);
                }
                else
                    MessageBox.Show("Same contact already exists.", "Already Exist Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Please enter valid data.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool result)
        {
            if (Application.Current.Windows.OfType<AddEditContactWindow>().FirstOrDefault() is AddEditContactWindow window)
            {
                window.DialogResult = result;
            }
        }

        private async Task<bool> ValidateContact()
        {
            return !string.IsNullOrWhiteSpace(Contact.FirstName) &&
                   !string.IsNullOrWhiteSpace(Contact.LastName) &&
                   !string.IsNullOrWhiteSpace(Contact.PhoneNumber) &&
                   _contactService.IsPhoneNumberValid(_contact.PhoneNumber) &&
                   await ValidateAddress(Contact.Address);
        }

        private async Task<bool> ValidateAddress(Models.AddressModel addressModel)
        {
            try
            {
                var address = await _easyPostService.CreateAndVerifyAddress(addressModel);
                Contact.Address.AddressId = address.Id;

                Console.WriteLine(JsonConvert.SerializeObject(address, Formatting.Indented));
                return address != null &&
                       !string.IsNullOrWhiteSpace(addressModel.Street) &&
                       !string.IsNullOrWhiteSpace(addressModel.City) &&
                       !string.IsNullOrWhiteSpace(addressModel.State) &&
                       !string.IsNullOrWhiteSpace(addressModel.Zip);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
