using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ReclamationDto
    {
        [Required]
        [MaxLength(100)]
        public string Objet { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
