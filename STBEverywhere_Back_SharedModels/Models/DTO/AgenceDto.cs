namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class AgenceDto
    {
        public string CodeAgence { get; set; } // Clé de référence
        public string Libelle { get; set; }    // Seuls les champs nécessaires
        public string CodePostal { get; set; } // Optionnel
    }
}
