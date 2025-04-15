using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Services;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using STBEverywhere_ApiAuth.Repositories;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.Crypto;
using STBEverywhere_Back_SharedModels;
using System.Text;
using STBEverywhere_back_APIChequier.Repository;
namespace STBEverywhere_back_APIChequier.Controllers
{
    [Route("api/DemandeChequierApi")]
    [ApiController]
    public class DemandeChequierController : ControllerBase
    {
        private readonly DemandeChequierService _DemandeChequierService;
        private readonly EmailService _emailService;
        //private readonly ILogger<DemandeChequierController> _logger;
        private readonly IDemandesChequiersRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DemandeChequierController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IDemandesChequiersRepository _DemandesChequiersRepository;


        public DemandeChequierController(IDemandesChequiersRepository DemandesChequiersRepository ,DemandeChequierService DemandeChequierService,ILogger<DemandeChequierController> logger,HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IDemandesChequiersRepository repository,EmailService emailService)
        {
            _logger = logger;
            _DemandeChequierService = DemandeChequierService;
            _httpClient = httpClient;
            _DemandesChequiersRepository = DemandesChequiersRepository;
            _repository = repository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
           
            _userRepository = userRepository;
        }




        [HttpGet("getDemandesChequierByAgence/{agenceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetDemandesChequierByAgence(string AgenceId)
        {
            try
            {
                _logger.LogInformation("Appel de GetDemandesChequierByAgence avec AgenceId: {AgenceId}", AgenceId);

                var result = await _DemandeChequierService.GetDemandesChequierByAgenceIdAsync(AgenceId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetDemandesChequierByAgence : {Message}", ex.Message);
                return StatusCode(500, new { message = "Erreur interne du serveur", erreur = ex.Message });
            }
        }





        [HttpPost("update-statut")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatutDemande(DemandeStatus NouveauStatut, int IdDemande, int IdAgent)
        {
            try
            {
                // Récupérer la demande à partir de son identifiant
                var demande = await _DemandesChequiersRepository.GetByIdAsync(IdDemande);

                if (demande == null)
                {
                    return NotFound("Demande introuvable.");
                }

                // Vérifier les règles de transition de statut en fonction du mode de livraison
                if (demande.ModeLivraison == ModeLivraison.LivraisonAgence &&
                    (NouveauStatut != DemandeStatus.DisponibleEnAgence && NouveauStatut != DemandeStatus.RemisAuClient))
                {
                    return BadRequest("Statut non valide pour une livraison en agence.");
                }

                if (demande.ModeLivraison == ModeLivraison.EnvoiRecommande &&
                    NouveauStatut != DemandeStatus.Expedie)
                {
                    return BadRequest("Statut non valide pour un envoi recommandé.");
                }

                // Mise à jour du statut et de l'agent ayant traité la demande
                demande.Status = NouveauStatut;
                demande.IdAgent = IdAgent;

                await _DemandesChequiersRepository.UpdateAsync(demande);

                // Retourner une réponse claire et standardisée
                return Ok(new { message = $"Demande {IdDemande} mise à jour avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut");
                return StatusCode(500, new { error = $"Erreur interne : {ex.Message}" });
            }
        }




        [HttpPost("DemandeChequierBarre")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DemanderChequierBarre([FromBody] DemandeChequierDTO demandeDto)
        {
            if (demandeDto == null || string.IsNullOrWhiteSpace(demandeDto.RibCompte))
            {
                return BadRequest("Les informations de la demande sont invalides.");
            }

            // por verifier qu'un compte peut pas avoir plus qu'une demande en cours pour  même compte
            bool hasPendingRequest = await _repository.HasDemandeEncours(demandeDto.RibCompte);
            if (hasPendingRequest)
            {
                return BadRequest("Une demande de chéquier est déjà en cours de préparation pour ce compte.");
            }
            // Vérifier si le compte est de type épargne
            bool isEpargne = await _repository.IsCompteEpargne(demandeDto.RibCompte);
            if (isEpargne)
            {
                return BadRequest("Les comptes de type épargne ne peuvent pas faire de demande de chéquier.");
            }
            

            // Vérifier si le compte a déjà un chéquier actif
            bool hasActiveChequier = await _repository.HasActiveChequier(demandeDto.RibCompte);
            if (hasActiveChequier)
            {
                return BadRequest("Le compte a déjà un chéquier actif, vous ne pouvez pas soumettre une nouvelle demande.");
            }

            if (demandeDto.PlafondChequier > 30000)
            {
                return BadRequest("Le plafond du chéquier ne peut pas dépasser 30 000 dinars.");
            }
            // Validation du mode de livraison
            /*if (demandeDto.ModeLivraison == ModeLivraison.LivraisonAgence)
            {
                if (string.IsNullOrWhiteSpace(demandeDto.Agence))
                {
                    return BadRequest("L'agence doit être spécifiée pour une livraison en agence.");
                }
            }
            else */if (demandeDto.ModeLivraison == ModeLivraison.EnvoiRecommande)
            {
                if (string.IsNullOrWhiteSpace(demandeDto.AdresseComplete) || string.IsNullOrWhiteSpace(demandeDto.CodePostal))
                {
                    return BadRequest("L'adresse complète et le code postal sont obligatoires pour un envoi recommandé.");
                }
            }


            int totalFeuillesEmises = await _repository.CountFeuillesByRib(demandeDto.RibCompte);

            // Créer une nouvelle demande de chéquier
            var demande = new DemandeChequier
            {
                RibCompte = demandeDto.RibCompte,
                NombreFeuilles = demandeDto.NombreFeuilles,
                Otp = demandeDto.Otp,
                //Agence = demandeDto.Agence,
               // Agence = demandeDto.ModeLivraison == ModeLivraison.LivraisonAgence ? demandeDto.Agence : null,
                AdresseComplete = demandeDto.ModeLivraison == ModeLivraison.EnvoiRecommande ? demandeDto.AdresseComplete : null,
                CodePostal = demandeDto.ModeLivraison == ModeLivraison.EnvoiRecommande ? demandeDto.CodePostal : null,
                Email = demandeDto.Email,
                NumTel = demandeDto.NumTel,
                PlafondChequier = demandeDto.PlafondChequier,
                Status = DemandeStatus.EnCoursPreparation,
                DateDemande = DateTime.Now,
                isBarre=true,
                AccepteEngagement = null,
                RaisonDemande = null,
                ModeLivraison=demandeDto.ModeLivraison
            };
            

            decimal plafondFeuille = Math.Round(demandeDto.PlafondChequier / demandeDto.NombreFeuilles, 2);
            decimal correction = demandeDto.PlafondChequier - (plafondFeuille * demandeDto.NombreFeuilles);

            for (int i = 0; i < demandeDto.NombreFeuilles; i++)
            {
                decimal montantFeuille = plafondFeuille;
                if (i == demandeDto.NombreFeuilles - 1) // Ajustement pour éviter des erreurs d’arrondi
                {
                    montantFeuille += correction;
                }
                int numFeuille = totalFeuillesEmises + i + 1;
                string numeroFeuille = numFeuille.ToString("D7") + demandeDto.RibCompte;

                demande.Feuilles.Add(new FeuilleChequier
                {
                    PlafondFeuille = montantFeuille,
                    NumeroFeuille = numeroFeuille,
                    DemandeChequier = demande
                });
            }
            await _repository.AddAsync(demande);
            //await _context.DemandesChequiers.AddAsync(demande);

            // Sauvegarder dans la base de données
            //await _context.SaveChangesAsync();
            await _repository.SaveAsync();

            // Envoi d'un e-mail de confirmation
            await _emailService.SendEmailAsync(demande.Email, "Demande de chéquier reçue",
                $"Votre demande de chéquier a bien été enregistrée et est en cours de traitement.");

            // Retourner une réponse après l'ajout des feuilles et l'envoi de l'email
            return Ok(new { message = "Demande de chéquier soumise avec succès.", demandeId = demande.IdDemande });
        }

        [HttpPost("DemandeChequierNonBarre")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DemanderChequierNonBarre([FromBody] DemandeChequierDTO demandeDto)
        {
            if (demandeDto == null || string.IsNullOrWhiteSpace(demandeDto.RibCompte) || string.IsNullOrWhiteSpace(demandeDto.RaisonDemande))
            {
                return BadRequest("Les informations de la demande sont invalides.");
            }
            // por verifier qu'un compte peut pas avoir plus qu'une demande en cours pour même compte
            bool hasPendingRequest = await _repository.HasDemandeEncours(demandeDto.RibCompte);
            if (hasPendingRequest)
            {
                return BadRequest("Une demande de chéquier est déjà en cours de préparation pour ce compte.");
            }
            // Vérifier si le compte est de type épargne
            bool isEpargne = await _repository.IsCompteEpargne(demandeDto.RibCompte);
            if (isEpargne)
            {
                return BadRequest("Les comptes de type épargne ne peuvent pas faire de demande de chéquier.");
            }


            // Vérifier si le compte a déjà un chéquier actif
            bool hasActiveChequier = await _repository.HasActiveChequier(demandeDto.RibCompte);
            if (hasActiveChequier)
            {
                return BadRequest("Le compte a déjà un chéquier actif, vous ne pouvez pas soumettre une nouvelle demande.");
            }

            if (demandeDto.PlafondChequier > 30000)
            {
                return BadRequest("Le plafond du chéquier ne peut pas dépasser 30 000 dinars.");
            }


            if (string.IsNullOrWhiteSpace(demandeDto.RaisonDemande))
            {
                return BadRequest("La raison de la demande doit être renseignée pour un chéquier non barré.");
            }

            if ((bool)!demandeDto.AccepteEngagement)
            {
                return BadRequest("Vous devez accepter l'engagement pour continuer.");
            }

            // Vérification de l'OTP (authentification renforcée)
            /*var otpValide = await _otpService.VerifierOtp(demandeDto.Otp);
            if (!otpValide)
            {
                return BadRequest("Le code OTP est invalide.");
            }*/
            int totalFeuillesEmises = await _repository.CountFeuillesByRib(demandeDto.RibCompte);

            // Créer une nouvelle demande de chéquier non barré
            var demande = new DemandeChequier
            {
                RibCompte = demandeDto.RibCompte,
                NombreFeuilles = demandeDto.NombreFeuilles,
                Otp = demandeDto.Otp,
                //Agence = demandeDto.Agence,
                Email = demandeDto.Email,
                NumTel = demandeDto.NumTel,
                PlafondChequier = demandeDto.PlafondChequier,
                RaisonDemande = demandeDto.RaisonDemande, // Raison de la demande
                Status = DemandeStatus.EnCoursPreparation,
                DateDemande = DateTime.Now,
                isBarre = false // Chéquier non barré
            };

            // Calcul des plafonds par feuille
            decimal plafondFeuille = Math.Round(demandeDto.PlafondChequier / demandeDto.NombreFeuilles, 2);
            decimal correction = demandeDto.PlafondChequier - (plafondFeuille * demandeDto.NombreFeuilles);

            for (int i = 0; i < demandeDto.NombreFeuilles; i++)
            {
                decimal montantFeuille = plafondFeuille;
                if (i == demandeDto.NombreFeuilles - 1) // Ajustement pour éviter des erreurs d’arrondi
                {
                    montantFeuille += correction;
                }
                int numFeuille = totalFeuillesEmises + i + 1;
                string numeroFeuille = numFeuille.ToString("D7") + demandeDto.RibCompte;

                demande.Feuilles.Add(new FeuilleChequier
                {
                    PlafondFeuille = montantFeuille,
                    NumeroFeuille = numeroFeuille,
                    DemandeChequier = demande
                });
            }



            await _repository.AddAsync(demande);
            //await _context.DemandesChequiers.AddAsync(demande);

            // Sauvegarder dans la base de données
            //await _context.SaveChangesAsync();
            await _repository.SaveAsync();

            // Envoi d'un e-mail de confirmation
            await _emailService.SendEmailAsync(demande.Email, "Demande de chéquier reçue",
                $"Votre demande de chéquier a bien été enregistrée et est en cours de traitement.");

            // Retourner une réponse après l'ajout des feuilles et l'envoi de l'email
            return Ok(new { message = "Demande de chéquier soumise avec succès.", demandeId = demande.IdDemande });



            

            // Générer un fichier PDF de confirmation pour téléchargement (futur)
            /*var pdfConfirmation = await _pdfService.GenererConfirmationPdf(demande);
            await _emailService.EnvoyerPdfParEmail(demande.Email, pdfConfirmation);*/

            
        }

        [HttpGet("by-rib")]
        public async Task<IActionResult> GetDemandesByRib(string rib)
        {
            var demandes = await _DemandesChequiersRepository.GetDemandesByRibComptes(new List<string> { rib });
            return Ok(demandes);
        }


        [HttpGet("ListeDemandesParClient")]
        [Authorize]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDemandesParClient()
        {
            var userId = GetUserIdFromToken();
            var client = await _userRepository.GetClientByUserIdAsync(userId);
            
            try
            {
                int clientId = client.Id;
                // Récupérer les RIBs des comptes associés à ce client
                var ribComptes = await _repository.GetRibComptesByClientId(clientId);

                if (!ribComptes.Any())
                {
                    return NotFound("Aucun compte trouvé pour ce client.");
                }

                // Récupérer les demandes de chéquiers associées aux comptes du client
                var demandes = await _repository.GetDemandesByRibComptes(ribComptes);

                if (!demandes.Any())
                {
                    return NotFound("Aucune demande de chéquier trouvée pour ce client.");
                }
                //les demandes seront affichés de la plus récentes à la plus ancienne
                var demandesTriees = demandes.OrderByDescending(d => d.DateDemande);

                // Mapper les données à retourner
                var result = demandesTriees.Select(d => new
                {
                    d.DateDemande,
                    d.RibCompte,
                    
                    Status = d.Status.ToString(),
                    Type = d.isBarre ? "chéque barré" : "chéque non barrée",


                    d.PlafondChequier,
                    d.NombreFeuilles
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
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