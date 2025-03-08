using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Repository.IRepositoy
{
    public interface IChequierRepository
    {
        Task<List<DemandeChequier>> GetChequiersDisponiblesAsync();
        Task<List<DemandeChequier>> GetDemandesByRibComptes(List<string> ribComptes);
        Task<List<Chequier>> GetChequiersByDemandesIds(List<int> demandesIds);

        Task SaveChangesAsync();
    }
}
