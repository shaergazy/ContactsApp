using ContactsApp.Models;
using ContactsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ContactsApp.Services
{
    public class ContactService : GenericService<Contact, int>, IContactService
    {
        private readonly ContactsDbContext _context;

        public ContactService(ContactsDbContext context) : base(context)
        {
            _context = context;
        }

        public override IEnumerable<Contact> GetAll()
        {
            return _context.Contacts.AsQueryable().Include(x => x.Address).ToList();
        }

        public override async Task DeleteAsync(int id)
        {
            var contact = await _context.Contacts.AsQueryable().Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id);
            _context.Remove(contact);
            _context.SaveChanges();
        }

        public bool IsUnique(Contact contact)
        {
            return !_context.Contacts.Any(x => x.FirstName == contact.FirstName 
                                                   && x.LastName == contact.LastName
                                                   && x.PhoneNumber == contact.PhoneNumber
                                                   && x.Id != contact.Id);
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            string pattern = @"^0\d{9}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
