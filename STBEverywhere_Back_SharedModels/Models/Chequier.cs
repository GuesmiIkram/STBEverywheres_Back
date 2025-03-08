using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class Chequier
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int DemandeChequierId { get; set; }

        [ForeignKey("DemandeChequierId")]
        public DemandeChequier DemandeChequier { get; set; }

        [Required]
        public ChequierStatus Status { get; set; } = ChequierStatus.Active;


        public DateTime? DateLivraison { get; set; } // Nullable

       //public List<FeuilleChequier> Feuilles { get; set; } = new List<FeuilleChequier>();

    }

    public enum ChequierStatus
    {
        Active,// Le chéquier est actif et peut être utilisé
        Désactivé, // Le chéquier est désactivé (perdu, suspendu, etc.)
        Bloqué // Le chéquier est bloqué (fraude, décision de la banque, etc.)
    }

}
