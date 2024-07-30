using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class MainVM : BindableBase
    {
        private readonly IContactService _contactService;
        private readonly IServiceProvider _serviceProvider;
        private Contact _selectedContact;

        public ObservableCollection<Contact> Contacts { get; }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set => SetProperty(ref _selectedContact, value);
        }

        public ICommand AddContactCommand { get; }
        public ICommand EditContactCommand { get; }
        public ICommand DeleteContactCommand { get; }

        public MainVM(IContactService contactService, IServiceProvider serviceProvider)
        {
            _contactService = contactService;
            _serviceProvider = serviceProvider;
            Contacts = new ObservableCollection<Contact>(_contactService.GetAllContacts());

            AddContactCommand = new DelegateCommand(OnAddContact);
            EditContactCommand = new DelegateCommand(OnEditContact, CanEditOrDelete).ObservesProperty(() => SelectedContact);
            DeleteContactCommand = new DelegateCommand(OnDeleteContact, CanEditOrDelete).ObservesProperty(() => SelectedContact);
        }
        private void OnAddContact()
        {
            var newContact = new Contact();
            var viewModel = _serviceProvider.GetRequiredService<AddEditContactVM>();
            viewModel.Contact = newContact;
            var window = new AddEditContactWindow { DataContext = viewModel };
            if (window.ShowDialog() == true)
            {
                Contacts.Add(newContact);
            }
        }

        private void OnEditContact()
        {
            var viewModel = _serviceProvider.GetRequiredService<AddEditContactVM>();
            viewModel.Contact = SelectedContact;
            var window = new AddEditContactWindow { DataContext = viewModel };
            if (window.ShowDialog() == true)
            {
                var index = Contacts.IndexOf(SelectedContact);
                Contacts[index] = SelectedContact;
            }
        }

        private void OnDeleteContact()
        {
            _contactService.DeleteContact(SelectedContact.Id);
            Contacts.Remove(SelectedContact);
        }

        private bool CanEditOrDelete()
        {
            return SelectedContact != null;
        }
    }
}
