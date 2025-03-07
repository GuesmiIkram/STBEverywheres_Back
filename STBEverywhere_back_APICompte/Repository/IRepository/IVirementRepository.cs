
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APICompte.Repository.IRepository
{
    public interface IVirementRepository:IRepository<Virement>
    {

        
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
       
        Task RollbackTransactionAsync();
    }
}
