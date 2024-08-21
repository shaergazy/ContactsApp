using ContactsApp.Models;

namespace ContactsApp.Services.Interfaces
{
    public interface IContactService : IGenericService<Contact, int>
    {
        /// <summary>
        /// Checks if a contact is unique by comparing key fields.
        /// </summary>
        /// <param name="contact">The contact to check for uniqueness.</param>
        /// <returns>True if the contact is unique; otherwise, False.</returns>
        bool IsUnique(Contact contact);

        /// <summary>
        /// Checks if a phone number is valid.
        /// A valid number starts with '0' and consists of 10 digits.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check.</param>
        /// <returns>True if the number is valid; otherwise, False.</returns>
        bool IsPhoneNumberValid(string phoneNumber);

    }

}
