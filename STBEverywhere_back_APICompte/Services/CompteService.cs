using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APICompte.Repository;
using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
using System.Linq.Expressions;
namespace STBEverywhere_back_APICompte.Services
{
    public class CompteService : ICompteService
    {

        private readonly ICompteRepository _compteRepository;

        public CompteService(ICompteRepository compteRepository)
        {
            _compteRepository = compteRepository;
        }



        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesByClientIdAsync(int clientId)
        {
            // Récupérer tous les comptes associés au client
            var comptes = await _compteRepository.GetAllAsync(c => c.ClientId == clientId);

            // Extraire tous les RIB des comptes
            var ribComptes = comptes.Select(c => c.RIB).ToList();

            // Récupérer toutes les demandes associées aux RIB des comptes du client
            return await _compteRepository.GetDemandesModificationAsync(ribComptes);
        }

        public async Task CreateDemandeModificationAsync(DemandeModificationDecouvert demande)
        {
            await _compteRepository.CreateDemandeModificationAsync(demande);
        }

        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, string statut)
        {
            return await _compteRepository.GetDemandesModificationAsync(ribCompte, statut);
        }

        public async Task<Compte> GetByRIBAsync(string rib)
        {
            return await _compteRepository.GetByRibAsync(rib);
        }






        public async Task<List<Compte>> GetAllAsync(Expression<Func<Compte, bool>> filter = null)
        {
            return await _compteRepository.GetAllAsync(filter);
        }

        public async Task<Compte> GetByRibAsync(string rib)
        {
            return await _compteRepository.GetByRibAsync(rib);
        }

        public async Task<Compte> UpdateAsync(Compte entity)
        {
            return await _compteRepository.UpdateAsync(entity);
        }

        public async Task<Compte> CreateAsync(Compte entity)
        {
            await _compteRepository.CreateAsync(entity);
            return entity;
        }
        public async Task<Client> GetClientByRIBAsync(string rib)
        {
            var compte = await _compteRepository.GetCompteByRIBAsync(rib);
            if (compte == null)
                return null;

            return compte.Client; // Retourne le client associé au compte
        }
        public async Task<decimal> GetSoldeByRIBAsync(string rib)
        {
            var compte = await _compteRepository.GetByRibAsync(rib);
            if (compte == null)
            {
                throw new InvalidOperationException("Compte introuvable.");
            }

            return compte.Solde;
        }

        public async Task SaveAsync()
        {
            await _compteRepository.SaveAsync();
        }

        public string GenerateIBANFromRIB(string rib)
        {
            if (string.IsNullOrWhiteSpace(rib) || rib.Length < 20)
            {
                throw new ArgumentException("Le RIB doit contenir au moins 20 caractères numériques.");
            }

            Random random = new Random();
            string randomDigits = random.Next(10, 99).ToString(); // Génère deux chiffres aléatoires

            string iban = $"TN{randomDigits}10{rib.Substring(2, 3)}{rib.Substring(5, 10)}{rib.Substring(18, 2)}";

            return iban;
        }
        



        string ICompteService.GenerateUniqueRIB()
        {
            string guidString = Guid.NewGuid().ToString("N");

            // Extrait uniquement les chiffres
            string ribDigits = string.Concat(guidString.Where(c => char.IsDigit(c)));

            // Vérifie que la chaîne a bien au moins 20 chiffres
            if (ribDigits.Length < 20)
            {
                ribDigits = ribDigits.PadRight(20, '0'); // Complète avec des zéros si nécessaire
            }

            // Retourne les 20 premiers chiffres
            return ribDigits.Substring(0, 20);
        }
    }
}

