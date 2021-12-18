using AzureBlobStorage.Models;

namespace AzureBlobStorage.Database
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext context
        {
            get
            {
                return db as ApplicationDbContext;
            }
        }

        public ProductRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
    }
}
