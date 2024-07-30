using Prism.Mvvm;
using Prism.Commands;
using ContactsApp.Models;
using ContactsApp.Services;
using Prism.Regions;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class AddEditContactVM : BindableBase, INavigationAware
    {
        private readonly IContactService _contactService;
        private readonly IRegionManager _regionManager;

        private Contact _contact;
        public Contact Contact
        {
            get => _contact;
            set => SetProperty(ref _contact, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditContactVM(IContactService contactService, IRegionManager regionManager)
        {
            _contactService = contactService;
            _regionManager = regionManager;

            SaveCommand = new DelegateCommand(OnSave);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        private void OnSave()
        {
            if (Contact.Id == 0)
            {
                _contactService.AddContact(Contact);
            }
            else
            {
                _contactService.UpdateContact(Contact);
            }

            _regionManager.RequestNavigate("ContentRegion", "MainWindow");
        }

        private void OnCancel()
        {
            _regionManager.RequestNavigate("ContentRegion", "MainWindow");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Contact"))
            {
                Contact = navigationContext.Parameters.GetValue<Contact>("Contact");
            }
            else
            {
                Contact = new Contact();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
