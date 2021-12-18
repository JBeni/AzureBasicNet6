namespace AzureBlobStorage.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }

        private IProductRepository _ProductRepo;
        public IProductRepository ProductRepo
        {
            get
            {
                if (_ProductRepo == null)
                    _ProductRepo = new ProductRepository(_db);

                return _ProductRepo;
            }
        }

        public async Task<int> SaveChanges()
        {
            return await _db.SaveChangesAsync(new CancellationToken());
        }
    }
}
