using AzureBasicNet6.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureBasicNet6.Database
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
