using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class DemandeCarte
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Iddemande { get; set; } // Identifiant unique de la demande
    



    [Required]
    [Column("RIB")] // Mappe vers la colonne RIB en base
    public string NumCompte { get; set; }

    [ForeignKey("NumCompte")] // Nom de la propriété
    [JsonIgnore]
    public Compte Compte { get; set; }


    [Required]

    public NomCarte NomCarte { get; set; }
    public TypeCarte TypeCarte { get; set; }

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
    public StatutDemande Statut { get; set; }
    public bool EmailEnvoye { get; set; }
    public bool EmailEnvoyeLivree { get; set; }
    public bool CarteAjouter { get; set; }

    public ICollection<Carte> Cartes { get; set; } = new List<Carte>(); // Collection de cartes
}

// Enum pour représenter les statuts de la demande
