using Microsoft.EntityFrameworkCore;
using STBEverywhere_ApiAuth.Repositories;
using STBEverywhere_back_APICompte.Repository;
using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
namespace STBEverywhere_back_APICompte.Services
{
    public class CompteService : ICompteService
    {

        private readonly ICompteRepository _compteRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CompteService> _logger;
        private readonly IUserRepository _userRepository;


        
        public CompteService(ICompteRepository compteRepository,IUserRepository userRepository,IHttpClientFactory httpClientFactory, ILogger<CompteService> logger)
        {
            _compteRepository = compteRepository;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory; 
            _logger = logger;
        }

        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, StatutDemandeEnum statut)
        {
            return await _compteRepository.GetDemandesModificationAsync(ribCompte, statut.ToString());
        }
       

        public async Task CreateDemandeModificationAsync(DemandeModificationDecouvert demande)
        {
            await _compteRepository.CreateDemandeModificationAsync(demande);
        }

        /*public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, string statut)
        {
            return await _compteRepository.GetDemandesModificationAsync(ribCompte, statut);
        }*/






        public async Task<Compte> GetByRIBAsync(string rib)
        {
            return await _compteRepository.GetByRibAsync(rib);
        }


        public async Task<string> GetAgenceIdOfCompteAsync(string rib)
        {
            var Compte = await _compteRepository.GetCompteByRIBAsync(rib);

           
           
                var client = await _userRepository.GetClientByUserIdAsync(Compte.ClientId);


            return client.AgenceId ;
        }




        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesByAgenceIdAsync(string agenceId)
        {
            var allComptes = await _compteRepository.GetAllAsync();

            var comptesAvecAgence = new List<Compte>();

            foreach (var compte in allComptes)
            {
                var client = await _userRepository.GetClientByUserIdAsync(compte.ClientId);
                if (client != null && client.AgenceId == agenceId)
                {
                    comptesAvecAgence.Add(compte);
                }
            }

            var demandes = new List<DemandeModificationDecouvert>();

            foreach (var compte in comptesAvecAgence)
            {
                var demandesPourCompte = await _compteRepository.GetDemandesModificationByCompteRibAsync(compte.RIB);
                demandes.AddRange(demandesPourCompte);
            }

            return demandes;
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

        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesByClientIdAsync(int clientId)
        {
            // 1. Récupérer tous les comptes du client
            var comptes = await _compteRepository.GetAllAsync(c => c.ClientId == clientId);

            // 2. Extraire les RIBs de ces comptes
            var ribComptes = comptes.Select(c => c.RIB).ToList();

            // 3. Récupérer toutes les demandes pour ces RIBs
            return await _compteRepository.GetDemandesModificationAsync(ribComptes);
        }



      




        public async Task<string> GenerateUniqueRIB(string agenceid)
        {
            var agenceCode = await GetAgenceCodeAsync(agenceid);

            if (string.IsNullOrEmpty(agenceCode) || agenceCode.Length != 3 || !agenceCode.All(char.IsDigit))
            {
                throw new InvalidOperationException("Code agence invalide.");
            }

            string rib;
            var random = new Random();

            do
            {
                var ribBuilder = new StringBuilder("10"); // les deux premiers chiffres

                ribBuilder.Append(agenceCode); // les 3 suivants = code agence

                // Le reste du rib est géneré aleatoirement
                for (int i = 0; i < 15; i++)
                {
                    ribBuilder.Append(random.Next(0, 10)); // chiffre entre 0 et 9
                }

                rib = ribBuilder.ToString();
            }
            while (await _compteRepository.ExistsByRibAsync(rib));

            return rib;
        }

        private async Task<string> GetAgenceCodeAsync(string agenceid)
        {
            try
            {




                var httpClient = _httpClientFactory.CreateClient("AgenceService");
                var response = await httpClient.GetAsync($"/api/AgenceApi/byId/{agenceid}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erreur récupération agence: {Code}", response.StatusCode);
                    throw new InvalidOperationException("Échec de la récupération de l'agence.");
                }

                var agence = await response.Content.ReadFromJsonAsync<AgenceDto>();
                return agence?.CodeAgence; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du code agence.");
                throw;
            }
        }
    }








}


