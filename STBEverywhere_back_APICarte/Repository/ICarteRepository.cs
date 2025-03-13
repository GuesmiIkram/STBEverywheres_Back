using STBEverywhere_Back_SharedModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STBEverywhere_back_APICarte.Repository
{
    public interface ICarteRepository
    {
        Task<IEnumerable<Carte>> GetCartesByRIBAsync(string rib);
        Task<bool> UpdateCarteAsync(Carte carte);
        Task<bool> CreateDemandeCarteAsync(DemandeCarte demandeCarte);
        Task<IEnumerable<DemandeCarte>> GetDemandesByStatutAsync(string statut);
        Task<bool> CreateCarteAsync(Carte carte);
        Task<bool> PinExistsAsync(String pin);
        Task<bool> CvvExistsAsync(String  cvv);
        Task<Carte> GetCarteByNumCarteAsync(string numCarte);
        Task<IEnumerable<DemandeCarte>> GetDemandesByRIBAsync(string rib);
        Task<DemandeCarte> GetDemandeCarteByIdAsync(int demandeId);
        Task<IEnumerable<DemandeCarte>> GetDemandesByCompteAndNomAndTypeAsync(string numCompte, string nomCarte, string typeCarte);
        Task<bool> SaveChangesAsync();
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<bool> UpdateEmailEnvoyeAsync(int demandeId, bool emailEnvoye);
        Task<bool> UpdateEmailEnvoyeLivreeAsync(int demandeId, bool emailEnvoyeLivree);
    }
}