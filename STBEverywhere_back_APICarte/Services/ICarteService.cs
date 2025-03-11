using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STBEverywhere_back_APICarte.Services
{
    public interface ICarteService
    {
        Task<IEnumerable<CarteDTO>> GetCartesByRIBAsync(string rib);
        Task<IEnumerable<DemandeCarteDTO>> GetDemandesByRIBAsync(string rib);
       // Task<bool> CreateDemandeCarteAsync(DemandeCarteDTO demandeCarteDTO);
        Task<bool> CreateCarteIfDemandeRecupereeAsync(int demandeId);
        Task<IEnumerable<DemandeCarte>> GetDemandesByStatutAsync(string statut);
        Task<CarteDetails> GetCarteDetailsAsync(string numCarte);
        Task SendEmailAsync(string email, string subject, string message);
        Task<bool> UpdateEmailEnvoyeAsync(int demandeId, bool emailEnvoye);
        Task<bool> UpdateEmailEnvoyeLivreeAsync(int demandeId, bool emailEnvoyeLivree);
        Task UpdateDemandeAsync(DemandeCarte demande);
        Task<string> BlockCarteAsync(string numCarte);
        Task<string> DeBlockCarteAsync(string numCarte);
    }
}