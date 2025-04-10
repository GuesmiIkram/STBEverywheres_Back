using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using STBEverywhere_ApiAgence.Models;
using STBEverywhere_ApiAgence.Services;

namespace STBEverywhere_ApiAgence.Controllers
{




    [ApiController]
    [Route("api/[controller]")]
    public class AgenceApiController : ControllerBase
    {
        private readonly IMongoCollection<Agence> _agences;

        public AgenceApiController(IMongoCollection<Agence> agences)
        {
            _agences = agences;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var agences = await _agences.Find(_ => true).ToListAsync();
            return Ok(agences);
        }




        [HttpGet("byId/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                // Vérifier si l'ID est valide
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("ID invalide");
                }

                var agence = await _agences.Find(a => a.Id == id).FirstOrDefaultAsync();

                if (agence == null)
                {
                    return NotFound($"Agence avec l'ID {id} non trouvée");
                }

                return Ok(agence);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
            }
        }
    }
}
    




