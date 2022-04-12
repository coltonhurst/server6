using Core.Interfaces;
using Core.Models;

namespace BusinessLogicLayer.ContactBusinessLogic;

public class ContactBusinessLogic : IContactBusinessLogic
{
    private readonly IContactRepository _contactRepositoryLayer;

    public ContactBusinessLogic(IContactRepository contactRepositoryLayer)
    {
        _contactRepositoryLayer = contactRepositoryLayer;
    }

    public async Task<Tuple<Contact?, ApiErrorResult?>> CreateContact(Contact newContact)
    {
        var contacts = await _contactRepositoryLayer.GetAllContacts();
        var conflictingContacts = contacts.Where(c => c.Name == newContact.Name || EmailAddressIntersection(newContact.Emails, c.Emails));

        // Return a HTTP 409 if the email or one of the email addresses already exist
        if (conflictingContacts != null && conflictingContacts.Any())
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "A contact with this name or email address already exists.",
                ReturnStatusCode = 409 // 409 -> conflict
            };

            return new Tuple<Contact?, ApiErrorResult?>(null, errorResult);
        }
        else
        {
            Contact createdContact = await _contactRepositoryLayer.CreateContact(newContact);

            return new Tuple<Contact?, ApiErrorResult?>(createdContact, null);
        }
    }

    public async Task<Tuple<Contact?, ApiErrorResult?>> GetContact(long id)
    {
        var contact = (await _contactRepositoryLayer.GetAllContacts()).Where(c => c.Id == id).FirstOrDefault();

        // Return a 404 if a contact with the specified id was not found
        if (contact == null)
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "A contact with this id was not found.",
                ReturnStatusCode = 404 // 404 -> not found
            };

            return new Tuple<Contact?, ApiErrorResult?>(null, errorResult);
        }
        else
        {
            return new Tuple<Contact?, ApiErrorResult?>(contact, null);
        }
    }

    public async Task<Tuple<Contact?, ApiErrorResult?>> UpdateContact(Contact contactUpdate)
    {
        var contacts = await _contactRepositoryLayer.GetAllContacts();
        var contactFound = contacts.Where(c => c.Id == contactUpdate.Id).FirstOrDefault();

        // Return a HTTP 404 if a contact with the specified id was not found
        if (contactFound == null)
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "A contact with this id was not found.",
                ReturnStatusCode = 404 // 404 -> not found
            };

            return new Tuple<Contact?, ApiErrorResult?>(null, errorResult);
        }
        else
        {
            Contact updatedContact = await _contactRepositoryLayer.UpdateContact(contactUpdate);

            return new Tuple<Contact?, ApiErrorResult?>(updatedContact, null);
        }
    }

    public async Task<Tuple<bool?, ApiErrorResult?>> DeleteContact(long id)
    {
        var contacts = await _contactRepositoryLayer.GetAllContacts();
        var contactFound = contacts.Where(c => c.Id == id).FirstOrDefault();

        // Return a HTTP 404 if a contact with the specified id was not found
        if (contactFound == null)
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "A contact with this id was not found.",
                ReturnStatusCode = 404 // 404 -> not found
            };

            return new Tuple<bool?, ApiErrorResult?>(null, errorResult);
        }
        else
        {
            var deletedContact = await _contactRepositoryLayer.DeleteContact(id);

            return new Tuple<bool?, ApiErrorResult?>(deletedContact, null);
        }
    }

    public async Task<Tuple<IEnumerable<Contact>?, ApiErrorResult?>> SearchContacts(string? name, DateOnly? birthDateRangeStart, DateOnly? birthDateRangeEnd)
    {
        // Return 400 if name and the birth date range are all null
        if (name == null && birthDateRangeStart == null && birthDateRangeEnd == null)
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "Please specify the name or date range search parameter.",
                ReturnStatusCode = 400 // 400 -> bad request
            };

            return new Tuple<IEnumerable<Contact>?, ApiErrorResult?>(null, errorResult);
        }

        var contacts = await _contactRepositoryLayer.GetAllContacts();

        // Get contacts that have a birthday within the range
        if (birthDateRangeStart != null && birthDateRangeEnd != null)
        {
            contacts = contacts.Where(c => c.BirthDate >= birthDateRangeStart && c.BirthDate <= birthDateRangeEnd);
        }

        // Get contacts that have the same name OR if the name to search by
        // is contained in existing contacts
        // Note - search case insensitive
        if (name != null)
        {
            contacts = contacts.Where(c =>
                (c.Name.ToLower().Trim() == name.ToLower().Trim()) ||
                (c.Name.ToLower().Trim().Contains(name.ToLower().Trim())));
        }

        // Return a HTTP 404 if contacts with the above criteria were not found
        if (contacts.Count() == 0)
        {
            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "No contacts were found.",
                ReturnStatusCode = 404 // 404 -> not found
            };

            return new Tuple<IEnumerable<Contact>?, ApiErrorResult?>(null, errorResult);
        }
        else
        {
            return new Tuple<IEnumerable<Contact>?, ApiErrorResult?>(contacts, null);
        }
    }

    /// <summary>
    /// This function takes two lists of Email objects and returns true if
    /// an emailAddress from the Email objects in listA is found in any
    /// of the Email objects in listB.
    /// </summary>
    /// <param name="listA"></param>
    /// <param name="listB"></param>
    /// <returns>True if a matching email address is found, false if not.</returns>
    private bool EmailAddressIntersection(IEnumerable<Email> listA, IEnumerable<Email> listB)
    {
        if (listA == null || listB == null)
            return false;

        var emailAddressesA = listA.Select(a => a.Address);
        var emailAddressesB = listB.Select(a => a.Address);

        foreach (string emailAddress in emailAddressesA)
        {
            if (emailAddressesB.Contains(emailAddress))
                return true;
        }

        return false;
    }
}