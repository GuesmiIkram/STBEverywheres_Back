using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class AddCartesSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemandesCarte",
                columns: table => new
                {
                    Iddemande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumCompte = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomCarte = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeCarte = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CIN = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumTel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesCarte", x => x.Iddemande);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cartes",
                columns: table => new
                {
                    NumCarte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NomCarte = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeCarte = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iddemande = table.Column<int>(type: "int", nullable: false),
                    RIB = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MotDePasse", "NumCIN" },
                values: new object[] { "$2a$11$oPGDoX44HnQ47Vq/d0zd..zYeB.uxRFcBkaeWgCV2dcP6hEPd45eO", "14668061" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MotDePasse", "NumCIN" },
                values: new object[] { "$2a$11$xxApJCAgsMrq7NcoFv55zuVfgPaziwC8748tOBFSNrkKiI8nMsp86", "14668062" });

            migrationBuilder.InsertData(
                table: "DemandesCarte",
                columns: new[] { "Iddemande", "CIN", "DateCreation", "Email", "Nom", "NomCarte", "NumCompte", "NumTel", "Prenom", "TypeCarte" },
                values: new object[,]
                {
                    { 1, "14668061", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "Doe", "Visa", "12345678923537902652", "12345678", "John", "International" },
                    { 2, "14668062", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Smith", "Mastercard", "65432110223463790345", "87654321", "Jane", "National" }
                });

            migrationBuilder.InsertData(
                table: "Cartes",
                columns: new[] { "NumCarte", "DateCreation", "DateExpiration", "Iddemande", "NomCarte", "RIB", "Statut", "TypeCarte" },
                values: new object[,]
                {
                    { "1111222233334444", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Visa", "12345678923537902652", "Livrée", "International" },
                    { "5555666677778888", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Mastercard", "65432110223463790345", "En cours", "National" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cartes_Iddemande",
                table: "Cartes",
                column: "Iddemande");

            migrationBuilder.CreateIndex(
                name: "IX_Cartes_RIB",
                table: "Cartes",
                column: "RIB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartes");

            migrationBuilder.DropTable(
                name: "DemandesCarte");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MotDePasse", "NumCIN" },
                values: new object[] { "$2a$11$w9DbSsI29rICLWuv2NDByeMnsUAvM26SLYQfdxbbzNRL9xlpf.NhC", "A123456" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MotDePasse", "NumCIN" },
                values: new object[] { "$2a$11$rF8XdGL8nMFDQjiM3xD0Xuj4Vu2cDc8qQP8WufcwJqUSB4ceEzGTC", "B654321" });
        }
    }
}
