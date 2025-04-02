using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
using System.Linq.Expressions;

namespace STBEverywhere_back_APIClient.Repositories
{
    public interface IBeneficiaireRepository
    {
        Task<Beneficiaire> GetByIdAsync(int id);
        Task<IEnumerable<Beneficiaire>> GetAllAsync();
        Task<IEnumerable<Beneficiaire>> GetAllAsync(Expression<Func<Beneficiaire, bool>> predicate);

        Task CreateAsync(Beneficiaire beneficiaire); 


    }
}
