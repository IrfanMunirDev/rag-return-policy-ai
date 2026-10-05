using Microsoft.EntityFrameworkCore;
using ReturnPolicy.Infrastructure.Entities;

namespace ReturnPolicy.Infrastructure.Data;

public class PolicyDbContext : DbContext
{
    public DbSet<PolicyChunkEntity> PolicyChunks => Set<PolicyChunkEntity>();

    public DbSet<User> Users => Set<User>();

    public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicyChunkEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.Embedding).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });
    }
}