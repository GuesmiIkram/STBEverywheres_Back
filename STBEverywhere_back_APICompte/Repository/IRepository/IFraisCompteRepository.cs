using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APICompte.Repository.IRepository
{
    public interface IFraisCompteRepository
    {

        Task<IEnumerable<FraisCompte>> GetAllAsync();
        Task<FraisCompte> GetByIdAsync(int id);
        Task CreateAsync(FraisCompte fraisCompte);
        Task UpdateAsync(FraisCompte fraisCompte);
        Task DeleteAsync(int id);
    }
}
