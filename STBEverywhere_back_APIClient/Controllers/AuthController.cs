using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APIClient.Services;
using System;
using STBEverywhere_back_APICompte.Services;
using STBEverywhere_back_APICompte.Repository.IRepository;

namespace STBEverywhere_back_APIClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IClientService _clientService;
        private readonly ICompteService _compteService;
        private readonly ICompteRepository _compteRepository;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger,
            IClientService clientService,
            ICompteService compteService,
            ICompteRepository compteRepository)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _compteService = compteService ?? throw new ArgumentNullException(nameof(compteService));
            _compteRepository = compteRepository ?? throw new ArgumentNullException(nameof(compteRepository));
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Mauvais identifiants
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Tentative de connexion pour l'email : {Email}", loginDto.Email);

                // Authentifier l'utilisateur et stocker les tokens dans les cookies
                var result = _authService.Authenticate(loginDto.Email, loginDto.Password);

                // Retourner une réponse réussie
                return Ok(new { Message = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Échec de la connexion pour l'email : {Email}. Raison : {Message}", loginDto.Email, ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de la connexion pour l'email : {Email}. Erreur : {Message}", loginDto.Email, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Token invalide ou expiré
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
      
        public IActionResult RefreshToken()
        {
            try
            {
                _logger.LogInformation("Requête de rafraîchissement du token reçue.");
                
                // Rafraîchir les tokens
                var (newAccessToken, newRefreshToken) = _authService.RefreshToken();
                Response.Cookies.Delete("AccessToken");
                Response.Cookies.Delete("RefreshToken");
                _authService.SetTokenCookies(newAccessToken, newRefreshToken);
                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Échec du rafraîchissement du token. Raison : {Message}", ex.Message);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors du rafraîchissement du token. Erreur : {Message}", ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }

        [HttpGet("tokens")]
        public IActionResult GetTokens()
        {
            var accessToken = Request.Cookies["AccessToken"];
            var refreshToken = Request.Cookies["RefreshToken"];

            Console.WriteLine(" Vérification des cookies dans la requête:");
            Console.WriteLine($" AccessToken: {(string.IsNullOrEmpty(accessToken) ? "Non trouvé" : accessToken)}");
            Console.WriteLine($" RefreshToken: {(string.IsNullOrEmpty(refreshToken) ? "Non trouvé" : refreshToken)}");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                Console.WriteLine(" Aucun token trouvé dans les cookies.");
                return Unauthorized(new { message = "Aucun token trouvé." });
            }

            Console.WriteLine("Tokens trouvés, envoi au client.");
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Succès
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Requête invalide
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Compte ou client introuvable
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Erreur serveur
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Vérifier que registerDto n'est pas null
                if (registerDto == null)
                {
                    _logger.LogWarning("RegisterDto est null.");
                    return BadRequest(new { Message = "Les données de la requête sont invalides." });
                }

                _logger.LogInformation("Tentative d'inscription pour le RIB : {RIB}", registerDto.RIB);

                // Vérifier si le RIB existe dans la base de données
                var compte = await _compteRepository.GetByRibAsync(registerDto.RIB);
                if (compte == null)
                {
                    _logger.LogWarning("Compte introuvable pour le RIB : {RIB}", registerDto.RIB);
                    return NotFound(new { Message = "Compte introuvable." });
                }

                // Récupérer l'ID du client associé à ce RIB
                var clientId = await _compteRepository.GetClientIdByRibAsync(registerDto.RIB);
                if (clientId == null)
                {
                    _logger.LogWarning("Client introuvable pour le RIB : {RIB}", registerDto.RIB);
                    return NotFound(new { Message = "Client introuvable pour ce compte." });
                }

                // Vérifier que _clientService est injecté correctement
                if (_clientService == null)
                {
                    _logger.LogError("ClientService n'a pas été injecté correctement.");
                    return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
                }

                // Récupérer le client à partir de l'ID
                var client = await _clientService.GetClientByIdAsync(clientId.Value);
                if (client == null)
                {
                    _logger.LogWarning("Client introuvable pour l'ID : {ClientId}", clientId);
                    return NotFound(new { Message = "Client introuvable." });
                }

                // Vérifier si le client a déjà un login et un mot de passe
                if (!string.IsNullOrEmpty(client.MotDePasse))
                {
                    _logger.LogWarning("Le client a déjà un login et un mot de passe.");
                    return BadRequest(new { Message = "Le client est déjà enregistré avec un login et un mot de passe." });
                }

                // Vérifier que l'email et le mot de passe sont fournis
                if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
                {
                    _logger.LogWarning("L'email ou le mot de passe est manquant.");
                    return BadRequest(new { Message = "L'email et le mot de passe sont requis." });
                }

                // Mettre à jour le client avec le login et le mot de passe
                client.Email = registerDto.Email; // Utiliser l'email comme login
                client.MotDePasse = BCrypt.Net.BCrypt.HashPassword(registerDto.Password); // Hasher le mot de passe

                // Mettre à jour le client dans la base de données
                await _clientService.UpdateClientInfoAsync(client.Id, client);

                _logger.LogInformation("Inscription réussie pour le client : {Email}", registerDto.Email);
                return Ok(new { Message = "Inscription réussie." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Une erreur s'est produite lors de l'inscription pour le RIB : {RIB}. Erreur : {Message}", registerDto?.RIB, ex.Message);
                return StatusCode(500, new { Message = "Une erreur interne s'est produite." });
            }
        }
    }

    }

