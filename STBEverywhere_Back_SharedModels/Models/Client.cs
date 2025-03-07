using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace STBEverywhere_Back_SharedModels
{
    public class Client
    {
        [Key]
        public int Id { get; set; }  // Clé primaire

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        public DateTime DateNaissance { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Adresse { get; set; }

        public string Civilite { get; set; }
        public string Nationalite { get; set; }
        public string EtatCivil { get; set; }
        public string Residence { get; set; }
        public string? NumCIN { get; set; }
        public string? MotDePasse { get; set; }

        // Relation One-to-Many : Un client peut avoir plusieurs comptes
        public ICollection<Compte> Comptes { get; set; } = new List<Compte>();
        public ICollection<DemandeCarte> DemandesCarte { get; set; } = new List<DemandeCarte>();
    }
}
