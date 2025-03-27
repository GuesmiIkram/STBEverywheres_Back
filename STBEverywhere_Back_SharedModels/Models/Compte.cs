using STBEverywhere_Back_SharedModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace STBEverywhere_Back_SharedModels
{
    public class Compte
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RIB { get; set; } // Clé primaire

        [Required]
        public string Type { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Solde { get; set; }

        public DateTime DateCreation { get; set; }
        public string Statut { get; set; }
        public string NumCin { get; set; }

        public string? NbrOperationsAutoriseesParJour { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal MontantMaxAutoriseParJour { get; set; }

        // Clé étrangère vers Client
        [Required]
        public int ClientId { get; set; } // Clé étrangère
        [JsonIgnore]
        [ForeignKey("ClientId")]
      
        public Client Client { get; set; } // Relation avec Client

        // Relation One-to-Many avec Carte
        public ICollection<Carte> Cartes { get; set; } = new List<Carte>(); // Collection de cartes
    }
}