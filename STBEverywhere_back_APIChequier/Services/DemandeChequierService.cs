using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_ApiAuth.Repositories;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using System.Text.Json;

namespace STBEverywhere_back_APIChequier.Services
{
    public class DemandeChequierService
    {

        private readonly IDemandesChequiersRepository _DemandeChequierRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DemandeChequierService> _logger;
        private readonly IUserRepository _userRepository;



        public DemandeChequierService(IDemandesChequiersRepository DemandeChequierRepository, IUserRepository userRepository, IHttpClientFactory httpClientFactory, ILogger<DemandeChequierService> logger)
        {
            _DemandeChequierRepository = DemandeChequierRepository;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }




        public async Task<IEnumerable<DemandeChequier>> GetDemandesChequierByAgenceIdAsync(string agenceId)
        {
            try
            {
                using var httpClient = new HttpClient();
                _logger.LogInformation("Appel du service Compte pour récupérer tous les comptes...");


                var apiUrl = "http://localhost:5185/api/compte/allComptes";
                


                var response = await httpClient.GetAsync(apiUrl);
                _logger.LogInformation("Réponse du service Compte : {StatusCode}", response.StatusCode);

               


                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Erreur lors de la récupération des comptes depuis le service Compte.");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var allComptes = JsonSerializer.Deserialize<List<Compte>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (allComptes == null)
                {
                    _logger.LogWarning("La liste des comptes est vide ou null après désérialisation.");
                }

                _logger.LogInformation("Agence ID reçue : {AgenceId}", agenceId);
                _logger.LogInformation("Nombre de comptes récupérés : {Count}", allComptes.Count);

                var comptesAvecAgence = new List<Compte>();
                _logger.LogInformation("Récupération des comptes...");
                _logger.LogInformation("Nombre total de comptes : {Count}", allComptes.Count());

                foreach (var compte in allComptes)
                {
                    var client = await _userRepository.GetClientByUserIdAsync(compte.ClientId);
                    if (client != null && client.AgenceId == agenceId)
                    {
                        _logger.LogInformation("Client trouvé: {Id} / Agence: {Agence}", client.Id, client.AgenceId);
                    
                    comptesAvecAgence.Add(compte);
                    }
                }

                var demandes = new List<DemandeChequier>();

                foreach (var compte in comptesAvecAgence)
                {
                    var demandesPourCompte = await _DemandeChequierRepository.GetDemandesByRibCompte(compte.RIB);
                    demandes.AddRange(demandesPourCompte);
                }

                return demandes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetDemandesChequierByAgenceIdAsync");
                // Log ou gestion d'erreur ici
                throw new Exception("Erreur lors de la récupération des demandes par agence : " + ex.Message);
            }
        }















    }
}
