using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContext;

public class AppDbContext : IdentityDbContext<User, Role, Guid,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReadList> ReadLists { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Photo> Photos { get; set; }
}
