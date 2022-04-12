using Core.Models;

namespace Core.Interfaces;

public interface IContactBusinessLogic
{
    /// <summary>
    /// Creates a new contact if one with the same name or contained email address does not already exist.
    /// </summary>
    /// <param name="newContact"></param>
    /// <returns>Returns the created contact</returns>
    Task<Tuple<Contact?, ApiErrorResult?>> CreateContact(Contact newContact);

    /// <summary>
    /// Finds and returns the contact with the matching id if found.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns the contact with the same id</returns>
    Task<Tuple<Contact?, ApiErrorResult?>> GetContact(long id);

    /// <summary>
    /// Updates a contact if there is one that exists with the same Contact.Id.
    /// </summary>
    /// <param name="contactUpdate"></param>
    /// <returns>Returns the updated contact object.</returns>
    Task<Tuple<Contact?, ApiErrorResult?>> UpdateContact(Contact contactUpdate);

    /// <summary>
    /// Deletes a contact that has the same id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns true if deleted, false if not deleted.</returns>
    Task<Tuple<bool?, ApiErrorResult?>> DeleteContact(long id);

    /// <summary>
    /// Finds contacts with the same or similar name, and limits them to the start
    /// and end date range provided. If all parameters are null returns a
    /// 400 bad request ApiErrorResult.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="birthDateRangeStart"></param>
    /// <param name="birthDateRangeEnd"></param>
    /// <returns>Returns a list of contacts that match the criteria.</returns>
    Task<Tuple<IEnumerable<Contact>?, ApiErrorResult?>> SearchContacts(string? name, DateOnly? birthDateRangeStart, DateOnly? birthDateRangeEnd);
}
