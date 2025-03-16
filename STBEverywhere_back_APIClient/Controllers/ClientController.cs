using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APIClient.Services;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.IO;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;

namespace STBEverywhere_back_APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ApplicationDbContext _context;

        public ClientController(IClientService clientService, ApplicationDbContext context)
        {
            _clientService = clientService;
            _context = context;
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClientInfo()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var client = await _clientService.GetClientByIdAsync(clientId.Value);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(client);
        }


        [HttpPut("update")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClientInfo([FromBody] Client updatedClient)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            bool isUpdated = await _clientService.UpdateClientInfoAsync(clientId.Value, updatedClient);
            if (!isUpdated)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(new { message = "Informations mises à jour avec succès !" });
        }

        [HttpGet("kyc/download")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadKYC()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var client = await _clientService.GetClientByIdAsync(clientId.Value);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            var pdfBytes = GenerateKYCReport(client);
            return File(pdfBytes, "application/pdf", $"Fiche_KYC_{client.Nom}_{client.Prenom}.pdf");
        }

        private byte[] GenerateKYCReport(Client client)
        {
            var pdf = PdfGenerator.GeneratePdf(GenerateKycHtml(client), (PdfSharp.PageSize)PageSize.A4);
            using (var stream = new MemoryStream())
            {
                pdf.Save(stream, false);
                return stream.ToArray();
            }
        }
        [HttpPost("upload-profile-image")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

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
            var client = await _clientService.GetClientByIdAsync(clientId.Value);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }


            client.PhotoClient = fileName;
            await _context.SaveChangesAsync();

            return Ok(new { fileName });
        }

        [HttpDelete("remove-profile-image")]
        [Authorize]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            var client = await _clientService.GetClientByIdAsync(clientId.Value);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }


            // Supprimer le fichier du serveur
            if (!string.IsNullOrEmpty(client.PhotoClient))
            {
                var filePath = Path.Combine("wwwroot/Images", client.PhotoClient);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Mettre à jour la base de données
            client.PhotoClient = null;
            await _context.SaveChangesAsync();

            return Ok();
        }
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
                
                   <img [src]=""'wwwroot/Images/' + client?.photoClient"" alt=""Profile"" class=""rounded-circle"" />
               
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
    }
}