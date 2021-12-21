namespace AzureBasicNet6.Database
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepo { get; }
        Task<int> SaveChanges();
    }
}
