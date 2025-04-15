using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APIClient.Services;
using System.IdentityModel.Tokens.Jwt; // Pour JwtRegisteredClaimNames
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
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        public ClientController(
            IClientService clientService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
             IWebHostEnvironment environment,
            ApplicationDbContext context,
            ILogger<ClientController> logger,
            EmailService emailService)
        {
            _clientService = clientService;
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _environment = environment;
        }








        [HttpGet("my-agency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClientAgency(
    [FromServices] IHttpClientFactory httpClientFactory)
        {
            try
            {
                // 1. Authentification
                var userId = GetUserIdFromToken();

                

                // 2. Récupération du client
                var client = await _userRepository.GetClientByUserIdAsync(userId);
                if (client == null || string.IsNullOrEmpty(client.AgenceId))
                {
                    return NotFound(new { Message = "Client ou agence non trouvée" });
                }

                // 3. Appel HTTP au service Agence
                var httpClient = httpClientFactory.CreateClient("AgenceService");
                var response = await httpClient.GetAsync($"/api/AgenceApi/byId/{client.AgenceId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la récupération de l'agence. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { Message = "Erreur lors de la récupération de l'agence" });
                }

                // 4. Traitement de la réponse
                var agence = await response.Content.ReadFromJsonAsync<AgenceDto>();
                return Ok(agence);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'agence du client");
                return StatusCode(500, new { Message = "Erreur interne du serveur" });
            }
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

        [HttpGet("GetClientRevenuMensuel")]
        public async Task<IActionResult> GetClientRevenuMensuel(int userId)
        {
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                return Ok(client);
           
        }


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
        public async Task<IActionResult> UpdateClientInfo([FromBody] UpdateClientDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                // Mapper le DTO vers l'entité Client
                client.Telephone = dto.Telephone;
                client.Email = dto.Email;
                client.Adresse = dto.Adresse;
                client.Civilite = dto.Civilite;
                client.EtatCivil = dto.EtatCivil;
                client.Residence = dto.Residence;
                client.SituationProfessionnelle = dto.SituationProfessionnelle;
                client.NiveauEducation = dto.NiveauEducation;
                client.NombreEnfants = dto.NombreEnfants;
                client.RevenuMensuel = dto.RevenuMensuel;


                bool isUpdated = await _clientService.UpdateClientInfoAsync(client.Id, client);

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
       /* [HttpGet("kyc/download")]
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
       */


        // Générer un rapport KYC au format PDF
       /* private byte[] GenerateKYCReport(Client client)
        {
            var pdf = PdfGenerator.GeneratePdf(GenerateKycHtml(client), (PdfSharp.PageSize)PageSize.A4);
            using (var stream = new MemoryStream())
            {
                pdf.Save(stream, false);
                return stream.ToArray();
            }
        }*/

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

        // ClientController.cs
        [HttpPost("request-password-change-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestPasswordChangeOTP()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé." });
                }

                // Générer un code OTP (6 chiffres, valide 15 minutes)
                var otpCode = new Random().Next(100000, 999999).ToString();
                var otpExpiry = DateTime.UtcNow.AddMinutes(15);

                // Stocker le code OTP dans la base de données
                user.ResetPasswordToken = otpCode;
                user.ResetPasswordTokenExpiry = otpExpiry;
                await _context.SaveChangesAsync();

                // Envoyer le code par email
                var emailSubject = "Code de vérification pour changement de mot de passe";
                var emailBody = $"Votre code de vérification est : {otpCode}\nCe code expirera dans 15 minutes.";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                return Ok(new { message = "Un code de vérification a été envoyé à votre adresse email." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du code OTP");
                return StatusCode(500, new { message = "Une erreur est survenue lors de l'envoi du code de vérification." });
            }
        }

        [HttpPost("change-password-with-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePasswordWithOTP([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                // 1. Récupérer l'utilisateur
                var userId = GetUserIdFromToken();
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return Unauthorized(new { message = "Utilisateur non trouvé." });
                }

                // 2. Vérifier le code OTP
                if (user.ResetPasswordToken != changePasswordDto.OTPCode ||
                    user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Code de vérification invalide ou expiré." });
                }

                // 3. Vérifier le mot de passe actuel
                if (!_userRepository.VerifyPassword(user, changePasswordDto.CurrentPassword))
                {
                    return BadRequest(new { message = "Mot de passe actuel incorrect." });
                }

                // 4. Vérifier que les nouveaux mots de passe correspondent
                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                {
                    return BadRequest(new { message = "Les nouveaux mots de passe ne correspondent pas." });
                }

                // 5. Valider la force du nouveau mot de passe
                if (changePasswordDto.NewPassword.Length < 8)
                {
                    return BadRequest(new { message = "Le mot de passe doit contenir au moins 8 caractères." });
                }

                // 6. Mettre à jour le mot de passe
                _userRepository.UpdatePassword(user, changePasswordDto.NewPassword);

                // Invalider le code OTP après utilisation
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;

                await _context.SaveChangesAsync();

                // Envoyer une confirmation par email
                var emailSubject = "Confirmation de changement de mot de passe";
                var emailBody = "Votre mot de passe a été changé avec succès.";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                return Ok(new { message = "Mot de passe changé avec succès." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Erreur d'authentification");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe");
                return StatusCode(500, new { message = "Une erreur est survenue lors du changement de mot de passe." });
            }
        }

        // ClientController.cs

        [HttpPost("upload-profile-image")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                // 1. Vérifier si un fichier a été envoyé
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Aucun fichier n'a été fourni." });
                }

                // 2. Vérification de la taille du fichier (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "La taille du fichier ne doit pas dépasser 5MB." });
                }

                // 3. Vérification du type de fichier
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Seuls les fichiers JPEG, PNG et GIF sont autorisés." });
                }

                // 4. Récupérer l'ID du client à partir du token
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                if (client == null)
                {
                    return NotFound(new { message = "Client non trouvé." });
                }

                // 5. Créer un nom de fichier unique
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"profile_{client.Id}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                // 6. Créer le dossier s'il n'existe pas
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 7. Sauvegarder le fichier sur le serveur
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 8. Supprimer l'ancienne photo si elle existe
                if (!string.IsNullOrEmpty(client.PhotoClient))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", client.PhotoClient);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 9. Mettre à jour le nom du fichier dans la base de données
                client.PhotoClient = fileName;
                await _context.SaveChangesAsync();

                return Ok(new { fileName });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Erreur d'authentification");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload de l'image");
                return StatusCode(500, new { message = "Une erreur est survenue lors de l'upload de l'image." });
            }
        }

        [HttpDelete("remove-profile-image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> RemoveProfileImage()
        {
            try
            {
                // 1. Récupérer l'ID du client à partir du token
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                if (client == null)
                {
                    return NotFound(new { message = "Client non trouvé." });
                }

                // 2. Vérifier si le client a une photo
                if (string.IsNullOrEmpty(client.PhotoClient))
                {
                    return BadRequest(new { message = "Le client n'a pas de photo de profil." });
                }

                // 3. Supprimer le fichier image
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", client.PhotoClient);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // 4. Mettre à jour la base de données
                client.PhotoClient = null;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Photo de profil supprimée avec succès." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Erreur d'authentification");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'image");
                return StatusCode(500, new { message = "Une erreur est survenue lors de la suppression de l'image." });
            }
        }



        [HttpPost("upload-documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadStudentDocuments(
     [FromForm] StudentPackDto documentsDto,
     [FromServices] EmailService emailService,
     [FromServices] ILogger<ClientController> logger)
        {
            try
            {
                // 1. Récupérer l'ID du client à partir du token
                var userId = GetUserIdFromToken();
                var clientstb = await _userRepository.GetClientByUserIdAsync(userId);
                var clientId = clientstb.Id;

                // Check if client exists
                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    return NotFound("Client not found");
                }

                // Vérifier si le client a déjà une demande en cours
                var existingRequest = await _context.PackStudents
                    .FirstOrDefaultAsync(p => p.ClientId == clientId && (p.Status == "Pending" || p.Status == "Processing"));

                if (existingRequest != null)
                {
                    return BadRequest("Vous avez déjà une demande en cours. Vous ne pouvez pas soumettre une nouvelle demande tant que la précédente n'est pas traitée.");
                }

                // Create client-specific directory
                var clientUploadsPath = Path.Combine(_environment.WebRootPath, "uploads", $"client_{clientId}");
                if (!Directory.Exists(clientUploadsPath))
                {
                    Directory.CreateDirectory(clientUploadsPath);
                }

                // Save files and get file names
                var passportFileName = await SaveFileAndGetName(documentsDto.Document1, clientUploadsPath);
                var inscriptionFileName = await SaveFileAndGetName(documentsDto.Document2, clientUploadsPath);
                var bourseFileName = await SaveFileAndGetName(documentsDto.Document3, clientUploadsPath);
                var domicileTunisieFileName = await SaveFileAndGetName(documentsDto.Document4, clientUploadsPath);

                string? domicileFranceFileName = null;
                if (documentsDto.Document5 != null)
                {
                    domicileFranceFileName = await SaveFileAndGetName(documentsDto.Document5, clientUploadsPath);
                }

                // Create new PackStudent record
                var packStudent = new PackStudent
                {
                    PassportPath = passportFileName,
                    InscriptionPath = inscriptionFileName,
                    BoursePath = bourseFileName,
                    DomicileTunisiePath = domicileTunisieFileName,
                    DomicileFrancePath = domicileFranceFileName,
                    SelectedAgency = documentsDto.Agency,
                    SubmissionDate = DateTime.UtcNow,
                    Status = "Pending",
                    ClientId = clientId
                };

                _context.PackStudents.Add(packStudent);
                await _context.SaveChangesAsync();

                // Préparation des pièces jointes
                var attachments = new List<string>
        {
            Path.Combine(clientUploadsPath, passportFileName),
            Path.Combine(clientUploadsPath, inscriptionFileName),
            Path.Combine(clientUploadsPath, bourseFileName),
            Path.Combine(clientUploadsPath, domicileTunisieFileName)
        };

                if (domicileFranceFileName != null)
                {
                    attachments.Add(Path.Combine(clientUploadsPath, domicileFranceFileName));
                }

                // Envoi de l'email avec pièces jointes
                try
                {
                    var emailSubject = "Nouvelle demande Pack Student";
                    var emailBody = $@"
Un client STB veut s'inscrire au pack student.

Détails de la demande:
- ID Client: {clientId}
- Agence sélectionnée: {documentsDto.Agency}
- Date de soumission: {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")}

Les documents sont joints à cet email.";

                    await emailService.SendEmailWithAttachmentsAsync(
                        "guesmii.ikram@gmail.com",
                        emailSubject,
                        emailBody,
                        attachments);
                }
                catch (Exception emailEx)
                {
                    logger.LogError(emailEx, "Erreur lors de l'envoi de l'email de notification");
                }

                return Ok(new
                {
                    message = "Documents envoyés avec succès!",
                    packStudentId = packStudent.Id
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'envoi des documents");
                return StatusCode(500, $"Erreur lors de l'envoi des documents: {ex.Message}");
            }
        }
        private async Task<string> SaveFileAndGetName(IFormFile file, string uploadsPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // Keep original file name and extension
            var fileName = file.FileName;
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // Return only the file name with extension
        }



        [HttpPost("upload-documents-elyssa")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadElyssaDocuments(
     [FromForm] PackElyssaDto documentsDto,
     [FromServices] EmailService emailService,
     [FromServices] ILogger<ClientController> logger)
        {
            try
            {
                // 1. Récupérer l'ID du client à partir du token
                var userId = GetUserIdFromToken();
                var clientstb = await _userRepository.GetClientByUserIdAsync(userId);
                var clientId = clientstb.Id;

                // Check if client exists
                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    return NotFound("Client not found");
                }

                // Vérifier si le client a déjà une demande en cours
                var existingRequest = await _context.PackElyssa
                    .FirstOrDefaultAsync(p => p.ClientId == clientId && (p.Status == "Pending" || p.Status == "Processing"));

                if (existingRequest != null)
                {
                    return BadRequest("Vous avez déjà une demande en cours. Vous ne pouvez pas soumettre une nouvelle demande tant que la précédente n'est pas traitée.");
                }

                // Create client-specific directory
                var clientUploadsPath = Path.Combine(_environment.WebRootPath, "uploads", $"Pack_Elyssa_client_{clientId}");
                if (!Directory.Exists(clientUploadsPath))
                {
                    Directory.CreateDirectory(clientUploadsPath);
                }


                var passportFileName = await SaveFileAndGetName(documentsDto.Document1, clientUploadsPath);
                var longStayVisaFileName = documentsDto.Document2 != null
                    ? await SaveFileAndGetName(documentsDto.Document2, clientUploadsPath)
                    : null;
                var visaRegistrationFileName = documentsDto.Document3 != null
                    ? await SaveFileAndGetName(documentsDto.Document3, clientUploadsPath)
                    : null;
                var frenchResidenceFileName = await SaveFileAndGetName(documentsDto.Document4, clientUploadsPath);
                var cdiContractFileName = await SaveFileAndGetName(documentsDto.Document5, clientUploadsPath);
                var taxCertificateFileName = await SaveFileAndGetName(documentsDto.Document6, clientUploadsPath);
                // Save files and get file names


                // Create new PackStudent record
                var packElyssa = new PackElyssa
                {
                    PassportPath = passportFileName,
                    LongStayVisaPath = longStayVisaFileName,
                    VisaRegistrationPath = visaRegistrationFileName,
                    FrenchResidenceProofPath = frenchResidenceFileName,
                    CDIContractPath = cdiContractFileName,
                    TaxWithholdingCertificatePath = taxCertificateFileName,
                    SelectedAgency = documentsDto.Agency,
                    SubmissionDate = DateTime.UtcNow,
                    Status = "Pending",
                    ClientId = clientId
                };


                _context.PackElyssa.Add(packElyssa);
                await _context.SaveChangesAsync();

                var attachments = new List<string>
        {
            Path.Combine(clientUploadsPath, passportFileName),
            Path.Combine(clientUploadsPath, frenchResidenceFileName),
            Path.Combine(clientUploadsPath, cdiContractFileName),
            Path.Combine(clientUploadsPath, taxCertificateFileName)
        };

                if (longStayVisaFileName != null)
                    attachments.Add(Path.Combine(clientUploadsPath, longStayVisaFileName));
                if (visaRegistrationFileName != null)
                    attachments.Add(Path.Combine(clientUploadsPath, visaRegistrationFileName));

                // Envoi de l'email avec pièces jointes
                try
                {
                    var emailSubject = "Nouvelle demande Pack Elyssa";
                    var emailBody = $@"
Un client STB veut s'inscrire au pack Elyssa.

Détails de la demande:
- ID Client: {clientId}
- Agence sélectionnée: {documentsDto.Agency}
- Date de soumission: {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")}

Les documents sont joints à cet email.";

                    await emailService.SendEmailWithAttachmentsAsync(
                        "guesmii.ikram@gmail.com",
                        emailSubject,
                        emailBody,
                        attachments);
                }
                catch (Exception emailEx)
                {
                    logger.LogError(emailEx, "Erreur lors de l'envoi de l'email de notification");
                }

                return Ok(new
                {
                    message = "Documents envoyés avec succès!",
                    packElyssaId = packElyssa.Id
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'envoi des documents");
                return StatusCode(500, $"Erreur lors de l'envoi des documents: {ex.Message}");
            }
        }
        private async Task<string> SaveFileAndGetName2(IFormFile file, string uploadsPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // Keep original file name and extension
            var fileName = file.FileName;
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // Return only the file name with extension
        }



        //pack elyssa 


    }
}
