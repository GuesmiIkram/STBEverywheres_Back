using STBEverywhere_Back_SharedModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class NotificationPack
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; }

    public bool IsRead { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Défaut dans le code

    [Required]
    [MaxLength(50)]
    public string NotificationType { get; set; }

    public int? RelatedPackId { get; set; }

    [ForeignKey("ClientId")]
    [JsonIgnore]
    public virtual Client Client { get; set; }
}