using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APIClient.Services;
using PdfSharpCore;
using System.IdentityModel.Tokens.Jwt; // Pour JwtRegisteredClaimNames
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.IO;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using System.Security.Claims;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_ApiAuth.Repositories;

namespace STBEverywhere_back_APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Applique l'authentification à toutes les méthodes du contrôleur
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;


        public ClientController(IClientService clientService, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Tentative d'enregistrement client pour {Email}", registerDto.Email);
                var result = await _clientService.RegisterAsync(registerDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur d'enregistrement client");
                return BadRequest(new { Message = ex.Message });
            }
        }


        /*
        [HttpGet("GetBeneficiairesByClientId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBeneficiairesByClientId()
        {
            // 1. Extraire le ClientId du token JWT
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientIdClaim))
            {
                return Unauthorized("ClientId non trouvé dans le token.");
            }

            if (!int.TryParse(clientIdClaim, out int clientId))
            {
                return Unauthorized("ClientId invalide dans le token.");
            }

            // 2. Récupérer la liste des bénéficiaires pour ce ClientId
            var beneficiaires = await _context.Beneficiaires
                .Where(b => b.ClientId == clientId)
                .ToListAsync();

            // 3. Vérifier si des bénéficiaires ont été trouvés
            if (beneficiaires == null || !beneficiaires.Any())
            {
                return NotFound("Aucun bénéficiaire trouvé pour ce client.");
            }

            // 4. Retourner la liste des bénéficiaires
            return Ok(beneficiaires);
        }
        
        */


        // Récupérer les informations du client
        [HttpGet("me")]
        public async Task<IActionResult> GetClientInfo()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);
         
                return Ok(client);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Erreur d'authentification");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur serveur");
                return StatusCode(500, new { message = "Erreur interne" });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateClientInfo([FromBody] Client updatedClient)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);
                updatedClient.Id = client.Id; // Ceci est crucial

                bool isUpdated = await _clientService.UpdateClientInfoAsync(updatedClient.Id, updatedClient);

                if (!isUpdated)
                {
                    return NotFound(new { message = "Client non trouvé" });
                }

                return Ok(new { message = "Informations mises à jour avec succès !" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour");
                return StatusCode(500, new { message = "Erreur interne" });
            }
        }

        // Télécharger le fichier KYC
        [HttpGet("kyc/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadKYC()
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            var pdfBytes = GenerateKYCReport(client);
            return File(pdfBytes, "application/pdf", $"Fiche_KYC_{client.Nom}_{client.Prenom}.pdf");
        }

        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = GetUserIdFromToken();
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier sélectionné.");
            }

            // Générer un nom de fichier unique
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot/Images", fileName);

            // Enregistrer le fichier sur le serveur
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Mettre à jour le nom de la photo dans la base de données
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            client.PhotoClient = fileName;
            await _context.SaveChangesAsync();

            return Ok(new { fileName });
        }

        // Générer un rapport KYC au format PDF
        private byte[] GenerateKYCReport(Client client)
        {
            var pdf = PdfGenerator.GeneratePdf(GenerateKycHtml(client), (PdfSharp.PageSize)PageSize.A4);
            using (var stream = new MemoryStream())
            {
                pdf.Save(stream, false);
                return stream.ToArray();
            }
        }

        // Générer le HTML pour le rapport KYC
        private string GenerateKycHtml(Client client)
        {
            return $@"
        <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 20px; }}
                    h1 {{ color: #2c3e50; }}
                    .info {{ margin-bottom: 15px; }}
                    .label {{ font-weight: bold; color: #34495e; }}
                    .section {{ margin-bottom: 30px; border-bottom: 1px solid #ddd; padding-bottom: 20px; }}
                    .section h2 {{ color: #2980b9; margin-bottom: 10px; }}
                    .photo-container {{ float: right; margin-left: 20px; margin-bottom: 20px; }}
                    .photo-container img {{ width: 100px; height: auto; border-radius: 5px; border: 1px solid #ddd; }}
                </style>
            </head>
            <body>
                <h1>Fiche KYC - {client.Nom} {client.Prenom}</h1>

                <!-- Informations personnelles -->
                <div class='section'>
                    <h2>Informations personnelles</h2>
                    <div class='info'>
                        <span class='label'>Nom:</span> {client.Nom}
                    </div>
                    <div class='info'>
                        <span class='label'>Prénom:</span> {client.Prenom}
                    </div>
                    <div class='info'>
                        <span class='label'>Date de naissance:</span> {client.DateNaissance.ToShortDateString()}
                    </div>
                    <div class='info'>
                        <span class='label'>Genre:</span> {client.Genre}
                    </div>
                    <div class='info'>
                        <span class='label'>Téléphone:</span> {client.Telephone}
                    </div>
                    <div class='info'>
                        <span class='label'>Email:</span> {client.Email}
                    </div>
                    <div class='info'>
                        <span class='label'>Adresse:</span> {client.Adresse}
                    </div>
                    <div class='info'>
                        <span class='label'>Civilité:</span> {client.Civilite}
                    </div>
                    <div class='info'>
                        <span class='label'>Nationalité:</span> {client.Nationalite}
                    </div>
                    <div class='info'>
                        <span class='label'>État civil:</span> {client.EtatCivil}
                    </div>
                    <div class='info'>
                        <span class='label'>Résidence:</span> {client.Residence}
                    </div>
                    <div class='info'>
                        <span class='label'>Pays de naissance:</span> {client.PaysNaissance ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Nom de la mère:</span> {client.NomMere ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Nom du père:</span> {client.NomPere ?? "Non renseigné"}
                    </div>
                </div>

                <!-- Informations d'identification -->
                <div class='section'>
                    <h2>Informations d'identification</h2>
                    <div class='info'>
                        <span class='label'>Numéro CIN:</span> {client.NumCIN ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Date de délivrance CIN:</span> {client.DateDelivranceCIN?.ToShortDateString() ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Date d'expiration CIN:</span> {client.DateExpirationCIN?.ToShortDateString() ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Lieu de délivrance CIN:</span> {client.LieuDelivranceCIN ?? "Non renseigné"}
                    </div>
                </div>

                <!-- Informations professionnelles -->
                <div class='section'>
                    <h2>Informations professionnelles</h2>
                    <div class='info'>
                        <span class='label'>Profession:</span> {client.Profession ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Situation professionnelle:</span> {client.SituationProfessionnelle ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Niveau d'éducation:</span> {client.NiveauEducation ?? "Non renseigné"}
                    </div>
                    <div class='info'>
                        <span class='label'>Revenu mensuel:</span> {client.RevenuMensuel.ToString("C")}
                    </div>
                </div>

                <!-- Informations familiales -->
                <div class='section'>
                    <h2>Informations familiales</h2>
                    <div class='info'>
                        <span class='label'>Nombre d'enfants:</span> {client.NombreEnfants}
                    </div>
                </div>
            </body>
        </html>
    ";
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
        /* private async Task<int> GetClientIdFromToken()
         {
             // Récupérer l'ID de l'utilisateur depuis le token
             var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
             if (identity == null)
                 throw new UnauthorizedAccessException("Identité non trouvée");

             var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
             if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                 throw new UnauthorizedAccessException("Claim d'identifiant utilisateur invalide");

             // Récupérer le client associé à cet utilisateur
             var client = await _userRepository.GetClientByUserIdAsync(userId);
             if (client == null)
                 throw new UnauthorizedAccessException("Client non trouvé pour cet utilisateur");

             return client.Id;
         }*/
    }
}