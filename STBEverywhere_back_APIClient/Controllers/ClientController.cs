using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_back_APIClient.Services;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.IO;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using System.Security.Claims;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // Applique l'authentification à toutes les méthodes du contrôleur
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ApplicationDbContext _context;

        public ClientController(IClientService clientService, ApplicationDbContext context)
        {
            _clientService = clientService;
            _context = context;
        }

       /* [HttpPost("CreateBeneficiaire")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBeneficiaire(CreateBeneficiaireDto compteDto)
        {
            // 1. Extraire le ClientId du token JWT
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            /*if (string.IsNullOrEmpty(clientIdClaim))
            {
                return Unauthorized("ClientId non trouvé dans le token.");
            }

            if (!int.TryParse(clientIdClaim, out int clientId))
            {
                return Unauthorized("ClientId invalide dans le token.");
            }*/

            // 2. Valider les données du DTO
           /* if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 3. Validation spécifique au type de bénéficiaire
            if (compteDto.Type == BeneficiaireType.PersonnePhisique)
            {
                if (string.IsNullOrEmpty(compteDto.Nom) || string.IsNullOrEmpty(compteDto.Prenom))
                {
                    return BadRequest("Le nom et le prénom sont obligatoires pour une personne physique.");
                }
            }
            else if (compteDto.Type == BeneficiaireType.PersonneMorale)
            {
                if (string.IsNullOrEmpty(compteDto.RaisonSociale))
                {
                    return BadRequest("La raison sociale est obligatoire pour une personne morale.");
                }
            }

            // 4. Créer un nouveau bénéficiaire
            var beneficiaire = new Beneficiaire
            {
                Nom = compteDto.Nom,
                Prenom = compteDto.Prenom,
                RIBCompte = compteDto.RIBCompte,
                Telephone = compteDto.Telephone,
                Email = compteDto.Email,
                RaisonSociale = compteDto.RaisonSociale,
                Type = compteDto.Type,
                ClientId = clientIdClaim // Associer le ClientId extrait du token
            };

            // 5. Enregistrer le bénéficiaire dans la base de données
            _context.Beneficiaires.Add(beneficiaire);
            await _context.SaveChangesAsync();

            // 6. Retourner une réponse 201 avec l'URI de la ressource créée
            return CreatedAtAction(nameof(CreateBeneficiaire), new { id = beneficiaire.Id }, beneficiaire);
        }*/

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




        // Récupérer les informations du client
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClientInfo()
        {
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Récupère l'ID du client depuis le token
            var client = await _clientService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(client);
        }

        // Mettre à jour les informations du client
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClientInfo([FromBody] Client updatedClient)
        { 
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Récupère l'ID du client depuis le token
            bool isUpdated = await _clientService.UpdateClientInfoAsync(clientId, updatedClient);
            if (!isUpdated)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            return Ok(new { message = "Informations mises à jour avec succès !" });
        }

        // Télécharger le fichier KYC
        [HttpGet("kyc/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadKYC()
        {
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Récupère l'ID du client depuis le token
            var client = await _clientService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            var pdfBytes = GenerateKYCReport(client);
            return File(pdfBytes, "application/pdf", $"Fiche_KYC_{client.Nom}_{client.Prenom}.pdf");
        }

        // Uploader une image de profil
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Récupère l'ID du client depuis le token
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
            var client = await _clientService.GetClientByIdAsync(clientId);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé" });
            }

            client.PhotoClient = fileName;
            await _context.SaveChangesAsync();

            return Ok(new { fileName });
        }

        // Supprimer la photo de profil
        [HttpDelete("remove-profile-image")]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var clientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Récupère l'ID du client depuis le token
            var client = await _clientService.GetClientByIdAsync(clientId);
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
    }
}