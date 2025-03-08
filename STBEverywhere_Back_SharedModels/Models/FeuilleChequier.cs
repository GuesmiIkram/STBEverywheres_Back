using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class FeuilleChequier
    {
        [Key]
        public int IdFeuille { get; set; }

        [Required]
        public string NumeroFeuille { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 12); // Génération auto

        [Required]
        public decimal PlafondFeuille { get; set; }

        [Required]
        public int DemandeChequierId { get; set; }

        [ForeignKey("DemandeChequierId")]
        public DemandeChequier DemandeChequier { get; set; }
    }
}
