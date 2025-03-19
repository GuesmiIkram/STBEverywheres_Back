using STBEverywhere_Back_SharedModels.Models.enums;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class CarteDetails
    {
        public string NumCarte { get; set; } // Numéro de la carte (identifiant unique)
        public NomCarte NomCarte { get; set; }
        public TypeCarte TypeCarte { get; set; }
        public DateTime DateCreation { get; set; } // Date de création de la carte
        public DateTime DateExpiration { get; set; } // Date d'expiration de la carte
        public StatutCarte Statut { get; set; }  // Statut actif/inactif 
        public string RIB { get; set; } // Référence au compte
        public DateTime? DateRecuperation { get; set; } // Date de récupération de la carte (nullable)
        public int CodePIN { get; set; } // Code PIN à 4 chiffres
        public int CodeCVV { get; set; } // Code CVV à 3 chiffres
        public int Iddemande { get; set; } // Référence à la demande de carte
        public decimal PlafondDAP { get; set; }
        public decimal PlafondTPE { get; set; }
    }
}
