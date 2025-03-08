using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STBEverywhere_back_APIChequier.Repository;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STBEverywhere_back_APIChequier.Controllers
{
    [Route("api/ChequierApi")]
    [ApiController]
    public class ChequierController : ControllerBase
    {
        private readonly IChequierRepository _repository;
        private readonly IDemandesChequiersRepository _demandesChequiersRepository;

        public ChequierController(IChequierRepository repository, IDemandesChequiersRepository demandesChequiersRepository)
        {
            _repository = repository;
            _demandesChequiersRepository = demandesChequiersRepository;
        }

        [HttpGet("cheques")]
        [Authorize]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChequesParClient()
        {
            var idClient = GetClientIdFromToken();
            Console.WriteLine($"Client ID: {idClient}");

            if (!idClient.HasValue)
            {
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }

            try
            {
                int clientId = idClient.Value;

                // Récupérer les RIBs des comptes du client
                var ribComptes = await _demandesChequiersRepository.GetRibComptesByClientId(clientId);

                if (!ribComptes.Any())
                {
                    return NotFound("Aucun compte trouvé pour ce client.");
                }

                // Récupérer les demandes de chéquiers associées aux comptes du client
                var demandes = await _demandesChequiersRepository.GetDemandesByRibComptes(ribComptes);

                if (!demandes.Any())
                {
                    return NotFound("Aucune demande de chéquier trouvée pour ce client.");
                }

                // Récupérer directement les chéquiers liés aux demandes de ce client
                var demandesIds = demandes.Select(d => d.IdDemande).ToList();
                var chequiers = await _repository.GetChequiersByDemandesIds(demandesIds);

                if (!chequiers.Any())
                {
                    return NotFound("Aucun chéquier trouvé pour ce client.");
                }

                // Formater les résultats pour le retour API
                var chequiersClient = chequiers.Select(c => new
                {
                    Status = c.Status.ToString(),
                    c.DateLivraison,
                    NumeroChequier = demandes.First(d => d.IdDemande == c.DemandeChequierId).NumeroChequier,
                    PlafondChequier = demandes.First(d => d.IdDemande == c.DemandeChequierId).PlafondChequier,
                    RibCompte = demandes.First(d => d.IdDemande == c.DemandeChequierId).RibCompte
                }).ToList();

                return Ok(chequiersClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
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
