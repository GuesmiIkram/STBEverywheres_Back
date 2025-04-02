using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_Back_SharedModels;
using TheArtOfDev.HtmlRenderer.PdfSharp;


using PdfSharp.Pdf;
using PdfSharp.Drawing;


using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_back_APICompte.Repository.IRepository;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using STBEverywhere_back_APICompte.Services;
using System.IdentityModel.Tokens.Jwt;
using STBEverywhere_ApiAuth.Repositories;
using Microsoft.AspNetCore.Components.Web;
using PdfSharp;
using DinkToPdf.Contracts;
using DinkToPdf;

namespace STBEverywhere_back_APICompte.Controllers
{
    [Route("api/compte")]
    [ApiController]
    public class CompteAPIController : ControllerBase
    {
        private readonly IConverter _pdfConverter;

        private readonly ICompteService _compteService;
        //private readonly ICompteRepository _dbCompte;
        private readonly IVirementRepository _dbVirement;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CompteAPIController> _logger;
        private readonly IMapper _mapper;
        public CompteAPIController(IConverter pdfConverter, ICompteService compteService, IUserRepository userRepository /*ICompteRepository dbCompte*/, IVirementRepository dbVirement, IHttpContextAccessor httpContextAccessor, ILogger<CompteAPIController> logger, IMapper mapper)
        {
            _compteService = compteService;
            //_dbCompte = dbCompte;
            _dbVirement = dbVirement;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _pdfConverter = pdfConverter;
        }


        [HttpGet("impressionIdentiteBancaire")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImpressionIdentiteBancaire(string rib, string iban)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var client = await _userRepository.GetClientByUserIdAsync(userId);

                if (client == null)
                {
                    return NotFound(new { message = "Client non trouvé" });
                }

                // Vérifier que le compte appartient bien au client
                var compte = await _compteService.GetByRIBAsync(rib);
                if (compte == null || compte.ClientId != client.Id)
                {
                    return NotFound(new { message = "Compte non trouvé ou n'appartient pas au client" });
                }

                var pdfBytes = GenerateIdentiteBancairePdf(client, rib, iban);
                return File(pdfBytes, "application/pdf", $"Identite_Bancaire_{client.Nom}_{client.Prenom}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération de l'identité bancaire");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Une erreur est survenue lors de la génération du document" });
            }
        }


        /*
        private byte[] GenerateIdentiteBancairePdf(Client client, string rib, string iban)
        {
            try
            {
                string htmlContent = GenerateIdentiteBancaireHtml(client, rib, iban);

                // Création du document PDF
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Convertir le HTML en PDF
                var container = new HtmlContainer();
                container.SetHtml(htmlContent);
                container.PerformLayout(gfx);
                container.PerformPaint(gfx);

                using (var stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du PDF");
                throw;
            }
        }*/





        /*private byte[] GenerateIdentiteBancairePdf(Client client, string rib, string iban)
        {
            try
            {
                // Générer le contenu HTML
                string htmlContent = GenerateIdentiteBancaireHtml(client, rib, iban);

                // Créer un nouveau document PDF
                var document = new PdfSharpCore.Pdf.PdfDocument();

                // Ajouter une page au document
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                // Obtenir un objet XGraphics pour dessiner sur la page
                var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);

                // Créer un renderer HTML (CORRECTION ICI)
                var container = new TheArtOfDev.HtmlRenderer.Core.HtmlContainer();
                container.SetHtml(htmlContent, TheArtOfDev.HtmlRenderer.Core.Entities.HtmlGenerationStyle.Inline);

                // Dessiner le contenu HTML sur la page PDF
                container.PerformLayout(gfx, new PdfSharpCore.Drawing.XSize(page.Width, page.Height));
                container.PerformPaint(gfx);

                // Sauvegarder dans un MemoryStream
                using (var stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du PDF");
                throw;
            }
        }*/













