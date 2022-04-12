namespace Core.Models;

public class Contact
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly? BirthDate { get; set; } = null!;
    public ICollection<Email> Emails { get; set; } = null!;
}
