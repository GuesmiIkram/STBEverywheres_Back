using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class PackStudent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PassportPath { get; set; }

        [Required]
        public string InscriptionPath { get; set; }

        [Required]
        public string BoursePath { get; set; }

        [Required]
        public string DomicileTunisiePath { get; set; }

        public string? DomicileFrancePath { get; set; }

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