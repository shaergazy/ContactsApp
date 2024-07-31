using ContactsApp.Commands;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Views;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class AddEditContactViewModel : INotifyPropertyChanged
    {
        private readonly ContactService _contactService;
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

        public AddEditContactViewModel(ContactService contactService)
        {
            _contactService = contactService;
            Contact = new Contact();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save()
        {
            if (ValidateContact())
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
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        private bool ValidateContact()
        {
            return !string.IsNullOrWhiteSpace(Contact.FirstName) &&
                   !string.IsNullOrWhiteSpace(Contact.LastName) &&
                   !string.IsNullOrWhiteSpace(Contact.PhoneNumber) &&
                   !Regex.IsMatch(Contact.PhoneNumber, @"^\d{10}$") &&
                   !string.IsNullOrWhiteSpace(Contact.Address);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
