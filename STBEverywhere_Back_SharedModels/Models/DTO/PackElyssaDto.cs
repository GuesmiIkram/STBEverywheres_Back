using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class PackElyssaDto
    {
        [Required]
        public IFormFile Document1 { get; set; } // Passeport

        public IFormFile? Document2 { get; set; } // Visa long séjour

        public IFormFile? Document3 { get; set; } // Justificatif validation visa

        [Required]
        public IFormFile Document4 { get; set; } // Justificatif domicile France

        [Required]
        public IFormFile Document5 { get; set; } // Contrat CDI

        [Required]
        public IFormFile Document6 { get; set; } // Certificat retenue impôt

        [Required]
        public string Agency { get; set; }
    }
}