using STBEverywhere_Back_SharedModels.Models.enums;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class UpdateStatutDemandeDto
    {
        public StatutDemande NouveauStatut { get; set; }

        [StringLength(500)]
        public string? Commentaire { get; set; }
    }
}
