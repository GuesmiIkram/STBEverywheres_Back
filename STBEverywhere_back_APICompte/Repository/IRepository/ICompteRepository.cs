using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
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
        Task<decimal> GetSoldeByRIBAsync(string rib);

        //Task SaveAsync(); 

        Task<bool> ExistsByRibAsync(string rib);
        Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationByCompteRibAsync(string rib);


        Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(List<string> ribComptes);
        Task CreateDemandeModificationAsync(DemandeModificationDecouvert demande);
        Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, string statut);





















       


    }
}
