using STBEverywhere_Back_SharedModels;
using System.Linq.Expressions;

namespace STBEverywhere_back_APICompte.Services
{
    public interface ICompteService
    {
        Task<List<Compte>> GetAllAsync(Expression<Func<Compte, bool>> filter = null);
        Task<Compte> GetByRibAsync(string rib);
        Task<Client> GetClientByRIBAsync(string rib);
        Task<Compte> UpdateAsync(Compte entity);
        Task<Compte> CreateAsync(Compte entity);
        Task SaveAsync();
        string GenerateUniqueRIB();
        
    }
}
