using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using EasyPost;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class AddEditContactViewModel : INotifyPropertyChanged
    {
        private readonly IContactService _contactService;
        private Contact _contact;
        private readonly Client _client;

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

        public AddEditContactViewModel(IContactService contactService, string apiKey)
        {
            _contactService = contactService;
            Contact = new Contact { Address = new Models.AddressModel() };
            _client = new Client(new ClientConfiguration(apiKey));

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void Save()
        {
            if (await ValidateContact())
            {
                if (_contactService.IsContactUnique(Contact))
                {
                    if (Contact.Id == 0)
                    {
                        _contactService.AddContact(Contact);
                    }
                    else
                    {
                        _contactService.UpdateContact(Contact);
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

        private async Task<bool> ValidateAddress(Models.AddressModel address)
        {

            EasyPost.Parameters.Address.Create parameters = new()
            {
                Street1 = address.Street,
                City = address.City,
                State = address.State,
                Zip = address.Zip,
                Country = address.Country,
            };

            EasyPost.Models.API.Address verifiedAddress = await _client.Address.CreateAndVerify(parameters);
            Contact.Address.AddressId = verifiedAddress.Id;

            Console.WriteLine(JsonConvert.SerializeObject(address, Formatting.Indented));
            return address != null &&
                   !string.IsNullOrWhiteSpace(address.Street) &&
                   !string.IsNullOrWhiteSpace(address.City) &&
                   !string.IsNullOrWhiteSpace(address.State) &&
                   !string.IsNullOrWhiteSpace(address.Zip);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
