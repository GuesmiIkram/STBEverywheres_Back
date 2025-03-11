using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_Back_SharedModels.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Définir les DbSets pour toutes les entités
        public DbSet<Client> Clients { get; set; }
        public DbSet<Compte> Comptes { get; set; }
        public DbSet<Virement> Virements { get; set; }
        public DbSet<Carte> Cartes { get; set; }
        public DbSet<DemandeCarte> DemandesCarte { get; set; }
        public DbSet<DemandeChequier> DemandesChequiers { get; set; }
        public DbSet<Chequier> Chequiers { get; set; }
        public DbSet<FeuilleChequier> FeuillesChequiers { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de l'entité Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nom).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Telephone).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Adresse).IsRequired().HasMaxLength(200);
                entity.Property(c => c.MotDePasse).IsRequired().HasMaxLength(100);

                // Relation One-to-Many : Un client peut avoir plusieurs demandes de carte
                entity.HasMany(c => c.DemandesCarte)
                      .WithOne(d => d.Client)
                      .HasForeignKey(d => d.ClientId);

                // Ajouter des données initiales pour les clients
                entity.HasData(
                    new Client
                    {
                        Id = 1,
                        Nom = "Doe",
                        Prenom = "John",
                        DateNaissance = new DateTime(1980, 1, 1),
                        Telephone = "123456789",
                        Email = "john.doe@example.com",
                        Adresse = "123 Main St",
                        Civilite = "M",
                        Nationalite = "US",
                        EtatCivil = "Célibataire",
                        Residence = "New York",
                        NumCIN = "14668061",
                        MotDePasse = BCrypt.Net.BCrypt.HashPassword("password123")
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
                        MotDePasse = BCrypt.Net.BCrypt.HashPassword("password456")
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

                // Ajouter des données initiales pour les comptes
                entity.HasData(
                    new Compte
                    {
                        RIB = "12345678923537902652",
                        NumCin = "14668061",
                        Type = "Courant",
                        Solde = 1000.50m,
                        DateCreation = new DateTime(2024, 1, 1),
                        Statut = "Actif",
                        ClientId = 1
                    },
                    new Compte
                    {
                        RIB = "65432110223463790345",
                        NumCin = "14668062",
                        Type = "Épargne",
                        Solde = 5000.00m,
                        DateCreation = new DateTime(2024, 1, 1),
                        Statut = "Actif",
                        ClientId = 1
                    }
                );
            });

            // Configuration de l'entité Carte
            modelBuilder.Entity<Carte>(entity =>
            {
                entity.HasKey(c => c.NumCarte);
                entity.Property(c => c.NomCarte).IsRequired().HasMaxLength(50);
                entity.Property(c => c.TypeCarte).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Statut).IsRequired().HasMaxLength(20);

                // Ajouter des données initiales pour les cartes
                entity.HasData(
                    new Carte
                    {
                        NumCarte = "1111222233334444",
                        NomCarte = "Visa",
                        TypeCarte = "International",
                        DateCreation = new DateTime(2024, 1, 1),
                        DateExpiration = new DateTime(2027, 1, 1),
                        Statut = "Active",
                        Iddemande = 1,
                        CodeCVV = "",
                        Plafond = 1000,
                        CodePIN = "",
                        RIB = "12345678923537902652" // RIB du compte associé
                    },
                    new Carte
                    {
                        NumCarte = "5555666677778888",
                        NomCarte = "Mastercard",
                        TypeCarte = "National",
                        DateCreation = new DateTime(2024, 1, 1),
                        DateExpiration = new DateTime(2027, 1, 1),
                        Statut = "active",
                        Iddemande = 2,
                        CodeCVV = "",
                        CodePIN = "",
                        Plafond=1000,
                        RIB = "65432110223463790345" // RIB du compte associé
                    }
                );
            });

            // Configuration de l'entité DemandeCarte
            modelBuilder.Entity<DemandeCarte>(entity =>
            {
                entity.HasKey(d => d.Iddemande); // Définir Iddemande comme clé primaire
                entity.Property(d => d.NumCompte).IsRequired().HasMaxLength(20);
                entity.Property(d => d.NomCarte).IsRequired().HasMaxLength(50);
                entity.Property(d => d.TypeCarte).IsRequired().HasMaxLength(20);
                entity.Property(d => d.CIN).IsRequired().HasMaxLength(20);
                entity.Property(d => d.Email).IsRequired().HasMaxLength(100);
                entity.Property(d => d.NumTel).IsRequired().HasMaxLength(20);
                entity.Property(d => d.Statut).IsRequired().HasMaxLength(20);

                // Relation Many-to-One : Une demande de carte est associée à un client
                entity.HasOne(d => d.Client)
                      .WithMany(c => c.DemandesCarte)
                      .HasForeignKey(d => d.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ajouter des données initiales pour les demandes de carte
                entity.HasData(
                    new DemandeCarte
                    {
                        Iddemande = 1,
                        NumCompte = "12345678923537902652",
                        NomCarte = "Visa",
                        TypeCarte = "International",
                        CIN = "14668061",
                        Email = "john.doe@example.com",
                        NumTel = "12345678",
                        Statut = "DisponibleAgence",
                        EmailEnvoye = false,
                        EmailEnvoyeLivree = false,
                        CarteAjouter = false,
                        ClientId = 1
                    },
                    new DemandeCarte
                    {
                        Iddemande = 2,
                        NumCompte = "65432110223463790345",
                        NomCarte = "Mastercard",
                        TypeCarte = "National",
                        CIN = "14668062",
                        Email = "jane.smith@example.com",
                        NumTel = "87654321",
                        Statut = "DisponibleAgence",
                        EmailEnvoye = false,
                        EmailEnvoyeLivree = false,
                        CarteAjouter =false,
                        ClientId = 2
                    }
                );
            }
            );

            // Configuration de l'entité Virement
            modelBuilder.Entity<Virement>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasIndex(v => new { v.RIB_Emetteur, v.DateVirement }).IsUnique();
            });
        }
    }
}