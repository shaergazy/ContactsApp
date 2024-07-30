using ContactsApp.Models;

namespace ContactsApp.Services
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAllContacts();
        void AddContact(Contact contact);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
    }

}
