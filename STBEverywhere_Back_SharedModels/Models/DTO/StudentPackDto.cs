using System.ComponentModel.DataAnnotations;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class StudentPackDto
    {

        public IFormFile Document1 { get; set; }
        public IFormFile Document2 { get; set; }
        public IFormFile Document3 { get; set; }
        public IFormFile Document4 { get; set; }
        public IFormFile? Document5 { get; set; }
        public string Agency { get; set; }

    }
}