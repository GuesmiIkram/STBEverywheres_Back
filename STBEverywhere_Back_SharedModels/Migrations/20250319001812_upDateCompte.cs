using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class upDateCompte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MontantMaxAutoriseParJour",
                table: "Comptes",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NbrOperationsAutoriseesParJour",
                table: "Comptes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondTPE",
                table: "Cartes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondDAP",
                table: "Cartes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                column: "Statut",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MotDePasse", "PhotoClient" },
                values: new object[] { "$2a$11$XUuE32HP7CcOrJvjYS.CDOFBASn69Oa78cRfVS4k5LwUkGdg7glPm", "mahmoud.jpg" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MotDePasse", "PhotoClient" },
                values: new object[] { "$2a$11$84obSXOXS8g359rjbr7MOuD0cY8xBH1lTrQpptb8PFEgo17c9L3we", "mahmoud.jpg" });

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "12345678923537902652",
                columns: new[] { "MontantMaxAutoriseParJour", "NbrOperationsAutoriseesParJour" },
                values: new object[] { 0m, null });

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "65432110223463790345",
                columns: new[] { "MontantMaxAutoriseParJour", "NbrOperationsAutoriseesParJour" },
                values: new object[] { 0m, null });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "ClientId", "DateCreation" },
                values: new object[] { 1, new DateTime(2025, 3, 19, 1, 18, 11, 295, DateTimeKind.Local).AddTicks(81) });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 19, 1, 18, 11, 295, DateTimeKind.Local).AddTicks(100));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontantMaxAutoriseParJour",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "NbrOperationsAutoriseesParJour",
                table: "Comptes");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondTPE",
                table: "Cartes",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondDAP",
                table: "Cartes",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                column: "Statut",
                value: "active");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MotDePasse", "PhotoClient" },
                values: new object[] { "$2a$11$wkcRMXy.iSj9j/WpA9073enTPsP.YLTs6hrRcqSfdjqS.QtHANucq", "C:\\Users\\Ikram\\Desktop\\ikram stage pfe\\STBEverywheres_Back\\STBEverywhere_Back_SharedModels\\Images\\mahmoud.jpg" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MotDePasse", "PhotoClient" },
                values: new object[] { "$2a$11$kr1VNWBmxojCwv4UofisW.lfJiyknxOU.5T8CcTh2BIVs7srLmd3C", "C:\\Users\\Ikram\\Desktop\\ikram stage pfe\\STBEverywheres_Back\\STBEverywhere_Back_SharedModels\\Images\\mahmoud.jpg" });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "ClientId", "DateCreation" },
                values: new object[] { 0, new DateTime(2025, 3, 11, 3, 38, 23, 411, DateTimeKind.Local).AddTicks(2641) });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 3, 38, 23, 411, DateTimeKind.Local).AddTicks(2667));
        }
    }
}
