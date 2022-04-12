# server6

This is an incomplete submission for the given prompt (below).

Thank you for letting me work on this. I was busy this weekend but did what I could in my spare time. I am confident with extra time this would be fully completed.

Colton

## Supported:

- In memory database
- CRUD operations
- Searching by name / birth date
- Business logic is accessible as it is in a separate project, and utilizes dependency injection via interfaces in the Core project
- The Contact and Email models are used

## Additions

- Utilizes dependency injection
- Swagger integrated (visit https://localhost:7079/swagger/index.html when running with the Api project as the startup project)
- Tuples with Core.Models.ApiErrorResult are used for error checking
- Comments

## Discoveries

- The type DateOnly is not yet supported by the json serializer (https://github.com/dotnet/runtime/issues/53539)

## What would I change / complete given more time?

- Ensure only one email can be set to IsPrimary
- Fix the storage of emails (everything works except when storing the email object)
- Seed the database with test data
- For performance, certain things in the business logic layer could be handled in the repository layer (like when updating a contact, if it does not exist, etc... checking if it exists first is not really needed)
- I used tuples so I could return two things between the layers... some value and then ApiErrorResult. I did it this way for simplicity. Given more time I would move this into a generic class and handle differently.

## Provided Prompt

Task: Create a sample .NET Core or .NET 6 web API application with simple CRUD operations via Entity Framework Core.

Requirements:
 
- Use in-memory database
- Seed the database with some basic sample information
- Controller should have Create, Read, Update, Delete, and Search operations.
- Searching should allow searching by name and birth date range
- Business logic should be accessible to a separate Console application
- When a contact has any emails, one and only one should be set to IsPrimary = true

Use the following model for the record:
 
```cs
public class Contact
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly? BirthDate { get; set; } = null!;
    public ICollection<Email> Emails { get; set; } = null!;
}
```
```cs
public class Email
{
    public long Id { get; set; }
    public bool IsPrimary { get; set; }
    public string Address { get; set; } = default!;
}
```