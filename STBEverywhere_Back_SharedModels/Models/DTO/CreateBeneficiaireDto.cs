using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace STBEverywhere_Back_SharedModels.Models.DTO
{
    public class CreateBeneficiaireDto
    {
        public string Nom { get; set; }

        public string Prenom { get; set; }


        [Required(ErrorMessage = "Le RIB du compte est obligatoire.")]
        public string RIBCompte { get; set; }

        public string Telephone { get; set; }

        [EmailAddress(ErrorMessage = "L'email doit être une adresse email valide.")]
        public string Email { get; set; }

       
    }
}

