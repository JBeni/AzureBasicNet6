namespace AzureBasicNet6.Database
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity model);

        IEnumerable<TEntity> GetAll();

        TEntity GetById(object Id);

        void Update(TEntity model);

        void Delete(TEntity model);

        void DeleteById(object Id);
    }
}
