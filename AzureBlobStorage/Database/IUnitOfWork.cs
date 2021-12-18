namespace AzureBlobStorage.Database
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepo { get; }
        Task<int> SaveChanges();
    }
}
