namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class ReponseDemandeDecouvertDto
    {
        public int IdDemande { get; set; }
        public bool Accepte { get; set; }
        public string? MotifRefus { get; set; }
        
    }
}
