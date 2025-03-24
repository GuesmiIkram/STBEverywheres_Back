using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class Beneficiaire
    {
        [Key]
        public int Id { get; set; }  // Clé primaire

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        public string RaisonSociale { get; set; }
        [Required]
        public string RIBCompte { get; set; }
        public string Telephone { get; set; }

        public string Email { get; set; }

        [Required]
        public BeneficiaireType Type { get; set; }

        // Clé étrangère vers Client
        [Required]
        public int ClientId { get; set; } // Clé étrangère

        [ForeignKey("ClientId")]
        public Client Client { get; set; } // Relation avec Client


    }
        public enum BeneficiaireType
    {
        PersonneMorale,
        PersonnePhisique

    }
    

}
