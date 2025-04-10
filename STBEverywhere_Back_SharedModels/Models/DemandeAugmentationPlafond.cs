using STBEverywhere_Back_SharedModels.Models.enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace STBEverywhere_Back_SharedModels.Models
{
    public class DemandeAugmentationPlafond
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string NumCarte { get; set; }

        [ForeignKey("NumCarte")]
        [JsonIgnore]
        public Carte Carte { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal NouveauPlafondTPE { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal NouveauPlafondDAB { get; set; }

        [Required]
        public DateTime DateDemande { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "varchar(20)")] // Change le type en varchar
        public string Statut { get; set; } = StatutDemandeAug.EnAttente.ToString();

        public string Raison { get; set; }

        public DateTime? DateTraitement { get; set; }

        [MaxLength(500)]
        public string? Commentaire { get; set; } // Nullable
    }
}
public enum StatutDemandeAug
{
    EnAttente,
    Approuvee,
    Rejetee,
   
}
