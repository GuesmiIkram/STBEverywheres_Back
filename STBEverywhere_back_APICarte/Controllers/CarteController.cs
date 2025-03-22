using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APICarte.Services;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_back_APICarte.Repository;
using STBEverywhere_Back_SharedModels.Models.enums;

namespace STBEverywhere_back_APICarte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarteController : ControllerBase

    {
        private readonly ICarteService _carteService;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CarteService> _logger;

        private readonly ICarteRepository _carteRepository;


        public CarteController(ICarteService carteService, ICarteRepository carteRepository, HttpClient httpClient, ILogger<CarteService> logger, ApplicationDbContext dbContext)
        {
            _carteService = carteService;
            _httpClient = httpClient;
            _logger = logger;
            _dbContext = dbContext;
            _carteRepository = carteRepository;
        }
        //API pour recuperer les Carte par RIB Compte se sont les Cartes prepayee
        [HttpGet("rib/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CarteDTO>>> GetCartesByRIB(string rib)
        {

            
           
      
           var cartes = await _carteService.GetCartesByRIBAsync(rib);
           return Ok(cartes);

            

        }
        [HttpPost("demande")]
      
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateDemandeCarte(DemandeCarteDTO demandeCarteDTO)
        {
            // Récupérer le ClientId depuis le token
            var clientId = GetClientIdFromToken();
           

            try
            {
                _logger.LogInformation("Création d'une nouvelle demande de carte pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);

                // Appeler l'API du service de compte pour vérifier si le RIB existe
                var response = await _httpClient.GetAsync($"http://localhost:5185/api/CompteApi/GetByRIB/{demandeCarteDTO.NumCompte}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("RIB invalide : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest(new { success = false, message = "RIB invalide." });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Réponse de l'API Compte : {JsonResponse}", jsonResponse);

                // Désérialiser la réponse en une liste de Compte
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var comptes = JsonSerializer.Deserialize<List<Compte>>(jsonResponse, options);
                if (comptes == null || !comptes.Any())
                {
                    _logger.LogWarning("Compte introuvable : {NumCompte}", demandeCarteDTO.NumCompte);
                    return NotFound(new { success = false, message = "Le compte associé n'existe pas." });
                }

                // Extraire le premier compte de la liste
                var compte = comptes.First();

                // Condition : Si le compte est de type "Épargne", la carte doit être de type "Épargne"
                if (compte.Type == "Epargne" && demandeCarteDTO.NomCarte != NomCarte.Épargne)
                {
                    _logger.LogWarning("Tentative de création d'une carte non Épargne pour un compte Épargne : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest("Un compte Épargne ne peut avoir qu'une  carte Épargne.");
                }

                // Vérifier si la demande est pour une carte Épargne
                if (demandeCarteDTO.NomCarte == NomCarte.Épargne)
                {
                    // Vérifier que le type de compte est "Épargne"
                    if (compte.Type != "Epargne")
                    {
                        _logger.LogWarning("Tentative de création d'une carte Épargne pour un compte non Épargne : {NumCompte}", demandeCarteDTO.NumCompte);
                        return BadRequest("Une carte Épargne ne peut être demandée que pour un compte de type Épargne.");
                    }
                }

                var cartes = await _carteRepository.GetCartesByRIBAsync(demandeCarteDTO.NumCompte);

                // Condition 1: Maximum 2 cartes internationales par compte
                if (demandeCarteDTO.TypeCarte == TypeCarte.International && cartes.Count(c => c.TypeCarte == TypeCarte.International) >= 2)
                {
                    _logger.LogWarning("Tentative de création d'une troisième carte internationale pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest("Un compte ne peut avoir que 2 cartes internationales.");
                }

                // Condition 2: Maximum 2 cartes nationales par compte
                if (demandeCarteDTO.TypeCarte == TypeCarte.National && cartes.Count(c => c.TypeCarte == TypeCarte.National) >= 2)
                {
                    _logger.LogWarning("Tentative de création d'une troisième carte nationale pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest("Un compte ne peut avoir que 2 cartes nationales.");
                }

                // Condition 3: Une seule carte épargne par compte
                if (demandeCarteDTO.NomCarte == NomCarte.Épargne && cartes.Any(c => c.NomCarte == NomCarte.Épargne))
                {
                    _logger.LogWarning("Tentative de création d'une deuxième carte épargne pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest("Un compte ne peut avoir qu'une seule carte épargne.");
                }

                // Condition 4: Pas de demande en cours avec le même nom et type de carte
                var demandesExistantes = await _carteRepository.GetDemandesByCompteAndNomAndTypeAsync(
                    demandeCarteDTO.NumCompte,
                    demandeCarteDTO.NomCarte,
                    demandeCarteDTO.TypeCarte
                );

                if (demandesExistantes.Any(d => d.Statut != StatutDemande.Recuperee))
                {
                    _logger.LogWarning("Une demande existe déjà avec le même nom et type de carte pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                    return BadRequest("Une demande est déjà en cours avec le même nom et type de carte.");
                }

                // Créer la demande de carte avec le statut "En cours de préparation"
                var demandeCarte = new DemandeCarte
                {
                    NumCompte = demandeCarteDTO.NumCompte,
                    NomCarte = demandeCarteDTO.NomCarte,
                    TypeCarte = demandeCarteDTO.TypeCarte,
                    CIN = demandeCarteDTO.CIN,
                    Email = demandeCarteDTO.Email,
                    NumTel = demandeCarteDTO.NumTel,
                    DateCreation = DateTime.Now, // Date de création définie sur maintenant
                    Statut = StatutDemande.EnPreparation, // Statut initial
                    ClientId = clientId // Utiliser le ClientId récupéré du token
                };

                // Enregistrer la demande de carte// Enregistrer la demande de carte
                _dbContext.DemandesCarte.Add(demandeCarte);
                await _dbContext.SaveChangesAsync();

                // Retourner une réponse JSON structurée
                return Ok(new { success = true, message = "Demande de carte créée avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la demande de carte.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Une erreur interne est survenue." });
            }
        }


        [HttpGet("demandes/rib/{rib}")]
       

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DemandeCarteDTO>>> GetDemandesByRIB(string rib)
        {
            var clientId = GetClientIdFromToken();
           

            var demandes = await _carteService.GetDemandesByRIBAsync(rib);
            return Ok(demandes);
        }

        private int? GetClientIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var clientIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (clientIdClaim != null)
                {
                    return int.Parse(clientIdClaim.Value);
                }
            }
            return null;
        }

        [HttpGet("details/{numCarte}")]
      

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarteDTO>> GetCarteDetails(string numCarte)
        {
            var clientId = GetClientIdFromToken();
           
            try
            {
                var carteDetails = await _carteService.GetCarteDetailsAsync(numCarte);
                return Ok(carteDetails);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("block/{numCarte}")]
       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> BlockCarte(string numCarte)
        {
            var clientId = GetClientIdFromToken();
          
            try
            {
                var result = await _carteService.BlockCarteAsync(numCarte);
                return Ok(new { message = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }


        [HttpPost("deblock/{numCarte}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeBlockCarte(string numCarte)
        {
            var clientId = GetClientIdFromToken();
            try
            {
                var result = await _carteService.DeBlockCarteAsync(numCarte);
                return Ok(new { message = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("cartes/by-client")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CarteDTO>>> GetCartesByClientId()
        {
            // Récupérer le ClientId depuis le token
            var clientIdFromToken = GetClientIdFromToken();
          

            try
            {
                // Récupérer les cartes associées au client
                var cartes = await _carteService.GetCartesByClientIdAsync(clientIdFromToken.Value);

                // Si aucune carte n'est trouvée, retourner une réponse 404
                if (cartes == null || !cartes.Any())
                {
                    return NotFound(new { message = "Aucune carte trouvée pour ce client." });
                }

                // Retourner les cartes trouvées
                return Ok(cartes);
            }
            catch (Exception ex)
            {
                // Journaliser l'erreur et retourner une réponse 500
                _logger.LogError(ex, "Erreur lors de la récupération des cartes par client ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
            }
        }
    }
}