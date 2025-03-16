namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } // Ancien mot de passe
        public string NewPassword { get; set; } // Nouveau mot de passe
        public string ConfirmNewPassword { get; set; } // Confirmation du nouveau mot de passe
    }
}
