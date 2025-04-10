using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;

using System;  // Pour DateTime
using System.IO; // Pour MemoryStream
using System.Linq; // Pour .Select() sur les comptes
using System.Collections.Generic; // Pour IEnumerable<>
using Scriban; // Pour le moteur de templates

using System.IdentityModel.Tokens.Jwt; // Pour JwtRegisteredClaimNames





using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APICompte.Repository.IRepository;

using System.Security.Claims;
using STBEverywhere_back_APICompte.Services;
using System.IdentityModel.Tokens.Jwt;
using STBEverywhere_ApiAuth.Repositories;

using Microsoft.AspNetCore.Components.Web;
using DinkToPdf; // Pour IConverter et HtmlToPdfDocument
using DinkToPdf.Contracts; // Pour les paramètres de conversion
using System.IO; // Pour MemoryStream
using STBEverywhere_Back_SharedModels.Data;
using QuestPDF.Fluent;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;



namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/compte")]
    [ApiController]
    public class CompteAPIController : ControllerBase
    {
        private readonly IConverter _pdfConverter;

        private readonly ICompteService _compteService;
        private readonly ApplicationDbContext _db;
        private readonly IVirementRepository _dbVirement;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CompteAPIController> _logger;
        private readonly IMapper _mapper;

        public CompteAPIController(IConverter pdfConverter, ICompteService compteService, IUserRepository userRepository, ApplicationDbContext db, IVirementRepository dbVirement, IHttpContextAccessor httpContextAccessor, ILogger<CompteAPIController> logger, IMapper mapper)

 main
        {
            _compteService = compteService;
            _db = db;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _pdfConverter = pdfConverter;
        }




























        /*

        [HttpGet("getComptesByAgence/{agenceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComptesByAgence(string agenceId)
        {
            try
            {
                var comptes = await _compteService.GetComptesByAgenceIdAsync(agenceId);

                if (!comptes.Any())
                {
                    return NotFound(new { message = "Aucun compte trouvé pour cette agence." });
                }

                return Ok(comptes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des comptes par agence");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur" });
            }
        }
        */





        [HttpGet("listecompte")]


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetComptesByClientId()
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;

            //var comptes = await _context.Compte
            // var comptes = await _dbCompte.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé");

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé" && c.Type != "Technique");
            if (comptes == null || !comptes.Any())
            {
                return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
            }
            _logger.LogInformation("Getting all comptes");
            return Ok(comptes);
        }

        /* public async Task<IActionResult> GetComptesByCin(string numCin)
         {
             var clientId = GetClientIdFromToken();
             if (clientId == null)
             {
                 return Unauthorized(new { message = "Utilisateur non authentifié" });
             }
             //var comptes = await _context.Compte
             var comptes = await _dbCompte.GetAllAsync(c => c.NumCin == numCin && c.Statut != "Clôturé");
             /*.Where(c => c.NumCin == numCin && c.statut != "Clôturé")
             .Select(c => new
             {
                 c.RIB,
                 c.type,
                 c.solde
             })
             .ToListAsync();

             if (comptes == null || !comptes.Any())
             {
                 return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
             }
             _logger.LogInformation("Getting all comptes");
             return Ok(comptes);
         }*/

        //liste des comptes qui peuvent effectuer des virements (tous les comptes sauf compte epargne) 

        [HttpGet("listecompteVirement")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetComptesVirementByClientId()
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé" && c.Type.ToLower() != "epargne" && c.Type != "Technique");


            if (comptes == null || !comptes.Any())
            {
                return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
            }

            _logger.LogInformation("Récupération des comptes actifs non épargne réussie.");
            return Ok(comptes);
        }

        [HttpPost("CreateCompte")]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        public async Task<IActionResult> CreateCompte([FromBody] CreateCompteDto compteDto)
        {
            var userId = GetUserIdFromToken();
            var Client = await _userRepository.GetClientByUserIdAsync(userId);
            var agenceid = Client.AgenceId;
            var clientId = Client.Id;
            _logger.LogInformation($"ClientId récupéré depuis le token : {clientId}");


            if (compteDto == null || string.IsNullOrEmpty(compteDto.type))
            {
                _logger.LogError(" Type  obligatoire");
                return BadRequest(new { message = "Type  obligatoires." });
            }

            var clientList = await _compteService.GetAllAsync(c => c.ClientId == clientId);

            _logger.LogInformation($"Nombre de clients trouvés : {clientList.Count()}");

            var client = clientList.FirstOrDefault();
            _logger.LogInformation($"Client trouvé ? {(client != null ? "Oui" : "Non")}");

            if (client == null)
            {
                return BadRequest(new { message = "Aucun client trouvé avec ce NumCin." });
            }
            if (compteDto.type.ToLower() == "epargne")
            {
                //var epargneCount = (await _dbCompte.GetAllAsync(c => c.NumCin == compteDto.NumCin && c.Type.ToLower() == "epargne")).Count;

                var epargneCount = (await _compteService.GetAllAsync(c => c.Type.ToLower() == "epargne")).Count;
                if (epargneCount >= 3)
                {
                    return BadRequest(new { message = "Vous ne pouvez pas avoir plus de 3 comptes d'épargne." });
                }
            }
            string generatedRIB = await _compteService.GenerateUniqueRIB(agenceid);
            string iban = _compteService.GenerateIBANFromRIB(generatedRIB);


   

            decimal initialSolde = compteDto.type.ToLower() == "epargne" ? 10 : 0;
            // Utilisation d'AutoMapper pour convertir compteDto en Compte
            var compte = _mapper.Map<Compte>(compteDto);

            // Ajout des valeurs manquantes
            compte.RIB = generatedRIB;
            compte.Solde = initialSolde;
            compte.DateCreation = DateTime.Now;
            compte.Statut = "Actif";
            compte.ClientId = (int)clientId;
            compte.NumCin = client.NumCin;
            compte.NbrOperationsAutoriseesParJour = "illimité";
            compte.MontantMaxAutoriseParJour = 2000.000m;
            compte.DecouvertAutorise = Client.RevenuMensuel;
            compte.IBAN = iban;


            await _compteService.CreateAsync(compte);

            /* await _dbCompte.CreateAsync(compte);
             await _dbCompte.SaveAsync();*/
            //_context.Compte.Add(compte);
            // await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompteByRIB), new { rib = compte.RIB }, compte);
        }








        [HttpGet("GetByRIB/{rib}")]


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompteByRIB(string rib)
        {
            var compte = await _compteService.GetAllAsync(c => c.RIB == rib);
            if (compte == null || !compte.Any())
            {
                return NotFound(new { message = "Aucun compte n'est associé à ce RIB." });
            }

            return Ok(compte);
        }


        [HttpPut("Cloturer/{rib}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloturerCompte(string rib)
        {

            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;

            var compte = (await _compteService.GetAllAsync(c => c.RIB == rib)).FirstOrDefault(); // Correction appliquée

            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            if (compte.Solde != 0)
            {
                return BadRequest(new { message = "Vous devez mettre votre compte à zéro puis réessayer de le clôturer." });
            }

            compte.Statut = "Clôturé";
            await _compteService.SaveAsync();
            return Ok(new { message = "Le compte a été clôturé avec succès." });
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

        [HttpGet("GetSoldeByRIB/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetSoldeByRIB(string rib)
        {
            try
            {
                var solde = await _compteService.GetSoldeByRIBAsync(rib);
                return Ok(new { Solde = solde });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("CreateCompteTechnique")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCompteTechnique([FromBody] CreateCompteDto compteDto)
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            try
            {
                _logger.LogInformation("Début de création d'un compte technique");





                // Génération des identifiants
                string generatedRIB = _compteService.GenerateUniqueRIB();
                string iban = _compteService.GenerateIBANFromRIB(generatedRIB);


                var compte = new Compte
                {
                    RIB = generatedRIB,
                    IBAN = iban,
                    Type = "Technique",
                    // Ajoutez ce champ à votre modèle Compte si nécessaire
                    Solde = 0,
                    DateCreation = DateTime.Now,
                    Statut = "Actif",
                    ClientId = client.Id,
                    NumCin = "TECHNIQUE", // Valeur spéciale pour les comptes techniques
                    NbrOperationsAutoriseesParJour = "illimité",
                    MontantMaxAutoriseParJour = 1000000.000m, // Limite haute pour les comptes techniques
                    DecouvertAutorise = 0 // Pas de découvert pour les comptes techniques
                };

                await _compteService.CreateAsync(compte);
                _logger.LogInformation($"Compte technique créé avec RIB: {compte.RIB}");

                return CreatedAtAction(nameof(GetCompteByRIB), new { rib = compte.RIB }, new
                {
                    RIB = compte.RIB,
                    IBAN = compte.IBAN,
                    Type = compte.Type,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du compte technique");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Une erreur interne est survenue lors de la création du compte technique."
                });
            }
        }
        [HttpGet("rib/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadRIB()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                if (client == null)
                    return NotFound(new { message = "Client non trouvé" });

                var comptes = await _compteService.GetAllAsync(c => c.ClientId == client.Id && c.Statut != "Clôturé" &&
    c.Type != "Technique");

                if (comptes == null || !comptes.Any())
                    return NotFound(new { message = "Aucun compte actif trouvé pour ce client" });

                // Configuration de la licence QuestPDF (gratuite pour les projets open source)
                QuestPDF.Settings.License = LicenseType.Community;

                var pdfBytes = GeneratePdfWithQuestPDF(client, comptes);
                return File(pdfBytes, "application/pdf", $"Releve_RIB_{client.Nom}_{client.Prenom}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du RIB");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la génération du document" });
            }
        }

        private byte[] GeneratePdfWithQuestPDF(Client client, IEnumerable<Compte> comptes)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // En-tête
                    page.Header()
                        .AlignCenter()
                        .Text("STB EVERYWHERE")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken3);

                    // Contenu principal
                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Column(col =>
                        {
                            // Titre principal
                            col.Item().Text($"Relevé d'Identité Bancaire - {client.Nom} {client.Prenom}")
                                .SemiBold().FontSize(16);

                            // Section Informations Client
                            col.Item().PaddingTop(10).Column(clientCol =>
                            {
                                clientCol.Item().Text("Informations client:").Bold();
                                clientCol.Item().Text($"Nom: {client.Nom} {client.Prenom}");
                                clientCol.Item().Text($"CIN: {client.NumCIN ?? "Non renseigné"}");
                                clientCol.Item().Text($"Date de naissance: {client.DateNaissance:dd/MM/yyyy}");
                                clientCol.Item().Text($"Adresse: {client.Adresse}");
                            });

                            // Section Comptes Bancaires
                            col.Item().PaddingTop(15).Text("Coordonnées bancaires:").Bold();

                            foreach (var compte in comptes)
                            {
                                col.Item().PaddingTop(5).Border(1).Padding(10).Column(accountCol =>
                                {
                                    accountCol.Item().Text($"Type: {compte.Type}").SemiBold();
                                    accountCol.Item().Text($"RIB: {compte.RIB}");
                                    accountCol.Item().Text($"IBAN: {compte.IBAN}");
                                    accountCol.Item().Text($"Solde: {compte.Solde:C}");
                                    accountCol.Item().Text($"Date création: {compte.DateCreation:dd/MM/yyyy}");
                                });
                            }
                        });

                    // Pied de page
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Document généré le ");
                            text.Span($"{DateTime.Now:dd/MM/yyyy à HH:mm}");
                            text.Span(" - STB EVERYWHERE");
                        });
                });
            }).GeneratePdf();
        }

       
    }
    }