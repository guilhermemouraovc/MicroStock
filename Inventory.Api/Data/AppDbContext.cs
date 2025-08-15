using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<Product> Products => Set<Product>();
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var e in ChangeTracker.Entries<Product>())
        {
            if (e.State == EntityState.Modified) e.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
