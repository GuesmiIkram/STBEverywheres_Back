using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class PackElyssa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PassportPath { get; set; } // Document 1

        public string? LongStayVisaPath { get; set; } // Document 2 (optionnel)

        public string? VisaRegistrationPath { get; set; } // Document 3 (optionnel)

        [Required]
        public string FrenchResidenceProofPath { get; set; } // Document 4

        [Required]
        public string CDIContractPath { get; set; } // Document 5

        [Required]
        public string TaxWithholdingCertificatePath { get; set; } // Document 6

        [Required]
        public string SelectedAgency { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}