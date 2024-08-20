using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.ViewModels.Commands;
using ContactsApp.Views;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IContactService _contactService;
        private readonly IShipmentService _shipmentService;
        private readonly IAddressService _addressService;

        public ObservableCollection<Contact> Contacts { get; set; }
        public ICommand AddContactCommand { get; set; }
        public ICommand EditContactCommand { get; set; }
        public ICommand DeleteContactCommand { get; set; }
        public ICommand CreateShipmentCommand { get; set; }
        private Contact _selectedContact;
        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
                ((RelayCommand)EditContactCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteContactCommand).RaiseCanExecuteChanged();
            }
        }

        public MainWindowViewModel(IContactService contactService, IShipmentService shipmentService, IAddressService addressService)
        {
            _contactService = contactService;
            _shipmentService = shipmentService;
            Contacts = new ObservableCollection<Contact>();
            AddContactCommand = new RelayCommand(async () => await OpenAddContactWindow());
            CreateShipmentCommand = new RelayCommand(async () => await OpenShipmentWindow());
            EditContactCommand = new RelayCommand(async () => await OpenEditContactWindow(), CanEditContact);
            DeleteContactCommand = new RelayCommand(async () => await ConfirmDeleteContact(), CanEditContact);
            LoadContacts();
            _addressService = addressService;   
        }

        private void LoadContacts()
        {
            var contacts = _contactService.GetAllContacts();
            Contacts.Clear();
            foreach (var contact in contacts)
            {
                Contacts.Add(contact);
            }
        }

        private async Task ConfirmDeleteContact()
        {
            if (SelectedContact == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete the contact {SelectedContact.FirstName} {SelectedContact.LastName}?",
                                         "Confirmation of deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteContact();
            }
        }

        private async Task DeleteContact()
        {
            _contactService.DeleteContact(SelectedContact.Id);
            Contacts.Remove(SelectedContact);
        }

        private async Task OpenAddContactWindow()
        {
            var addContactWindow = new AddEditContactWindow();
            var viewModel = new AddEditContactViewModel(_contactService, "EZTK3d5abae2f8924511b83857f21407e65f8N1bjRd0s3DVhdhO6kq3Ug");
            addContactWindow.DataContext = viewModel;

            if (addContactWindow.ShowDialog() == true)
            {
                Contacts.Add(viewModel.Contact);
            }
        }

        private async Task OpenShipmentWindow()
        {
            var shipmentWindow = new ShipmentWindow();
            var viewModel = new ShipmentViewModel(_shipmentService, _addressService);
            shipmentWindow.DataContext = viewModel;
            shipmentWindow.ShowDialog();
        }

        private async Task OpenEditContactWindow()
        {
            if (SelectedContact == null) return;

            var editContactWindow = new AddEditContactWindow();
            var viewModel = new AddEditContactViewModel(_contactService, "EZTK3d5abae2f8924511b83857f21407e65f8N1bjRd0s3DVhdhO6kq3Ug")
            {
                Contact = new Contact
                {
                    Id = SelectedContact.Id,
                    FirstName = SelectedContact.FirstName,
                    LastName = SelectedContact.LastName,
                    PhoneNumber = SelectedContact.PhoneNumber,
                    Address = new AddressModel
                    {
                        Id = SelectedContact.Address.Id,
                        Street = SelectedContact.Address.Street,
                        City = SelectedContact.Address.City,
                        State = SelectedContact.Address.State,
                        Zip = SelectedContact.Address.Zip,
                        Country = SelectedContact.Address.Country,
                    }
                }
            };

            editContactWindow.DataContext = viewModel;

            if (editContactWindow.ShowDialog() == true)
            {
                SelectedContact.FirstName = viewModel.Contact.FirstName;
                SelectedContact.LastName = viewModel.Contact.LastName;
                SelectedContact.PhoneNumber = viewModel.Contact.PhoneNumber;
                SelectedContact.Address.Street = viewModel.Contact.Address.Street;
                SelectedContact.Address.City = viewModel.Contact.Address.City;
                SelectedContact.Address.State = viewModel.Contact.Address.State;
                SelectedContact.Address.Zip = viewModel.Contact.Address.Zip;
                SelectedContact.Address.Country = viewModel.Contact.Address.Country;

                await Task.Run(() => _contactService.UpdateContact(SelectedContact));

                LoadContacts();
            }
        }

        private bool CanEditContact()
        {
            return SelectedContact != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
