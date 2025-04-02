using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class DemandeModificationDecouvert
    {


        [Key]
        public int Id { get; set; } // Clé primaire auto-incrémentée

        [Required]
        public string RIBCompte { get; set; } // RIB du compte concerné

        [Required]
        public decimal DecouvertDemande { get; set; } // Montant du découvert demandé

        [Required]
        public string StatutDemande { get; set; } = "En attente"; // Statut initial

        [Required]
        public DateTime DateDemande { get; set; } = DateTime.Now; // Date de la demande

        public bool NotificationEnvoyee { get; set; } = false; // Pour éviter les doublons de notifications en temps réel
        public bool MailEnvoyee { get; set; } = false; // Pour éviter les doublons d'emails


        // Relation avec la table Compte (clé étrangère)
        [ForeignKey("RIBCompte")]
        public Compte Compte { get; set; }
    }


}

