using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APICompte.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        Task SaveAsync();
        Task CreateAsync(T entity);

    }
}
