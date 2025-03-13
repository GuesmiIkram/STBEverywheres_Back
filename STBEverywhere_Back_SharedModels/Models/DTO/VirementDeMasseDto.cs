namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class VirementDeMasseDto
    {
        public string RIB_Emetteur { get; set; }
        public decimal Montant { get; set; }
        public string Description { get; set; }
        public string Motif { get; set; }
        public string FichierBeneficaires { get; set; }
    }
}
