using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Agence
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string CodeAgence { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string CodeDR { get; set; } = string.Empty;
    public string DR { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;
    public string Gouvernerat { get; set; } = string.Empty;
    public string AdresseEmail { get; set; } = string.Empty;
    public string addresse { get; set; } = string.Empty;
    public string tel1 { get; set; } = string.Empty;
    public string tel2 { get; set; } = string.Empty;
    public string fax { get; set; } = string.Empty;
    public string gsm { get; set; } = string.Empty;
    public string localisation { get; set; } = string.Empty;
}
