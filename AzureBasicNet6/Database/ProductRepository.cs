using AzureBasicNet6.Models;

namespace AzureBasicNet6.Database
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
