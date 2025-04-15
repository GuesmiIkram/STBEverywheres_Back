namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ReclamationResponseDto
    {
        public int Id { get; set; }
        public string Objet { get; set; }
        public string Description { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateResolution { get; set; }
        public string Statut { get; set; }
    }
}
