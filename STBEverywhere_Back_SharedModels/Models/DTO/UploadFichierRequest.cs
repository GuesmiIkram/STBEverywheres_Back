using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class UploadFichierRequest
    {
        [Required]
        public IFormFile? Fichier { get; set; }
    }
}