        private byte[] GenerateIdentiteBancairePdf(Client client, string rib, string iban)
        {
            try
            {
                string htmlContent = GenerateIdentiteBancaireHtml(client, rib, iban);

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
            },
                    Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                }
            }
                };

                return _pdfConverter.Convert(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du PDF");
                throw;
            }
        }






        /*

        private byte[] GenerateIdentiteBancairePdf(Client client, string rib, string iban)
        {
            var pdf = PdfGenerator.GeneratePdf(GenerateIdentiteBancaireHtml(client, rib, iban), (PdfSharp.PageSize)PageSize.A4);
            using (var stream = new MemoryStream())
            {
                pdf.Save(stream, false);
                return stream.ToArray();
            }
        }*/



        private string GenerateIdentiteBancaireHtml(Client client, string rib, string iban)
        {
            // Vérification et formatage des données
            rib = rib?.Replace(" ", "") ?? "";
            iban = iban?.Replace(" ", "") ?? "";

            // Extraction des parties du RIB
            string codeBanque = rib.Length >= 2 ? rib.Substring(0, 2) : "";
            string codeAgence = rib.Length >= 5 ? rib.Substring(2, 3) : "";
            string numeroCompte = rib.Length >= 15 ? rib.Substring(5, 10) : "";
            string devise = rib.Length >= 18 ? rib.Substring(15, 3) : "";
            string cle = rib.Length >= 20 ? rib.Substring(18, 2) : "";

            // Formatage de l'IBAN
            string ibanPart1 = iban.Length >= 4 ? iban.Substring(0, 4) : "";
            string ibanPart2 = iban.Length > 4 ? iban.Substring(4) : "";

            return $@"
<html>
    <head>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 20px; }}
            .header {{ text-align: center; margin-bottom: 20px; }}
            .logo {{ width: 150px; height: auto; }}
            .title {{ font-size: 18px; font-weight: bold; margin: 10px 0; }}
            .notice {{ font-style: italic; margin-bottom: 20px; text-align: center; }}
            .section {{ margin-bottom: 15px; }}
            .section-title {{ font-weight: bold; margin-bottom: 5px; }}
            .two-columns {{ display: flex; margin-bottom: 15px; }}
            .column {{ flex: 1; }}
            .table {{ width: 100%; border-collapse: collapse; margin-bottom: 15px; }}
            .table th, .table td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
            .table th {{ background-color: #f2f2f2; }}
            .no-border-table {{ border: none; }}
            .no-border-table td {{ border: none; padding: 3px 0; }}
        </style>
    </head>
    <body>
        <div class='header'>
            <img src='https://www.stb.com.tn/Portals/0/STB_LOGO.png' class='logo' alt='STB Logo'>
            <div class='title'>RELEVÉ D'IDENTITÉ BANCAIRE</div>
            <div class='notice'>Ce relevé est destiné à être remis sur leur demande à vos débiteurs étrangers</div>
        </div>

        <div class='two-columns'>
            <div class='column'>
                <div class='section-title'>TITULAIRE DU COMPTE</div>
                <table class='no-border-table'>
                    <tr><td>Nom:</td><td>{client.Nom}</td></tr>
                    <tr><td>Prénom:</td><td>{client.Prenom}</td></tr>
                    <tr><td>Adresse:</td><td>{client.Adresse}</td></tr>
                </table>
            </div>
            <div class='column'>
                <div class='section-title'>DOMICILIATION</div>
                <div>Société Tunisienne de Banque</div>
                <div>Agence {codeAgence}</div>
            </div>
        </div>

        <div class='section'>
            <div class='section-title'>RIB - IDENTIFIANT DU COMPTE NATIONAL</div>
            <table class='table'>
                <tr>
                    <th>Code Banque</th>
                    <th>Code Agence</th>
                    <th>Numéro de Compte</th>
                    <th>Devise</th>
                    <th>Clé</th>
                </tr>
                <tr>
                    <td>{codeBanque}</td>
                    <td>{codeAgence}</td>
                    <td>{numeroCompte}</td>
                    <td>{devise}</td>
                    <td>{cle}</td>
                </tr>
            </table>
        </div>

        <div class='section'>
            <div class='section-title'>IBAN - IDENTIFIANT INTERNATIONAL DU COMPTE</div>
            <table class='table'>
                <tr>
                    <td>{ibanPart1}</td>
                    <td>{ibanPart2}</td>
                </tr>
            </table>
        </div>

        <div class='section'>
            <div class='section-title'>BIC - IDENTIFIANT INTERNATIONAL DE LA BANQUE</div>
            <table class='table'>
                <tr>
                    <td>STBKTNTT</td>
                </tr>
            </table>
        </div>
    </body>
</html>";
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

            //var comptes = await _context.Compte
            // var comptes = await _dbCompte.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé");

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé");
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

            var comptes = await _compteService.GetAllAsync(c => c.ClientId == clientId && c.Statut != "Clôturé" && c.Type.ToLower() != "epargne");


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

            /*if (client == null)
            {
                return BadRequest(new { message = "Aucun client trouvé avec ce NumCin." });
            }*/
            if (compteDto.type.ToLower() == "epargne")
            {
                //var epargneCount = (await _dbCompte.GetAllAsync(c => c.NumCin == compteDto.NumCin && c.Type.ToLower() == "epargne")).Count;

                var epargneCount = (await _compteService.GetAllAsync(c => c.Type.ToLower() == "epargne")).Count;
                if (epargneCount >= 3)
                {
                    return BadRequest(new { message = "Vous ne pouvez pas avoir plus de 3 comptes d'épargne." });
                }
            }

            string generatedRIB = _compteService.GenerateUniqueRIB();
            string iban= _compteService.GenerateIBANFromRIB(generatedRIB);
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
            compte.DecouvertAutorise = Client.RevenuMensuel ;
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








    }

}