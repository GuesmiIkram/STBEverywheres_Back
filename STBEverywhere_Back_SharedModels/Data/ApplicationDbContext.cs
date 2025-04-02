using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using STBEverywhere_Back_SharedModels.Models;
using STBEverywhere_Back_SharedModels.Models.enums;

namespace STBEverywhere_Back_SharedModels.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Compte> Comptes { get; set; }
        public DbSet<Virement> Virements { get; set; }
        public DbSet<Carte> Cartes { get; set; }
        public DbSet<DemandeCarte> DemandesCarte { get; set; }
        public DbSet<DemandeChequier> DemandesChequiers { get; set; }
        public DbSet<Chequier> Chequiers { get; set; }
        public DbSet<FeuilleChequier> FeuillesChequiers { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<Beneficiaire> Beneficiaires { get; set; }

        public DbSet<PackStudent> PackStudents { get; set; }
        public DbSet<PackElyssa> PackElyssa { get; set; }

        public DbSet<FraisCompte> FraisComptes { get; set; }
        public DbSet<PeriodeDecouvert> PeriodeDecouverts { get; set; }
        public DbSet<DemandeModificationDecouvert> DemandeModificationDecouverts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'entité User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasConversion<string>();
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.ResetPasswordToken).HasMaxLength(255);
                entity.Property(u => u.ResetPasswordTokenExpiry);

                // Données initiales
                entity.HasData(
                    new User { Id = 1, Email = "guesmiimahmoud@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"), Role = UserRole.Client },
                    new User { Id = 2, Email = "jane.smith@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password456"), Role = UserRole.Client },
                    new User { Id = 3, Email = "agent@stb.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("agent123"), Role = UserRole.Agent }
                );
            });

            // Configuration de l'entité Agent
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nom).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Departement).HasMaxLength(100);

                // Remove the IsRequired(false) since the property is now nullable by type
                // entity.Property(a => a.UserId).IsRequired(false); // No longer needed

                // Relation with User
                entity.HasOne(a => a.User)
                    .WithOne()
                    .HasForeignKey<Agent>(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasData(
                        new Agent { Id = 1, Nom = "Admin", Prenom = "STB", Departement = "Administration", UserId = 3 }
                    );
            });

            // Configuration de l'entité Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nom).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Telephone).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Adresse).IsRequired().HasMaxLength(200);
                entity.Property(c => c.UserId).IsRequired(false);

                // Relation avec User (sans navigation inverse)
                entity.HasOne(c => c.User)
                    .WithOne()
                    .HasForeignKey<Client>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                // Relations One-to-Many
                entity.HasMany(c => c.Comptes)
                    .WithOne(c => c.Client)
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.DemandesCarte)
                    .WithOne(d => d.Client)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Données initiales
                entity.HasData(
                    new Client
                    {
                        Id = 1,
                        Nom = "Doe",
                        Prenom = "John",
                        DateNaissance = new DateTime(1980, 1, 1),
                        Telephone = "123456789",
                        Email = "guesmiimahmoud@gmail.com",
                        Adresse = "123 Main St",
                        Civilite = "M",
                        Nationalite = "US",
                        EtatCivil = "Célibataire",
                        Residence = "New York",
                        NumCIN = "14668061",
                        DateDelivranceCIN = new DateTime(2010, 1, 1),
                        DateExpirationCIN = new DateTime(2030, 1, 1),
                        LieuDelivranceCIN = "New York",
                        PhotoClient = "mahmoud.jpg",
                        Genre = "Masculin",
                        Profession = "Ingénieur",
                        SituationProfessionnelle = "Employé",
                        NiveauEducation = "Master",
                        NombreEnfants = 2,
                        RevenuMensuel = 5000.00m,
                        PaysNaissance = "USA",
                        NomMere = "Jane Doe",
                        NomPere = "John Doe Sr.",
                        UserId = 1
                    },
                    new Client
                    {
                        Id = 2,
                        Nom = "Smith",
                        Prenom = "Jane",
                        DateNaissance = new DateTime(1990, 5, 15),
                        Telephone = "987654321",
                        Email = "jane.smith@example.com",
                        Adresse = "456 Elm St",
                        Civilite = "Mme",
                        Nationalite = "CA",
                        EtatCivil = "Marié(e)",
                        Residence = "Toronto",
                        NumCIN = "14668062",
                        DateDelivranceCIN = new DateTime(2015, 5, 15),
                        DateExpirationCIN = new DateTime(2035, 5, 15),
                        LieuDelivranceCIN = "Toronto",
                        PhotoClient = "mahmoud.jpg",
                        Genre = "Féminin",
                        Profession = "Médecin",
                        SituationProfessionnelle = "Indépendant",
                        NiveauEducation = "Doctorat",
                        NombreEnfants = 1,
                        RevenuMensuel = 7000.00m,
                        PaysNaissance = "Canada",
                        NomMere = "Mary Smith",
                        NomPere = "Robert Smith",
                        UserId = 2
                    }
                );
            });

            // Configuration de l'entité Compte
            modelBuilder.Entity<Compte>(entity =>
            {
                entity.HasKey(c => c.RIB);
                entity.Property(c => c.Type).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Solde).HasColumnType("decimal(18,3)");
                entity.Property(c => c.Statut).HasMaxLength(20);

                entity.HasData(
                    new Compte
                    {
                        RIB = "12345678923537902652",
                        NumCin = "14668061",
                        Type = "Courant",
                        Solde = 1000.50m,
                        DateCreation = new DateTime(2024, 5, 1),
                        Statut = "Actif",
                        IBAN = "TN2110500678923537952",
                        ClientId = 1
                    },
                    new Compte
                    {
                        RIB = "65432110223463790345",
                        NumCin = "14668062",
                        Type = "Epargne",
                        Solde = 5000.00m,
                        DateCreation = new DateTime(2025, 1, 1),
                        Statut = "Actif",
                        IBAN = "TN1210500110223463745",
                        ClientId = 2
                    }
                );
            });

            // Configuration de l'entité Carte
            modelBuilder.Entity<Carte>(entity =>
            {
                entity.HasKey(c => c.NumCarte);
                entity.Property(c => c.NomCarte).HasConversion<string>();
                entity.Property(c => c.TypeCarte).HasConversion<string>();
                entity.Property(c => c.Statut).HasConversion<string>();

                // Relation avec Compte
                entity.HasOne(c => c.Compte)
                    .WithMany()
                    .HasForeignKey(c => c.RIB)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasData(
                    new Carte
                    {
                        NumCarte = "1111222233334444",
                        NomCarte = NomCarte.VisaClassic,
                        TypeCarte = TypeCarte.International,
                        DateCreation = new DateTime(2024, 1, 1),
                        DateExpiration = new DateTime(2027, 1, 1),
                        Statut = StatutCarte.Active,
                        Iddemande = 1,
                        CodeCVV = "",
                        Nature = "postpayée",
                        PlafondTPE = 40000,
                        PlafondDAP = 20000,
                        Solde = 1000.50m,
                        CodePIN = "",
                        RIB = "12345678923537902652"
                    },
                    new Carte
                    {
                        NumCarte = "5555666677778888",
                        NomCarte = NomCarte.Mastercard,
                        TypeCarte = TypeCarte.National,
                        DateCreation = new DateTime(2024, 1, 1),
                        DateExpiration = new DateTime(2027, 1, 1),
                        Statut = StatutCarte.Active,
                        Iddemande = 2,
                        CodeCVV = "",
                        Solde = 5000.00m,
                        Nature = "postpayée",
                        CodePIN = "",
                        PlafondTPE = 40000,
                        PlafondDAP = 20000,
                        RIB = "65432110223463790345"
                    }
                );
            });

            // Configuration de l'entité DemandeCarte
            modelBuilder.Entity<DemandeCarte>(entity =>
            {
                entity.HasKey(d => d.Iddemande);
                entity.Property(d => d.NumCompte).IsRequired().HasMaxLength(20);
                entity.Property(d => d.NomCarte).HasConversion<string>();
                entity.Property(d => d.TypeCarte).HasConversion<string>();
                entity.Property(d => d.Statut).HasConversion<string>();
                entity.Property(d => d.CIN).IsRequired().HasMaxLength(20);
                entity.Property(d => d.Email).IsRequired().HasMaxLength(100);
                entity.Property(d => d.NumTel).IsRequired().HasMaxLength(20);

                entity.HasData(
                    new DemandeCarte
                    {
                        Iddemande = 1,
                        NumCompte = "12345678923537902652",
                        NomCarte = NomCarte.VisaClassic,
                        TypeCarte = TypeCarte.International,
                        CIN = "14668061",
                        Email = "john.doe@example.com",
                        NumTel = "12345678",
                        Statut = StatutDemande.DisponibleEnAgence,
                        EmailEnvoye = false,
                        EmailEnvoyeLivree = false,
                        CarteAjouter = false,
                        ClientId = 1
                    },
                    new DemandeCarte
                    {
                        Iddemande = 2,
                        NumCompte = "65432110223463790345",
                        NomCarte = NomCarte.Mastercard,
                        TypeCarte = TypeCarte.National,
                        CIN = "14668062",
                        Email = "jane.smith@example.com",
                        NumTel = "87654321",
                        Statut = StatutDemande.EnPreparation,
                        EmailEnvoye = false,
                        EmailEnvoyeLivree = false,
                        CarteAjouter = false,
                        ClientId = 2
                    }
                );
            });

            // Configuration de l'entité Virement
            modelBuilder.Entity<Virement>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasIndex(v => new { v.RIB_Emetteur, v.DateVirement }).IsUnique();
            });


            // le convertisseur de la liste des idvirement de l'entité FraisCompte


            modelBuilder.Entity<FraisCompte>()
        .Property(e => e.IdsVirementsStr)
        .HasDefaultValue(""); // Valeur par défaut vide




        }

       
    }
}