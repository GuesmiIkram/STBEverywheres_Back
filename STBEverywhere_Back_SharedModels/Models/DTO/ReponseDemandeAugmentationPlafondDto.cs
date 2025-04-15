namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ReponseDemandeAugmentationPlafondDto
    {
        public int DemandeId { get; set; }
        public string NouveauStatut { get; set; } // "Approuvee" ou "Rejetee"
        public string Commentaire { get; set; }
    }
}