using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class Carte
    {
        [Key]
        [Required]
        public string NumCarte { get; set; } // Numéro de la carte (identifiant unique)

        [Required]
        public NomCarte NomCarte { get; set; } // Utilisation de l'enum pour le nom de la carte

        [Required]
        public TypeCarte TypeCarte { get; set; }  // Utilisation de l'enum pour le type de carte

        [Required]
        public DateTime DateCreation { get; set; } // Date de création de la carte

        [Required]
        public DateTime DateExpiration { get; set; } // Date d'expiration de la carte

        [Required]
        public StatutCarte Statut { get; set; }  // Utilisation de l'enum pour le statut de la carte

        public string? Nature { get; set; }  // postpayee, prepayee

        [Required]
        public int Iddemande { get; set; } // Référence à la demande de carte

        [ForeignKey("Iddemande")]
        public DemandeCarte DemandeCarte { get; set; }

        // Clé étrangère vers Compte
        [Required]
        public string RIB { get; set; }
        public decimal? Solde { get; set; } // Référence au compte

        [ForeignKey("RIB")]
        public Compte Compte { get; set; } // Relation avec Compte

        // Nouveaux champs
        public DateTime? DateRecuperation { get; set; } // Date de récupération de la carte (nullable)

        [Required]
        public string CodePIN { get; set; } // Code PIN à 4 chiffres

        [Required]
        public string CodeCVV { get; set; } // Code CVV à 3 chiffres

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PlafondTPE { get; set; } // Par défaut 4000 pour toutes les cartes

        public decimal PlafondDAP { get; set; } // Par défaut 2000 pour toutes les cartes
    }
}