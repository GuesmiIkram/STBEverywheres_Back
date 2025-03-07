using STBEverywhere_Back_SharedModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DemandeCarte
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Iddemande { get; set; } // Identifiant unique de la demande

    [Required]
    public string NumCompte { get; set; } // Référence au compte

    [Required]
    public string NomCarte { get; set; } // Visa, Mastercard, Tecno, etc.

    [Required]
    public string TypeCarte { get; set; }  // "National" ou "International"

    [Required]
    public string CIN { get; set; } // Numéro CIN du client

    [Required, EmailAddress]
    public string Email { get; set; } // Email du client

    [Required]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Le numéro de téléphone doit contenir 8 chiffres.")]
    public string NumTel { get; set; } // Numéro de téléphone

    public DateTime DateCreation { get; set; } = DateTime.Now; // Date de la demande

    // Statut de la demande
    [Required]
    public String Statut { get; set; } // Statut de la demande

    // Clé étrangère vers Client
    [Required]
    public int ClientId { get; set; } // Référence au client

    [ForeignKey("ClientId")]
    public Client Client { get; set; } // Relation avec Client

    public bool EmailEnvoye { get; set; }
    public bool EmailEnvoyeLivree { get; set; }
    public bool CarteAjouter { get; set; }
}

// Enum pour représenter les statuts de la demande
