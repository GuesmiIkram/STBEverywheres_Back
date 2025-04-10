using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Repository.IRepository
{
    public interface IDemandeModificationDecouvertRepository
    {
        Task<DemandeModificationDecouvert?> GetByIdAsync(int id);
        Task UpdateAsync(DemandeModificationDecouvert demande);
        Task<List<DemandeModificationDecouvert>> GetByStatutAsync(string statut);

    }
}
