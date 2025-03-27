using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public string? ResetPasswordToken { get; set; }

     
    
    }

    public enum UserRole
    {
        Client,
        Agent
    }
}