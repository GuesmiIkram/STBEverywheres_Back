namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class VirementDto
    {
        public string RIB_Emetteur { get; set; }   // RIB du compte émetteur (obligatoire)
        public string RIB_Recepteur { get; set; }   // RIB du compte récepteur (obligatoire)
        public decimal Montant { get; set; }       // Montant à virer (obligatoire)
        public string Description { get; set; }

        public string motif { get; set; }
    }
}

