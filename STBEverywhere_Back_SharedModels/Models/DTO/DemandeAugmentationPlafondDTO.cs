using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class DemandeAugmentationPlafondDTO
    {
        [Required]
        public string NumCarte { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal NouveauPlafondTPE { get; set; }

        [Required]
        [Range(0.01, 50000)]
        public decimal NouveauPlafondDAB { get; set; }

        [StringLength(500)]
        public string Raison { get; set; }



    }
}
