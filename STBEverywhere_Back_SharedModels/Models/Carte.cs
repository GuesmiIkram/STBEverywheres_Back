using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Carte
{
    [Key] 
    [Required]
    public string NumCarte { get; set; } // Numéro de la carte (identifiant unique)

    [Required]
    public string NomCarte { get; set; } // Visa, Mastercard, Tecno, etc.

    [Required]
    public string TypeCarte { get; set; }  // "National" ou "International"

    [Required]
    public DateTime DateCreation { get; set; } // Date de création de la carte

    [Required]
    public DateTime DateExpiration { get; set; } // Date d'expiration de la carte

    [Required]
    public string Statut { get; set; }  // Statut actif/inactif 
    public string? Nature { get; set; }  // postpayee pépayee

    [Required]
    public int Iddemande { get; set; } // Référence à la demande de carte

    [ForeignKey("Iddemande")]
    public DemandeCarte DemandeCarte { get; set; }

    // Clé étrangère vers Compte
    [Required]
    public string RIB { get; set; }
    public decimal? Solde { get; set; }// Référence au compte

    [ForeignKey("RIB")]
    public Compte Compte { get; set; } // Relation avec Compte

    // Nouveaux champs
    public DateTime? DateRecuperation { get; set; } // Date de récupération de la carte (nullable)


    [Required]
     // Code PIN à 4 chiffres
    public String CodePIN { get; set; }

    [Required]
     // Code CVV à 3 chiffres
    public String  CodeCVV { get; set; }
     [Column(TypeName = "decimal(18, 2)")] 
    public decimal PlafondTPE { get; set; }//par defaut 4000 pour toute les carte 
    public decimal PlafondDAP{ get; set; }//par defaut 2000 pour toute les carte 
}