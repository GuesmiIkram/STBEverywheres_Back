﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateNaissance = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Telephone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresse = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Civilite = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nationalite = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EtatCivil = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Residence = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumCIN = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDelivranceCIN = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DateExpirationCIN = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LieuDelivranceCIN = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhotoClient = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotDePasse = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetPasswordToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Genre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Profession = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SituationProfessionnelle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NiveauEducation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreEnfants = table.Column<int>(type: "int", nullable: false),
                    RevenuMensuel = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PaysNaissance = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomMere = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomPere = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Virements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RIB_Emetteur = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RIB_Recepteur = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Montant = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Motif = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateVirement = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeVirement = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FichierBeneficaires = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Virements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comptes",
                columns: table => new
                {
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Solde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumCin = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NbrOperationsAutoriseesParJour = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontantMaxAutoriseParJour = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comptes", x => x.RIB);
                    table.ForeignKey(
                        name: "FK_Comptes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DemandesCarte",
                columns: table => new
                {
                    Iddemande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumCompte = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomCarte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeCarte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CIN = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumTel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    EmailEnvoye = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EmailEnvoyeLivree = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CarteAjouter = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesCarte", x => x.Iddemande);
                    table.ForeignKey(
                        name: "FK_DemandesCarte_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DemandesChequiers",
                columns: table => new
                {
                    IdDemande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RibCompte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDemande = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NombreFeuilles = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Otp = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModeLivraison = table.Column<int>(type: "int", nullable: false),
                    Agence = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdresseComplete = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodePostal = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumTel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroChequier = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlafondChequier = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    RaisonDemande = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccepteEngagement = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    isBarre = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesChequiers", x => x.IdDemande);
                    table.ForeignKey(
                        name: "FK_DemandesChequiers_Comptes_RibCompte",
                        column: x => x.RibCompte,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cartes",
                columns: table => new
                {
                    NumCarte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomCarte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeCarte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nature = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iddemande = table.Column<int>(type: "int", nullable: false),
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Solde = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    DateRecuperation = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CodePIN = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodeCVV = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlafondTPE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlafondDAP = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartes", x => x.NumCarte);
                    table.ForeignKey(
                        name: "FK_Cartes_Comptes_RIB",
                        column: x => x.RIB,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cartes_DemandesCarte_Iddemande",
                        column: x => x.Iddemande,
                        principalTable: "DemandesCarte",
                        principalColumn: "Iddemande",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Chequiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemandeChequierId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateLivraison = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chequiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chequiers_DemandesChequiers_DemandeChequierId",
                        column: x => x.DemandeChequierId,
                        principalTable: "DemandesChequiers",
                        principalColumn: "IdDemande",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    Contenu = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Destinataire = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sujet = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateEnvoi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsEnvoye = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_DemandesChequiers_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "DemandesChequiers",
                        principalColumn: "IdDemande",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeuillesChequiers",
                columns: table => new
                {
                    IdFeuille = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroFeuille = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlafondFeuille = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    DemandeChequierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeuillesChequiers", x => x.IdFeuille);
                    table.ForeignKey(
                        name: "FK_FeuillesChequiers_DemandesChequiers_DemandeChequierId",
                        column: x => x.DemandeChequierId,
                        principalTable: "DemandesChequiers",
                        principalColumn: "IdDemande",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Adresse", "Civilite", "DateDelivranceCIN", "DateExpirationCIN", "DateNaissance", "Email", "EtatCivil", "Genre", "LieuDelivranceCIN", "MotDePasse", "Nationalite", "NiveauEducation", "Nom", "NomMere", "NomPere", "NombreEnfants", "NumCIN", "PaysNaissance", "PhotoClient", "Prenom", "Profession", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Residence", "RevenuMensuel", "SituationProfessionnelle", "Telephone" },
                values: new object[,]
                {
                    { 1, "123 Main St", "M", new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "guesmiimahmoud@gmail.com", "Célibataire", "Masculin", "New York", "$2a$11$xl3X1Y3aAYL/DxIoSjAZdewfDrw/8APZBm/BhH6qATPCWl.N0n9uu", "US", "Master", "Doe", "Jane Doe", "John Doe Sr.", 2, "14668061", "USA", "mahmoud.jpg", "John", "Ingénieur", null, null, "New York", 5000.00m, "Employé", "123456789" },
                    { 2, "456 Elm St", "Mme", new DateTime(2015, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2035, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Marié(e)", "Féminin", "Toronto", "$2a$11$I4ZwYpa1rs36aiUyiRjgpu7PjTLyAixY73bat3JGFkBoGjWh985WO", "CA", "Doctorat", "Smith", "Mary Smith", "Robert Smith", 1, "14668062", "Canada", "mahmoud.jpg", "Jane", "Médecin", null, null, "Toronto", 7000.00m, "Indépendant", "987654321" }
                });

            migrationBuilder.InsertData(
                table: "Comptes",
                columns: new[] { "RIB", "ClientId", "DateCreation", "MontantMaxAutoriseParJour", "NbrOperationsAutoriseesParJour", "NumCin", "Solde", "Statut", "Type" },
                values: new object[,]
                {
                    { "12345678923537902652", 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, null, "14668061", 1000.50m, "Actif", "Courant" },
                    { "65432110223463790345", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, null, "14668062", 5000.00m, "Actif", "Epargne" }
                });

            migrationBuilder.InsertData(
                table: "DemandesCarte",
                columns: new[] { "Iddemande", "CIN", "CarteAjouter", "ClientId", "DateCreation", "Email", "EmailEnvoye", "EmailEnvoyeLivree", "NomCarte", "NumCompte", "NumTel", "Statut", "TypeCarte" },
                values: new object[,]
                {
                    { 1, "14668061", false, 1, new DateTime(2025, 3, 20, 9, 47, 11, 999, DateTimeKind.Local).AddTicks(9813), "john.doe@example.com", false, false, "VisaClassic", "12345678923537902652", "12345678", "DisponibleEnAgence", "International" },
                    { 2, "14668062", false, 2, new DateTime(2025, 3, 20, 9, 47, 11, 999, DateTimeKind.Local).AddTicks(9949), "jane.smith@example.com", false, false, "Mastercard", "65432110223463790345", "87654321", "EnPreparation", "National" }
                });

            migrationBuilder.InsertData(
                table: "Cartes",
                columns: new[] { "NumCarte", "CodeCVV", "CodePIN", "DateCreation", "DateExpiration", "DateRecuperation", "Iddemande", "Nature", "NomCarte", "PlafondDAP", "PlafondTPE", "RIB", "Solde", "Statut", "TypeCarte" },
                values: new object[,]
                {
                    { "1111222233334444", "", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "postpayée", "VisaClassic", 20000m, 40000m, "12345678923537902652", 1000.50m, "Active", "International" },
                    { "5555666677778888", "", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "postpayée", "Mastercard", 20000m, 40000m, "65432110223463790345", 5000.00m, "Active", "National" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cartes_Iddemande",
                table: "Cartes",
                column: "Iddemande");

            migrationBuilder.CreateIndex(
                name: "IX_Cartes_RIB",
                table: "Cartes",
                column: "RIB");

            migrationBuilder.CreateIndex(
                name: "IX_Chequiers_DemandeChequierId",
                table: "Chequiers",
                column: "DemandeChequierId");

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_ClientId",
                table: "Comptes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesCarte_ClientId",
                table: "DemandesCarte",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesChequiers_RibCompte",
                table: "DemandesChequiers",
                column: "RibCompte");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_DemandeId",
                table: "EmailLogs",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeuillesChequiers_DemandeChequierId",
                table: "FeuillesChequiers",
                column: "DemandeChequierId");

            migrationBuilder.CreateIndex(
                name: "IX_Virements_RIB_Emetteur_DateVirement",
                table: "Virements",
                columns: new[] { "RIB_Emetteur", "DateVirement" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartes");

            migrationBuilder.DropTable(
                name: "Chequiers");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "FeuillesChequiers");

            migrationBuilder.DropTable(
                name: "Virements");

            migrationBuilder.DropTable(
                name: "DemandesCarte");

            migrationBuilder.DropTable(
                name: "DemandesChequiers");

            migrationBuilder.DropTable(
                name: "Comptes");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
