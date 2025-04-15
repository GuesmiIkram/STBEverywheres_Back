using Microsoft.AspNetCore.Mvc;
using STBEverywhere_back_APIAgent.Service.IService;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;
using System.Security.Claims;
using STBEverywhere_ApiAuth.Repositories;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using STBEverywhere_Back_SharedModels.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace STBEverywhere_back_APIAgent.Controllers
{
    [Route("api/agent/packs")]
    [ApiController]
    public class ReponsePacksController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReponsePacksController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ReponsePacksController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReponsePacksController> logger,
            IUserRepository userRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("student-demands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentDemandsByAgency()
        {
            try
            {
                // Récupération de l'agent et son agence
                var userId = GetUserIdFromToken();
                var agent = await _userRepository.GetAgentByUserIdAsync(userId);
                if (agent == null || string.IsNullOrEmpty(agent.AgenceId))
                {
                    return BadRequest("Agent introuvable ou agence non assignée.");
                }

                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour récupérer les demandes
                var response = await client.GetAsync(
                    $"http://localhost:5260/api/client/student-demands-by-agency/{agent.AgenceId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var demandes = await response.Content.ReadFromJsonAsync<object>();
                return Ok(demandes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes Pack Student");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpGet("elyssa-demands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetElyssaDemandsByAgency()
        {
            try
            {
                // Récupération de l'agent et son agence
                var userId = GetUserIdFromToken();
                var agent = await _userRepository.GetAgentByUserIdAsync(userId);
                if (agent == null || string.IsNullOrEmpty(agent.AgenceId))
                {
                    return BadRequest("Agent introuvable ou agence non assignée.");
                }

                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour récupérer les demandes
                var response = await client.GetAsync(
                    $"http://localhost:5260/api/client/elyssa-demands-by-agency/{agent.AgenceId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var demandes = await response.Content.ReadFromJsonAsync<object>();
                return Ok(demandes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes Pack Elyssa");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpPost("student-demands/{demandId}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptStudentDemand(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour accepter la demande
                var response = await client.PostAsync(
                    $"http://localhost:5260/api/client/send-student-documents-email/{demandId}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'acceptation de la demande Student {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpPost("elyssa-demands/{demandId}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptElyssaDemand(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour accepter la demande
                var response = await client.PostAsync(
                    $"http://localhost:5260/api/client/send-elyssa-documents-email/{demandId}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'acceptation de la demande Elyssa {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        private int GetUserIdFromToken()
        {
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader))
                {
                    throw new UnauthorizedAccessException("Header Authorization manquant");
                }

                var tokenParts = authHeader.Split(' ');
                if (tokenParts.Length != 2 || !tokenParts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Format d'autorisation invalide");
                }

                var token = tokenParts[1].Trim();
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    throw new UnauthorizedAccessException("Le token n'est pas un JWT valide");
                }

                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub ||
                    c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    throw new UnauthorizedAccessException("Claim d'identifiant utilisateur invalide");
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetUserIdFromToken");
                throw new UnauthorizedAccessException("Erreur de traitement du token", ex);
            }
        }




        [HttpPost("student-demands/{demandId}/refuse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefuseStudentDemand(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour refuser la demande
                var response = await client.PostAsync(
                    $"http://localhost:5260/api/client/refuser-Student-documents/{demandId}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du refus de la demande Student {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpPost("elyssa-demands/{demandId}/refuse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefuseElyssaDemand(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour refuser la demande
                var response = await client.PostAsync(
                    $"http://localhost:5260/api/client/refuser-elyssa-documents/{demandId}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du refus de la demande Elyssa {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpGet("student-demands/{demandId}/generate-pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateStudentPdf(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour générer le PDF
                var response = await client.GetAsync(
                    $"http://localhost:5260/api/client/generate-student-pdf/{demandId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"PackStudent_Demande_{demandId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la génération du PDF pour la demande Student {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }

        [HttpGet("elyssa-demands/{demandId}/generate-pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateElyssaPdf(int demandId)
        {
            try
            {
                // Configuration du client HTTP
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer ", ""));

                // Appel à l'API Client pour générer le PDF
                var response = await client.GetAsync(
                    $"http://localhost:5260/api/client/generate-elyssa-pdf/{demandId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erreur API Client: {StatusCode} - {Content}",
                        response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, errorContent);
                }

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                return File(pdfBytes, "application/pdf", $"PackElyssa_Demande_{demandId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la génération du PDF pour la demande Elyssa {demandId}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }
    }
}