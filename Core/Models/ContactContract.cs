namespace Core.Models;

public class ContactContract
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? BirthDate { get; set; } = null!;
    public ICollection<Email> Emails { get; set; } = null!;
}
