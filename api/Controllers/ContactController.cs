using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly IContactBusinessLogic _contactBusinessLogicLayer;

    public ContactController(ILogger<ContactController> logger, IContactBusinessLogic contactBusinessLogicLayer)
    {
        _logger = logger;
        _contactBusinessLogicLayer = contactBusinessLogicLayer;
    }

    /// <summary>
    /// Create a new contact.
    /// </summary>
    /// <param name="newContact">Expects Contact object in body.</param>
    /// <returns>Created Contact object.</returns>
    [HttpPost]
    public async Task<ActionResult<ContactContract>> CreateContact([FromBody] ContactContract newContact)
    {
        _logger.LogInformation("POST /Contact");

        if (newContact == null)
            return BadRequest();

        try
        {
            // Convert the contact contract
            Tuple<Contact?, ApiErrorResult?> contact = ConvertFromContactContract(newContact);
            if (contact.Item2 != null || contact.Item1 == null)
            {
                Response.StatusCode = contact.Item2.ReturnStatusCode;
                return new JsonResult(contact.Item2);
            }

            // Create the contact
            Tuple<Contact?, ApiErrorResult?> result = await _contactBusinessLogicLayer.CreateContact(contact.Item1);

            // Error handling & returns
            if (result.Item2 != null)
            {
                Response.StatusCode = result.Item2.ReturnStatusCode;
                return new JsonResult(result.Item2);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status201Created;
                return new JsonResult(ConvertToContactContract(result.Item1));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return new JsonResult(new ApiErrorResult());
        }
    }

    /// <summary>
    /// Get a contact by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns contact object if an id matches.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ContactContract>> GetContact(long id)
    {
        _logger.LogInformation($"GET /Contact/{id}");

        if (id <= 0)
            return BadRequest();

        try
        {
            Tuple<Contact?, ApiErrorResult?> result = await _contactBusinessLogicLayer.GetContact(id);

            if (result.Item2 != null)
            {
                Response.StatusCode = result.Item2.ReturnStatusCode;
                return new JsonResult(result.Item2);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(ConvertToContactContract(result.Item1));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return new JsonResult(new ApiErrorResult());
        }
    }

    /// <summary>
    /// Update a contact based on id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="contactUpdate"></param>
    /// <returns>Updated Contact object.</returns>
    [HttpPut]
    public async Task<ActionResult<ContactContract>> UpdateContact([FromBody] ContactContract contactUpdate)
    {
        _logger.LogInformation($"UPDATE /Contact");

        if (contactUpdate == null || contactUpdate.Id < 0)
            return BadRequest();

        try
        {
            // Convert the contact contract
            Tuple<Contact?, ApiErrorResult?> contact = ConvertFromContactContract(contactUpdate);
            if (contact.Item2 != null || contact.Item1 == null)
            {
                Response.StatusCode = contact.Item2.ReturnStatusCode;
                return new JsonResult(contact.Item2);
            }

            // Update the contact
            Tuple<Contact?, ApiErrorResult?> result = await _contactBusinessLogicLayer.UpdateContact(contact.Item1);

            // Error handling & returns
            if (result.Item2 != null)
            {
                Response.StatusCode = result.Item2.ReturnStatusCode;
                return new JsonResult(result.Item2);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(ConvertToContactContract(result.Item1));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return new JsonResult(new ApiErrorResult());
        }
    }

    /// <summary>
    /// Delete a contact based on their id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>HTTP Status Code 204 on successful deletion.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteContact(long id)
    {
        _logger.LogInformation($"DELETE /Contact/{id}");

        if (id < 0)
            return BadRequest();

        try
        {
            Tuple<bool?, ApiErrorResult?> result = await _contactBusinessLogicLayer.DeleteContact(id);

            if (result.Item2 != null)
            {
                Response.StatusCode = result.Item2.ReturnStatusCode;
                return new JsonResult(result.Item2);
            }
            else if (result.Item1 == null || result.Item1 == false)
            {
                throw new Exception("DeleteContact failed for unknown reasons.");
            }
            else
            {
                // On a successful delete, return an HTTP status code 204
                // For our use case, this matches RFC 7231, page 29
                // https://www.rfc-editor.org/rfc/rfc7231
                return NoContent();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return new JsonResult(new ApiErrorResult());
        }
    }

    /// <summary>
    /// Search contacts based on name OR by birth date range.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="birthDateRangeStart"></param>
    /// <param name="birthDateRangeEnd"></param>
    /// <returns>Returns a JSON array of contacts that fit the specified criteria.</returns>
    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<ContactContract>>> Search([FromQuery] string? name, [FromQuery] string? birthDateRangeStart, [FromQuery] string? birthDateRangeEnd)
    {
        _logger.LogInformation($"GET /Contact/Search?name={name}&birthDateRangeStart={birthDateRangeStart}&birthDateRangeEnd={birthDateRangeEnd}");

        try
        {
            Tuple<IEnumerable<Contact>?, ApiErrorResult?> result = await _contactBusinessLogicLayer.SearchContacts(name, ConvertStringToDateOnly(birthDateRangeStart), ConvertStringToDateOnly(birthDateRangeEnd));

            if (result.Item2 != null)
            {
                Response.StatusCode = result.Item2.ReturnStatusCode;
                return new JsonResult(result.Item2);
            }
            else
            {
                Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(ConvertToContactContract(result.Item1));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            return new JsonResult(new ApiErrorResult());
        }
    }

    /// <summary>
    /// Converts a ContactContract into a Contact.
    /// We do this because the JsonSerializer does not yet
    /// support DateOnly (https://github.com/dotnet/runtime/issues/53539)
    /// that we use in the Core.Models.Contact model.
    /// Expects ContactContract.BirthDate to be in this form: yyyy-mm-dd,
    /// otherwise it will be 2000-01-01.
    /// </summary>
    /// <param name="newContact"></param>
    /// <returns></returns>
    private Tuple<Contact?, ApiErrorResult?> ConvertFromContactContract(ContactContract contactContract)
    {
        try
        {
            var contact = new Contact
            {
                Id = contactContract.Id,
                Name = contactContract.Name,
                BirthDate = ConvertStringToDateOnly(contactContract.BirthDate),
                Emails = contactContract.Emails
            };

            return new Tuple<Contact?, ApiErrorResult?>(contact, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            var errorResult = new ApiErrorResult
            {
                FriendlyErrorMessage = "Converting the ContactContract to a Contact failed.",
                ReturnStatusCode = 400 // 400 -> bad request
            };

            return new Tuple<Contact?, ApiErrorResult?>(null, errorResult);
        }
    }

    /// <summary>
    /// Converts a Contact into a ContactContract.
    /// We do this because the JsonSerializer does not yet
    /// support DateOnly (https://github.com/dotnet/runtime/issues/53539)
    /// that we use in the Core.Models.Contact model.
    /// </summary>
    /// <param name="newContact"></param>
    /// <returns></returns>
    private ContactContract ConvertToContactContract(Contact contact)
    {
        List<Email> emails = new List<Email>();

        if (contact.Emails != null)
        {
            foreach (Email e in contact.Emails)
            {
                emails.Add(new Email
                {
                    Id = e.Id,
                    IsPrimary = e.IsPrimary,
                    Address = e.Address
                });
            }
        }

        return new ContactContract()
        {
            Id = contact.Id,
            Name = contact.Name,
            BirthDate = contact?.BirthDate?.ToString(),
            Emails = emails
        };
    }

    /// <summary>
    /// Converts a Contact List into a ContactContract List.
    /// We do this because the JsonSerializer does not yet
    /// support DateOnly (https://github.com/dotnet/runtime/issues/53539)
    /// that we use in the Core.Models.Contact model.
    /// </summary>
    /// <param name="newContact"></param>
    /// <returns></returns>
    private List<ContactContract> ConvertToContactContract(IEnumerable<Contact> contacts)
    {
        List<ContactContract> contactContracts = new List<ContactContract>();

        foreach (Contact contact in contacts)
        {
            contactContracts.Add(ConvertToContactContract(contact));
        }

        return contactContracts;
    }

    private DateOnly? ConvertStringToDateOnly(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return null;

        var dateSplit = date.ToLower().Trim().Split('-');

        return new DateOnly(int.Parse(dateSplit[0]), int.Parse(dateSplit[1]), int.Parse(dateSplit[2]));
    }
}
