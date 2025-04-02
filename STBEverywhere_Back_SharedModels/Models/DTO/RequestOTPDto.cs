using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class RequestOTPDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
