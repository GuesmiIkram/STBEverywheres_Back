using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class Beneficiaire
    {
        [Key]
        public int Id { get; set; }  // Clé primaire
        public string Nom { get; set; }

        public string Prenom { get; set; }

       // public string? RaisonSociale { get; set; }
        [Required]
        public string RIBCompte { get; set; }
        public string? Telephone { get; set; }

        public string? Email { get; set; }

       /* [Required]
        [Column(TypeName = "varchar(50)")]*/ // Stocke en tant que chaîne dans MySQL
       // public string Type { get; set; }

        // Clé étrangère vers Client
        [Required]
        public int ClientId { get; set; } // Clé étrangère

        [ForeignKey("ClientId")]
        public Client Client { get; set; } // Relation avec Client

        /*[NotMapped]
        public BeneficiaireType TypeEnum
        {
            get => (BeneficiaireType)Enum.Parse(typeof(BeneficiaireType), Type);
            set => Type = value.ToString();
        }*/


    }




       /* public enum BeneficiaireType
    {
        PersonneMorale,
        PersonnePhisique

    }*/
    

}
