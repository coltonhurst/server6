using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Models;

namespace RepositoryLayer.Contexts;

public class InMemoryContext : DbContext
{
    public DbSet<DbContact> Contacts { get; set; }
    public DbSet<DbEmail> Emails { get; set; }

    public InMemoryContext(DbContextOptions<InMemoryContext> options) : base(options)
    {
    }
}
