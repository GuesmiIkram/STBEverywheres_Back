using STBEverywhere_Back_SharedModels;
using System.Linq.Expressions;


namespace STBEverywhere_back_APICompte.Repository.IRepository
{
    public interface ICompteRepository:IRepository<Compte>
    {
        Task <List<Compte>> GetAllAsync (Expression<Func<Compte,bool>>filter=null); 
        //Task CreateAsync(Compte entity );
        //Task UpdateAsync(Compte entity);
        Task<Compte> UpdateAsync(Compte entity);

        Task<Compte> GetByRibAsync(string rib);
        Task<Compte> GetCompteByRIBAsync(string rib);
        Task<int?> GetClientIdByRibAsync(string rib);

        //Task SaveAsync(); 
    }
}
