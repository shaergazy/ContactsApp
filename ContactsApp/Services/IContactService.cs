using ContactsApp.Models;

namespace ContactsApp.Services
{
    public interface IContactService
    {
        /// <summary>
        /// Retrieves all contacts from the database.
        /// </summary>
        /// <returns>An enumerable collection of contacts.</returns>
        IEnumerable<Contact> GetAllContacts();

        /// <summary>
        /// Adds a new contact to the database.
        /// </summary>
        /// <param name="contact">The contact to be added.</param>
        void AddContact(Contact contact);

        /// <summary>
        /// Updates an existing contact in the database.
        /// </summary>
        /// <param name="contact">The contact with updated information.</param>
        void UpdateContact(Contact contact);

        /// <summary>
        /// Deletes a contact from the database based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the contact to be deleted.</param>
        void DeleteContact(int id);

        /// <summary>
        /// Checks if a contact is unique by comparing key fields.
        /// </summary>
        /// <param name="contact">The contact to check for uniqueness.</param>
        /// <returns>True if the contact is unique; otherwise, False.</returns>
        bool IsContactUnique(Contact contact);

        /// <summary>
        /// Checks if a phone number is valid.
        /// A valid number starts with '0' and consists of 10 digits.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check.</param>
        /// <returns>True if the number is valid; otherwise, False.</returns>
        bool IsPhoneNumberValid(string phoneNumber);

    }

}
