using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class DemandeChequier
    {
        [Key]
        public int IdDemande { get; set; }

        [Required]
        public string RibCompte { get; set; }

        [ForeignKey("RibCompte")]
        public Compte Compte { get; set; }

        [Required]
        public DateTime DateDemande { get; set; } = DateTime.UtcNow;

        [Required]
        public int NombreFeuilles { get; set; }

        [Required]
        public DemandeStatus Status { get; set; } = DemandeStatus.EnCoursPreparation;

        public bool Otp { get; set; }

        [Required]
        public ModeLivraison ModeLivraison { get; set; } // Ajout du mode de livraison

        //public string? Agence { get; set; } // Obligatoire si "LivraisonAgence"

        public string? AdresseComplete { get; set; } // Obligatoire si "EnvoiRecommande"

        public string? CodePostal { get; set; } // Obligatoire si "EnvoiRecommande"

        [Required, EmailAddress]
        public string Email { get; set; } // Email du client

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Le numéro de téléphone doit contenir 8 chiffres.")]
        public string NumTel { get; set; } // Numéro de téléphone

        [Required]
        public string NumeroChequier { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10);

        [Required]
        public decimal PlafondChequier { get; set; }
        public string? RaisonDemande { get; set; } // Raison de la demande de chéquier non barré
        public bool? AccepteEngagement { get; set; } // Accepte l'engagement pour l'encaissement en espèces (nullable)

        public bool isBarre { get; set; } // Indicateur si le chéquier est barré ou non
        public int? IdAgent { get; set; }
        public ICollection<FeuilleChequier> Feuilles { get; set; } = new List<FeuilleChequier>();

        // Relation One-to-Many : Une demande peut avoir plusieurs e-mails
        public ICollection<EmailLog> Emails { get; set; } = new List<EmailLog>();
    }

    public enum DemandeStatus
    {
        EnCoursPreparation=0,
        DisponibleEnAgence=1,
        RemisAuClient=2,
        Expedie= 3 //envoi postale
    }

    public enum ModeLivraison
    {
        LivraisonAgence=0,
        EnvoiRecommande=1
    }

}

