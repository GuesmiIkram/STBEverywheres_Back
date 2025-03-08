using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        [Required]
        public int DemandeId { get; set; }

        [ForeignKey("DemandeId")]
        public DemandeChequier DemandeChequier { get; set; }
        public string Contenu { get; set; }
        public string Destinataire { get; set; }

        public string Sujet { get; set; }
        public string EmailType { get; set; } // Ex: "ValidationDemande", "ChequierDispo"
        public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;
        public bool IsEnvoye { get; set; } = false;
    }
}
