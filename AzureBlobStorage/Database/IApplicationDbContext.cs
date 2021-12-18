using AzureBlobStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureBlobStorage.Database
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
