namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class CarteDTO
    {
        public string NumCarte { get; set; } // Numéro de la carte (identifiant unique)
        public string NomCarte { get; set; } // Visa, Mastercard, Tecno, etc.
        public string TypeCarte { get; set; }  // "National" ou "International"
        public DateTime DateCreation { get; set; } // Date de création de la carte
        public DateTime DateExpiration { get; set; } // Date d'expiration de la carte
        public string Statut { get; set; }  // Statut actif/inactif 
        public string RIB { get; set; } // Référence au compte
        public DateTime? DateRecuperation { get; set; } // Date de récupération de la carte (nullable)
        public int CodePIN { get; set; } // Code PIN à 4 chiffres
        public int CodeCVV { get; set; } // Code CVV à 3 chiffres
        public int Iddemande { get; set; } // Référence à la demande de carte
        public decimal Plafond { get; set; }
    }
}