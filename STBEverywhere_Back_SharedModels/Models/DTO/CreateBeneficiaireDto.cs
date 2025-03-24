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
        [Required(ErrorMessage = "Le nom est obligatoire pour une personne physique.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire pour une personne physique.")]
        public string Prenom { get; set; }

        // Required for Personne Morale
        [Required(ErrorMessage = "La raison sociale est obligatoire pour une personne morale.")]
        public string RaisonSociale { get; set; }

        // Required for all beneficiary types
        [Required(ErrorMessage = "Le RIB du compte est obligatoire.")]
        public string RIBCompte { get; set; }

        public string Telephone { get; set; }

        [EmailAddress(ErrorMessage = "L'email doit être une adresse email valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le type de bénéficiaire est obligatoire.")]
        public BeneficiaireType Type { get; set; }
    }
}

