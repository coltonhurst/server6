namespace RepositoryLayer.Models;

public class DbEmail
{
    public long Id { get; set; }
    public bool IsPrimary { get; set; }
    public string Address { get; set; } = default!;

    public long DbContactId { get; set; }
    public DbContact DbContact { get; set; }
}
