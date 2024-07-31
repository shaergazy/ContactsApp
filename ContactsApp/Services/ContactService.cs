using ContactsApp.Models;

namespace ContactsApp.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactsDbContext _context;

        public ContactService(ContactsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Contact> GetAllContacts()
        {
            return _context.Contacts.ToList();
        }

        public Contact GetContactById(int id)
        {
            var contact = _context.Contacts.Find(id);
            return contact;
        }

        public void AddContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
        }

        public void UpdateContact(Contact contact)
        {
            _context.ChangeTracker.Clear();
            _context.Contacts.Update(contact);
            _context.SaveChanges();
        }

        public void DeleteContact(int id)
        {
            var contact = GetContactById(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }
        }
    }
}
