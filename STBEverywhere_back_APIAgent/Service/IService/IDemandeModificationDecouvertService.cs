using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Service.IService
{
    public interface IDemandeModificationDecouvertService
    {
        Task<DemandeModificationDecouvert?> GetDemandeByIdAsync(int id);
        Task RepondreDemandeAsync(DemandeModificationDecouvert demande, bool accepte, string? motifRefus, int idAgent);
        Task<List<DemandeModificationDecouvert>> GetDemandesParStatutAsync(string statut);

    }
}
