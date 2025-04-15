using System.Text.Json.Serialization;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    /*public class ReponseReclamationAgentDto
    {
        public int Id { get; set; }
        public string ContenuReponse { get; set; }
    }*/


    public class ReponseReclamationAgentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("contenuReponse")]
        public string ContenuReponse { get; set; }
    }

}
