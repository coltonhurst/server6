using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Models;
using RepositoryLayer.Contexts;
using RepositoryLayer.Models;

namespace RepositoryLayer.ContactRepository;

public class ContactRepository : IContactRepository
{
    private readonly InMemoryContext _context;

    public ContactRepository(InMemoryContext context)
    {
        _context = context;
    }

    public async Task<Contact> CreateContact(Contact newContact)
    {
        DbContact contact = MapContact(newContact);

        await _context.AddAsync(contact);
        await _context.SaveChangesAsync();

        return newContact;
    }

    public async Task<IEnumerable<Contact>> GetAllContacts()
    {
        var dbContacts = await _context.Contacts.ToListAsync();
        List<Contact> contacts = new List<Contact>();

        foreach (DbContact dbc in dbContacts)
        {
            contacts.Add(MapContact(dbc));
        }

        return contacts;
    }

    public async Task<Contact> UpdateContact(Contact contactUpdate)
    {
        DbContact contact = MapContact(contactUpdate);

        var contactToUpdate = await _context.Contacts.Where(c => c.Id == contact.Id).FirstOrDefaultAsync();

        if (contactToUpdate == null)
        {
            throw new Exception("Contact not found.");
        }

        contactToUpdate.Name = contactUpdate.Name;
        contactToUpdate.BirthDate = contactUpdate.BirthDate;
        contactToUpdate.Emails = new List<DbEmail>();

        foreach (DbEmail e in contact.Emails)
        {
            contactToUpdate.Emails.Add(e);
        }

        await _context.SaveChangesAsync();

        return MapContact(contact);
    }

    public async Task<bool> DeleteContact(long id)
    {
        var contact = _context.Contacts.Where(c => c.Id == id).FirstOrDefault();

        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    private DbContact MapContact(Contact c)
    {
        List<DbEmail> emails = new List<DbEmail>();

        DbContact mappedContact = new DbContact
        {
            Id = c.Id,
            Name = c.Name,
            BirthDate = c.BirthDate,
            Emails = new List<DbEmail>()
        };

        foreach (Email e in c.Emails)
        {
            mappedContact.Emails.Add(new DbEmail
            {
                Id = e.Id,
                IsPrimary = e.IsPrimary,
                Address = e.Address,
                DbContactId = c.Id,
                DbContact = mappedContact
            });
        }

        return mappedContact;
    }

    private Contact MapContact(DbContact c)
    {
        List<Email> emails = new List<Email>();

        Contact mappedContact = new Contact
        {
            Id = c.Id,
            Name = c.Name,
            BirthDate = c.BirthDate,
            Emails = new List<Email>()
        };

        if (c.Emails != null)
        {
            foreach (DbEmail e in c.Emails)
            {
                mappedContact.Emails.Add(new Email
                {
                    Id = e.Id,
                    IsPrimary = e.IsPrimary,
                    Address = e.Address
                });
            }
        }

        return mappedContact;
    }
}
