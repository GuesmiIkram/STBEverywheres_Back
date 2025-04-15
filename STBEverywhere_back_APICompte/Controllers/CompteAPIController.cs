using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;



using System.IdentityModel.Tokens.Jwt; // Pour JwtRegisteredClaimNames

using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APICompte.Repository.IRepository;

using System.Security.Claims;
using STBEverywhere_back_APICompte.Services;

using STBEverywhere_ApiAuth.Repositories;

using DinkToPdf.Contracts; // Pour les paramètres de conversion

using STBEverywhere_Back_SharedModels.Data;
using QuestPDF.Fluent;

using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;





namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/compte")]
    [ApiController]
    public class CompteAPIController : ControllerBase
    {
        private readonly IConverter _pdfConverter;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICompteService _compteService;
        private readonly ApplicationDbContext _db;
        private readonly IVirementRepository _dbVirement;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CompteAPIController> _logger;
        private readonly IMapper _mapper;

        public CompteAPIController(IConverter pdfConverter, IWebHostEnvironment webHostEnvironment, ICompteService compteService, IUserRepository userRepository, ApplicationDbContext db, IVirementRepository dbVirement, IHttpContextAccessor httpContextAccessor, ILogger<CompteAPIController> logger, IMapper mapper)


        {
            _compteService = compteService;
            _db = db;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _pdfConverter = pdfConverter;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpGet("allComptes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllComptes()
        {
            try
            {
                var comptes = await _compteService.GetAllAsync(); // Sans filtre = tous les comptes

                if (comptes == null || !comptes.Any())
                {
                    return NotFound(new { message = "Aucun compte trouvé dans la base de données." });
                }

                _logger.LogInformation("Liste complète des comptes récupérée avec succès.");
                return Ok(comptes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les comptes.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erreur serveur lors de la récupération des comptes." });
            }
        }





        [HttpGet("listecompte")]


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetComptesByClientId()
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            var clientId = client.Id;



            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Type != "Technique");
            if (comptes == null || !comptes.Any())
            {
                return NotFound(new { message = "Aucun compte actif trouvé pour vous." });
            }
            _logger.LogInformation("Getting all comptes");
            return Ok(comptes);
        }



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

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé" && c.Statut != "desactive" && c.Type.ToLower() != "epargne" && c.Type != "Technique");


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

        [HttpPut("desactive/{rib}")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> desactiverCompte(string rib, string idAgent)
        {



            var compte = (await _compteService.GetAllAsync(c => c.RIB == rib)).FirstOrDefault(); // Correction appliquée

            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }


            compte.Statut = "desactive";
            //compte.idAgent = idAgent;

            await _compteService.SaveAsync();
            return Ok(new { message = "Le compte a été désactivé avec succès." });
        }

        [HttpPut("active/{rib}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> activerCompte(string rib, string idAgent)
        {
            var compte = (await _compteService.GetAllAsync(c => c.RIB == rib)).FirstOrDefault();

            if (compte == null)
            {
                return NotFound(new { message = "Compte introuvable." });
            }

            compte.Statut = "actif";
            //compte.idAgent = idAgent;
            await _compteService.SaveAsync();

            return Ok(new { message = "Le compte a été activé avec succès." });
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

            var compte = (await _compteService.GetAllAsync(c => c.RIB == rib)).FirstOrDefault();
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
            var agenceid = client.AgenceId;
            try
            {
                _logger.LogInformation("Début de création d'un compte technique");





                // Génération des identifiants
                string generatedRIB = await _compteService.GenerateUniqueRIB(agenceid);
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



        private byte[] GeneratePdfWithQuestPDF(Client client, string rib, IWebHostEnvironment hostingEnvironment)
        {
            string logoPath = Path.Combine(hostingEnvironment.WebRootPath, "images", "STBlogo.jpg");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header unifié avec logo et ligne séparatrice
                    page.Header()
                        .Column(headerCol =>
                        {
                            // Première ligne avec logo et titres
                            headerCol.Item().Row(row =>
                            {
                                // Logo STB à gauche
                                row.ConstantItem(100).Height(50).Image(logoPath, ImageScaling.FitArea);

                                // Titres à droite
                                row.RelativeItem()
                                   .Column(col =>
                                   {
                                       col.Item().AlignRight().Text("STB BANK").Bold().FontSize(18);
                                       col.Item().AlignRight().Text("RELEVÉ D'IDENTITÉ BANCAIRE RIB").Bold().FontSize(16);
                                   });
                            });

                            // Ligne séparatrice sous l'en-tête
                            headerCol.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
                        });

                    // Contenu principal
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(col =>
                        {
                            // Titulaire du compte et date d'activation
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Titulaire du compte : {client.Nom} {client.Prenom}");
                                row.RelativeItem().AlignRight().Text($"Date d'activation : {DateTime.Now:dd/MM/yyyy}");
                            });

                            // Espace vide
                            col.Item().Height(20);

                            // Vérification et découpage du RIB
                            if (!string.IsNullOrEmpty(rib))
                            {
                                var ribCleaned = rib.Replace(" ", "");

                                // Découpage du RIB selon le format attendu
                                var codeBanque = ribCleaned.Length >= 2 ? ribCleaned.Substring(0, 2) : "";
                                var codeAgence = ribCleaned.Length >= 5 ? ribCleaned.Substring(2, 3) : "";
                                var numCompte = ribCleaned.Length >= 15 ? ribCleaned.Substring(5, 10) : "";
                                var nat = ribCleaned.Length >= 18 ? ribCleaned.Substring(15, 3) : "";
                                var cleRib = ribCleaned.Length >= 20 ? ribCleaned.Substring(18, 2) : "";

                                // Tableau des codes RIB
                                /*col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(50);  // Code Banque
                                        columns.ConstantColumn(50);  // Code Agence
                                        columns.ConstantColumn(100); // Numéro de compte
                                        columns.ConstantColumn(50);  // Nat
                                        columns.ConstantColumn(50);  // Clé RIB
                                    });

                                    // En-têtes du tableau
                                    table.Header(header =>
                                    {
                                        header.Cell().AlignCenter().Text("Code Banque").Bold();
                                        header.Cell().AlignCenter().Text("Code Agence").Bold();
                                        header.Cell().AlignCenter().Text("Numéro de compte").Bold();
                                        header.Cell().AlignCenter().Text("Nat").Bold();
                                        header.Cell().AlignCenter().Text("Clé RIB").Bold();
                                    });

                                    // Données du RIB
                                    table.Cell().AlignCenter().Text(codeBanque);
                                    table.Cell().AlignCenter().Text(codeAgence);
                                    table.Cell().AlignCenter().Text(numCompte.Insert(6, ".")); // Ajout du point pour le format
                                    table.Cell().AlignCenter().Text(nat);
                                    table.Cell().AlignCenter().Text(cleRib);
                                });*/
                                col.Item().Table(table =>
                                {
                                    // Configuration des colonnes
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(50);  // Code Banque
                                        columns.ConstantColumn(50);  // Code Agence
                                        columns.ConstantColumn(100); // Numéro de compte
                                        columns.ConstantColumn(50);  // Nat
                                        columns.ConstantColumn(50);  // Clé RIB
                                    });

                                    // En-têtes du tableau avec bordures
                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).AlignCenter().Text("Code Banque").Bold();
                                        header.Cell().BorderBottom(1).AlignCenter().Text("Code Agence").Bold();
                                        header.Cell().BorderBottom(1).AlignCenter().Text("Numéro de compte").Bold();
                                        header.Cell().BorderBottom(1).AlignCenter().Text("Nat").Bold();
                                        header.Cell().BorderBottom(1).AlignCenter().Text("Clé RIB").Bold();
                                    });

                                    // Données du RIB avec bordures
                                    table.Cell().Border(1).AlignCenter().Text(codeBanque);
                                    table.Cell().Border(1).AlignCenter().Text(codeAgence);
                                    table.Cell().Border(1).AlignCenter().Text(numCompte.Insert(6, "."));
                                    table.Cell().Border(1).AlignCenter().Text(nat);
                                    table.Cell().Border(1).AlignCenter().Text(cleRib);
                                });
                            }

                            // Informations de l'agence
                            col.Item().PaddingTop(20).Column(agenceCol =>
                            {
                                agenceCol.Item().Text("Agence : RAS JEBEL");
                                agenceCol.Item().Text("Adresse : Av. H. Bougaffa - 7070 Ras Jebal");
                                agenceCol.Item().Text("Téléphone : 72447177");
                                agenceCol.Item().Text("Direction : Direction Régionale de Bizerte");
                                agenceCol.Item().Text("Fax : 70143070");
                            });

                            // Ligne séparatrice
                            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Black);

                            // Pied de page avec date et lieu
                            col.Item().AlignRight().Text($"Fait à Tunis le, {DateTime.Now:dd/MM/yyyy}");
                        });
                });
            }).GeneratePdf();
        }



        [HttpGet("rib/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadRIB(string rib)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                if (client == null)
                    return NotFound(new { message = "Client non trouvé" });

                var comptes = await _compteService.GetAllAsync(c => c.ClientId == client.Id && c.Statut != "Clôturé" && c.Type != "Technique");

                if (comptes == null || !comptes.Any())
                    return NotFound(new { message = "Aucun compte actif trouvé pour ce client" });

                // Prendre le premier compte (ou implémenter une logique pour choisir le compte)
                var compte = comptes.FirstOrDefault();
                if (string.IsNullOrEmpty(compte?.RIB))
                    return NotFound(new { message = "RIB non disponible pour ce compte" });

                // Configuration de la licence QuestPDF
                QuestPDF.Settings.License = LicenseType.Community;

                // Génération du PDF avec le RIB spécifique
                var pdfBytes = GeneratePdfWithQuestPDF(client, rib, _webHostEnvironment);
                return File(pdfBytes, "application/pdf", $"RIB_{client.Nom}.pdf");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du RIB");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Une erreur est survenue lors de la génération du document" });
            }
        }

        /* [HttpGet("rib/download")]
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
         }*/

        /*private byte[] GeneratePdfWithQuestPDF(Client client, IEnumerable<Compte> comptes)
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
        }*/


    }
}