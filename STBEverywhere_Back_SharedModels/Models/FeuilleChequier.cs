using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class FeuilleChequier
    {
        [Key]
        public int IdFeuille { get; set; }

        [Required]
        public string NumeroFeuille { get; set; } 

        [Required]
        [Column(TypeName = "decimal(10,3)")]
        public decimal PlafondFeuille { get; set; }

        [Required]
        public int DemandeChequierId { get; set; }

        [ForeignKey("DemandeChequierId")]
        public DemandeChequier DemandeChequier { get; set; }
    }
}
