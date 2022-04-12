using Core.Models;

namespace Core.Interfaces;

public interface IContactRepository
{
    Task<Contact> CreateContact(Contact newContact);
    Task<IEnumerable<Contact>> GetAllContacts();
    Task<Contact> UpdateContact(Contact contactUpdate);
    Task<bool> DeleteContact(long id);
}
