namespace RepositoryLayer.Models;

public class DbContact
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public DateOnly? BirthDate { get; set; } = null!;
    public ICollection<DbEmail> Emails { get; set; } = null!;
}
