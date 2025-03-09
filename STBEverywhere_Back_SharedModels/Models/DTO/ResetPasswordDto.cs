namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
