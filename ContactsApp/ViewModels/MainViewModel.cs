using ContactsApp.Commands;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ContactService _contactService;

        public ObservableCollection<Contact> Contacts { get; set; }
        public ICommand AddContactCommand { get; set; }
        public ICommand EditContactCommand { get; set; }
        public ICommand DeleteContactCommand { get; set; }
        private Contact _selectedContact;
        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
                ((RelayCommand)EditContactCommand).RaiseCanExecuteChanged();
            }
        }

        public MainWindowViewModel(ContactService contactService)
        {
            _contactService = contactService;
            Contacts = new ObservableCollection<Contact>();
            AddContactCommand = new RelayCommand(async () => await OpenAddContactWindow());
            EditContactCommand = new RelayCommand(async () => await OpenEditContactWindow(), CanEditContact);
            DeleteContactCommand = new RelayCommand(async () => await ConfirmDeleteContact());
            LoadContacts();
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
            var viewModel = new AddEditContactViewModel(_contactService);
            addContactWindow.DataContext = viewModel;

            if (addContactWindow.ShowDialog() == true)
            {
                Contacts.Add(viewModel.Contact);
            }
        }

        private async Task OpenEditContactWindow()
        {
            if (SelectedContact == null) return;

            var editContactWindow = new AddEditContactWindow();
            var viewModel = new AddEditContactViewModel(_contactService)
            {
                Contact = new Contact
                {
                    Id = SelectedContact.Id,
                    FirstName = SelectedContact.FirstName,
                    LastName = SelectedContact.LastName,
                    PhoneNumber = SelectedContact.PhoneNumber,
                    Address = SelectedContact.Address
                }
            };

            editContactWindow.DataContext = viewModel;

            if (editContactWindow.ShowDialog() == true)
            {
                SelectedContact.FirstName = viewModel.Contact.FirstName;
                SelectedContact.LastName = viewModel.Contact.LastName;
                SelectedContact.PhoneNumber = viewModel.Contact.PhoneNumber;
                SelectedContact.Address = viewModel.Contact.Address;

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
