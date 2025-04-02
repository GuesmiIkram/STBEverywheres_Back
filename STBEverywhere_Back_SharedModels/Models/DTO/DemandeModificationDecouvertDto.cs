using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class DemandeModificationDecouvertDto
    {
        [Required]
        public string RIBCompte { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Le découvert demandé doit être positif.")]
        public decimal DecouvertDemande { get; set; }
    }
}
