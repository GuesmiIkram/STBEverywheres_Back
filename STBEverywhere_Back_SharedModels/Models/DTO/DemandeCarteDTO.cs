using STBEverywhere_Back_SharedModels.Models.enums;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class DemandeCarteDTO
    {
        public int Iddemande { get; set; } // Identifiant unique de la demande
        public string NumCompte { get; set; } // Référence au compte
        public NomCarte NomCarte { get; set; }
        public TypeCarte TypeCarte { get; set; }
        public string CIN { get; set; } // Numéro CIN du client
        public string Email { get; set; } // Email du client
        public string NumTel { get; set; } // Numéro de téléphone
        public DateTime DateCreation { get; set; } = DateTime.Now; // Date de la demande
        public StatutDemande Statut { get; set; }
        public int? ClientId { get; set; } // Référence au client
    }
}