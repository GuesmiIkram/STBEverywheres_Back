using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  STBEverywhere_Back_SharedModels
{
    public class Virement
    {
        [Key] // Clé primaire auto-incrémentée
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string RIB_Emetteur { get; set; }

        [Required]
        public string RIB_Recepteur { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Le montant doit être supérieur à zéro.")]
        public decimal Montant { get; set; }

        [Required]
        public string Motif { get; set; }

        public DateTime DateVirement { get; set; }  // Date et heure précise du virement

        public string Statut { get; set; }

        public string Description { get; set; }

        
    }


}
