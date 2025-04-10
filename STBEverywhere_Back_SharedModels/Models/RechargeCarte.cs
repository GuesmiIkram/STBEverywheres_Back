

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace STBEverywhere_Back_SharedModels.Models
{
        [Table("RechargesCarte")] // Nom de la table dans la base de données
        public class RechargeCarte
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [StringLength(16)]
            [ForeignKey("CarteEmetteur")]
            public string CarteEmetteurNum { get; set; } // Numéro de la carte émettrice

            [Required]
            [StringLength(16)]
            [ForeignKey("CarteRecepteur")]
            public string CarteRecepteurNum { get; set; } // Numéro de la carte réceptrice

            [Required]
            [Column(TypeName = "decimal(18, 3)")]
            [Range(0.01, 100000)]
            public decimal Montant { get; set; } // Montant transféré (hors frais)

            [Column(TypeName = "decimal(18, 3)")]
            public decimal Frais { get; set; } // Frais de recharge (0 si même client)

            [Required]
            public DateTime DateRecharge { get; set; } = DateTime.UtcNow;

            // Navigation properties
            [JsonIgnore]
            public virtual Carte CarteEmetteur { get; set; }
            [JsonIgnore]
             public virtual Carte CarteRecepteur { get; set; }
        }
    }

