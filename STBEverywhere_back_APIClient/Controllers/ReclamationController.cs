using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.enums;
using STBEverywhere_ApiAuth.Repositories;
using System.Net;
using STBEverywhere_Back_SharedModels.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
namespace STBEverywhere_back_APIClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  
    public class ReclamationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ReclamationController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReclamationController(
            ApplicationDbContext dbContext,
            IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
            ILogger<ReclamationController> logger)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        
        [HttpPost("effectuer-reclamation")]
       
        public async Task<IActionResult> CreateReclamation([FromBody] ReclamationDto reclamationDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                var reclamation = new Reclamation
                {
                    ClientId = client.Id,
                    Objet = reclamationDto.Objet,
                    Description = reclamationDto.Description,
                    Statut = ReclamationStatut.Nouvelle,
                    DateCreation = DateTime.UtcNow
                };

                _dbContext.Reclamations.Add(reclamation);
                await _dbContext.SaveChangesAsync();

                var reference = $"REC-{reclamation.Id.ToString().PadLeft(8, '0')}";

                return Ok(new ReclamationResponseDto
                {
                    Id = reclamation.Id,
                    Objet = reclamation.Objet,
                    Description = reclamation.Description,
                    DateCreation = reclamation.DateCreation,
                    Statut = reclamation.Statut.ToString(),
                    Reference = reference
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la réclamation");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur est survenue");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReclamations()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                var reclamations = await _dbContext.Reclamations
                    .Where(r => r.ClientId == client.Id)
                    .OrderByDescending(r => r.DateCreation)
                    .Select(r => new ReclamationResponseDto
                    {
                        Id = r.Id,
                        Objet = r.Objet,
                        Description = r.Description,
                        DateCreation = r.DateCreation,
                        DateResolution = r.DateResolution,
                        Statut = r.Statut.ToString(),
                        Reference = $"REC-{r.Id.ToString().PadLeft(8, '0')}"
                    })
                    .ToListAsync();

                return Ok(reclamations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réclamations");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur est survenue");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReclamationDetails(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                var reclamation = await _dbContext.Reclamations
                    .FirstOrDefaultAsync(r => r.Id == id && r.ClientId == client.Id);

                if (reclamation == null)
                    return NotFound();

                return Ok(new ReclamationResponseDto
                {
                    Id = reclamation.Id,
                    Objet = reclamation.Objet,
                    Description = reclamation.Description,
                    DateCreation = reclamation.DateCreation,
                    DateResolution = reclamation.DateResolution,
                    Statut = reclamation.Statut.ToString(),
                    Reference = $"REC-{reclamation.Id.ToString().PadLeft(8, '0')}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la réclamation");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Une erreur est survenue");
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
    }
}
