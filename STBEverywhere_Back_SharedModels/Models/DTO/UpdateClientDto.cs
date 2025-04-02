using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class UpdateClientDto
    {
        public string Telephone { get; set; }

        public string Email { get; set; }

        public string Adresse { get; set; }
        public string Civilite { get; set; }
        public string EtatCivil { get; set; }
        public string Residence { get; set; }
        public string SituationProfessionnelle { get; set; }
        public string NiveauEducation { get; set; }

        [Range(0, 20, ErrorMessage = "Le nombre d'enfants doit être entre 0 et 20")]
        public int NombreEnfants { get; set; }
        public decimal RevenuMensuel { get; set; }

    }
}
