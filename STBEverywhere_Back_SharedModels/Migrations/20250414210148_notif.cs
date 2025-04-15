﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class notif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Departement = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    AgenceId = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
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
                    AgenceId = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Beneficiaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RIBCompte = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telephone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaires_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comptes",
                columns: table => new
                {
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IBAN = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Solde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumCin = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DecouvertAutorise = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                name: "NotificationsPack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotificationType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RelatedPackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsPack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsPack_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PackElyssa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassportPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LongStayVisaPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VisaRegistrationPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FrenchResidenceProofPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CDIContractPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxWithholdingCertificatePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SelectedAgency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubmissionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackElyssa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackElyssa_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PackStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PassportPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InscriptionPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BoursePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DomicileTunisiePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DomicileFrancePath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SelectedAgency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubmissionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackStudents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reclamations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Objet = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateResolution = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DemandeModificationDecouverts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RIBCompte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DecouvertDemande = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    StatutDemande = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDemande = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MotifRefus = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdAgentRepondant = table.Column<int>(type: "int", nullable: true),
                    NotificationEnvoyee = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MailEnvoyee = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandeModificationDecouverts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandeModificationDecouverts_Comptes_RIBCompte",
                        column: x => x.RIBCompte,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DemandesCarte",
                columns: table => new
                {
                    Iddemande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RIB = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
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
                    EmailEnvoye = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EmailEnvoyeLivree = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CarteAjouter = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesCarte", x => x.Iddemande);
                    table.ForeignKey(
                        name: "FK_DemandesCarte_Comptes_RIB",
                        column: x => x.RIB,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
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
                name: "FraisComptes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IdsVirementsStr = table.Column<string>(type: "longtext", nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraisComptes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraisComptes_Comptes_RIB",
                        column: x => x.RIB,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PeriodeDecouverts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MontantMaxDecouvert = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SoldeInitial = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodeDecouverts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodeDecouverts_Comptes_RIB",
                        column: x => x.RIB,
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
                    PlafondDAP = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CompteRIB = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartes", x => x.NumCarte);
                    table.ForeignKey(
                        name: "FK_Cartes_Comptes_CompteRIB",
                        column: x => x.CompteRIB,
                        principalTable: "Comptes",
                        principalColumn: "RIB");
                    table.ForeignKey(
                        name: "FK_Cartes_Comptes_RIB",
                        column: x => x.RIB,
                        principalTable: "Comptes",
                        principalColumn: "RIB",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "DemandesAugmentationPlafond",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumCarte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NouveauPlafondTPE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NouveauPlafondDAB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateDemande = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Raison = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateTraitement = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Commentaire = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CarteNumCarte = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesAugmentationPlafond", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesAugmentationPlafond_Cartes_CarteNumCarte",
                        column: x => x.CarteNumCarte,
                        principalTable: "Cartes",
                        principalColumn: "NumCarte");
                    table.ForeignKey(
                        name: "FK_DemandesAugmentationPlafond_Cartes_NumCarte",
                        column: x => x.NumCarte,
                        principalTable: "Cartes",
                        principalColumn: "NumCarte",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FraisCartes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumCarte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraisCartes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraisCartes_Cartes_NumCarte",
                        column: x => x.NumCarte,
                        principalTable: "Cartes",
                        principalColumn: "NumCarte",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RechargesCarte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CarteEmetteurNum = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CarteRecepteurNum = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Montant = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Frais = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateRecharge = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RechargesCarte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RechargesCarte_Cartes_CarteEmetteurNum",
                        column: x => x.CarteEmetteurNum,
                        principalTable: "Cartes",
                        principalColumn: "NumCarte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RechargesCarte_Cartes_CarteRecepteurNum",
                        column: x => x.CarteRecepteurNum,
                        principalTable: "Cartes",
                        principalColumn: "NumCarte",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "PasswordHash", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role" },
                values: new object[,]
                {
                    { 1, "guesmiimahmoud@gmail.com", true, "$2a$11$.dtHNNkJzHN7NmQRwrnCCuS0ckKHlS.2WaLyUNpqzFErhlyDWH0W.", null, null, "Client" },
                    { 2, "jane.smith@example.com", true, "$2a$11$1AiyCnoZ3zT3QpZ/28kuEOWijt.UDuNQFmyC2F8fqeLK3VLiYc45q", null, null, "Client" },
                    { 3, "agent@stb.com", true, "$2a$11$d5iwJIRq5EYbQwW7O.FBnu46x.ehFYLWc4BfQG49uH7PphY/09niW", null, null, "Agent" },
                    { 4, "robert.smith@example.com", true, "$2a$11$qn3MxQEodcvaQQdB5kJV0uYHt4nc88kivC7j2/ihdU/WopdU8JoA2", null, null, "Client" },
                    { 5, "agent5@stb.com", true, "$2a$11$niuTT3Y.DQYENpplvoeTgOuAfbXfDL3hoVkDTuPEuN12jsGRE3E4u", null, null, "Agent" }
                });

            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "AgenceId", "Departement", "Nom", "Prenom", "UserId" },
                values: new object[,]
                {
                    { 1, "67f83774f6176b4e97078b05", "Administration", "Admin", "STB", 3 },
                    { 2, "67f83774f6176b4e97078b06", "Administration", "Admin5", "STB5", 5 }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Adresse", "AgenceId", "Civilite", "DateDelivranceCIN", "DateExpirationCIN", "DateNaissance", "Email", "EtatCivil", "Genre", "LieuDelivranceCIN", "Nationalite", "NiveauEducation", "Nom", "NomMere", "NomPere", "NombreEnfants", "NumCIN", "PaysNaissance", "PhotoClient", "Prenom", "Profession", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Residence", "RevenuMensuel", "SituationProfessionnelle", "Telephone", "UserId" },
                values: new object[,]
                {
                    { 1, "123 Main St", "67f83774f6176b4e97078b05", "M", new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "guesmiimahmoud@gmail.com", "Célibataire", "Masculin", "New York", "US", "Master", "Doe", "Jane Doe", "John Doe Sr.", 2, "14668061", "USA", "mahmoud.jpg", "John", "Ingénieur", null, null, "New York", 5000.00m, "Employé", "123456789", 1 },
                    { 2, "456 Elm St", "67f83774f6176b4e97078b06", "Mme", new DateTime(2015, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2035, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Marié(e)", "Féminin", "Toronto", "CA", "Doctorat", "Smith", "Mary Smith", "Robert Smith", 1, "14668062", "Canada", "mahmoud.jpg", "Jane", "Médecin", null, null, "Toronto", 7000.00m, "Indépendant", "987654321", 2 },
                    { 4, "456 ben arous", "67f6461d3d6e3c7fa3ef47ae", "Mme", new DateTime(2013, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2035, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "robert.smith@example.com", "Marié(e)", "Masculin", "Toronto", "TN", "Doctorat", "robert", "Mary Smith", "Robert Smith", 1, "19668067", "Canada", "mahmoud.jpg", "smith", "Médecin", null, null, "Tunis", 2000.00m, "Indépendant", "997654321", 4 }
                });

            migrationBuilder.InsertData(
                table: "Comptes",
                columns: new[] { "RIB", "ClientId", "DateCreation", "DecouvertAutorise", "IBAN", "MontantMaxAutoriseParJour", "NbrOperationsAutoriseesParJour", "NumCin", "Solde", "Statut", "Type" },
                values: new object[,]
                {
                    { "12345678923537902652", 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "TN2110500678923537952", 0m, null, "14668061", 1000.50m, "Actif", "Courant" },
                    { "65432110223463790345", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, "TN1210500110223463745", 0m, null, "14668062", 5000.00m, "Actif", "Epargne" }
                });

            migrationBuilder.InsertData(
                table: "DemandesCarte",
                columns: new[] { "Iddemande", "CIN", "CarteAjouter", "DateCreation", "Email", "EmailEnvoye", "EmailEnvoyeLivree", "NomCarte", "RIB", "NumTel", "Statut", "TypeCarte" },
                values: new object[,]
                {
                    { 1, "14668061", false, new DateTime(2025, 4, 14, 22, 1, 47, 524, DateTimeKind.Local).AddTicks(2113), "john.doe@example.com", false, false, "VisaClassic", "12345678923537902652", "12345678", "DisponibleEnAgence", "International" },
                    { 2, "14668062", false, new DateTime(2025, 4, 14, 22, 1, 47, 524, DateTimeKind.Local).AddTicks(2229), "jane.smith@example.com", false, false, "Mastercard", "65432110223463790345", "87654321", "EnPreparation", "National" }
                });

            migrationBuilder.InsertData(
                table: "Cartes",
                columns: new[] { "NumCarte", "CodeCVV", "CodePIN", "CompteRIB", "DateCreation", "DateExpiration", "DateRecuperation", "Iddemande", "Nature", "NomCarte", "PlafondDAP", "PlafondTPE", "RIB", "Solde", "Statut", "TypeCarte" },
                values: new object[,]
                {
                    { "1111222233334444", "", "", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "postpayee", "VisaClassic", 20000m, 40000m, "12345678923537902652", 1000.50m, "Active", "International" },
                    { "5555666677778888", "", "", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "postpayee", "Mastercard", 20000m, 40000m, "65432110223463790345", 5000.00m, "Active", "National" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_UserId",
                table: "Agents",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaires_ClientId",
                table: "Beneficiaires",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Cartes_CompteRIB",
                table: "Cartes",
                column: "CompteRIB");

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
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_ClientId",
                table: "Comptes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandeModificationDecouverts_RIBCompte",
                table: "DemandeModificationDecouverts",
                column: "RIBCompte");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesAugmentationPlafond_CarteNumCarte",
                table: "DemandesAugmentationPlafond",
                column: "CarteNumCarte");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesAugmentationPlafond_NumCarte",
                table: "DemandesAugmentationPlafond",
                column: "NumCarte");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesCarte_RIB",
                table: "DemandesCarte",
                column: "RIB");

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
                name: "IX_FraisCartes_NumCarte",
                table: "FraisCartes",
                column: "NumCarte");

            migrationBuilder.CreateIndex(
                name: "IX_FraisComptes_RIB",
                table: "FraisComptes",
                column: "RIB");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsPack_ClientId",
                table: "NotificationsPack",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PackElyssa_ClientId",
                table: "PackElyssa",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PackStudents_ClientId",
                table: "PackStudents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodeDecouverts_RIB",
                table: "PeriodeDecouverts",
                column: "RIB");

            migrationBuilder.CreateIndex(
                name: "IX_RechargesCarte_CarteEmetteurNum",
                table: "RechargesCarte",
                column: "CarteEmetteurNum");

            migrationBuilder.CreateIndex(
                name: "IX_RechargesCarte_CarteRecepteurNum",
                table: "RechargesCarte",
                column: "CarteRecepteurNum");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_ClientId",
                table: "Reclamations",
                column: "ClientId");

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
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Beneficiaires");

            migrationBuilder.DropTable(
                name: "Chequiers");

            migrationBuilder.DropTable(
                name: "DemandeModificationDecouverts");

            migrationBuilder.DropTable(
                name: "DemandesAugmentationPlafond");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "FeuillesChequiers");

            migrationBuilder.DropTable(
                name: "FraisCartes");

            migrationBuilder.DropTable(
                name: "FraisComptes");

            migrationBuilder.DropTable(
                name: "NotificationsPack");

            migrationBuilder.DropTable(
                name: "PackElyssa");

            migrationBuilder.DropTable(
                name: "PackStudents");

            migrationBuilder.DropTable(
                name: "PeriodeDecouverts");

            migrationBuilder.DropTable(
                name: "RechargesCarte");

            migrationBuilder.DropTable(
                name: "Reclamations");

            migrationBuilder.DropTable(
                name: "Virements");

            migrationBuilder.DropTable(
                name: "DemandesChequiers");

            migrationBuilder.DropTable(
                name: "Cartes");

            migrationBuilder.DropTable(
                name: "DemandesCarte");

            migrationBuilder.DropTable(
                name: "Comptes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
