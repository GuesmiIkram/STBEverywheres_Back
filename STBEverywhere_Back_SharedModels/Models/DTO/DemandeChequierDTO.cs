using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class DemandeChequierDTO
    {

       
        public string RibCompte { get; set; }

        public int NombreFeuilles { get; set; }

       
        public bool Otp { get; set; }

        public ModeLivraison ModeLivraison { get; set; }
        public string? AdresseComplete { get; set; }
        public string? CodePostal { get; set; }

        public string Email { get; set; } // Email du client

        
        public string NumTel { get; set; }
        public string RaisonDemande { get; set; } // Raison de la demande de chéquier non barré
        public bool? AccepteEngagement { get; set; } // Accepte l'engagement pour l'encaissement en espèces (nullable)
        public decimal PlafondChequier { get; set; }
    }
}
