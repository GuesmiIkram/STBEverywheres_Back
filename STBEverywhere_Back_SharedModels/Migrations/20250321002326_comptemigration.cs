using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class comptemigration : Migration
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

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$CuzoT6knnZo7y2cdmbVS1elhg1zREp.aMZI2/78GsaJknW4dlwJN.");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$JX8hh5xjlULr28xXmVfP5unYP4/8m2b4CL0peASvc9LSiwZYLCN2a");

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
                column: "DateCreation",
                value: new DateTime(2025, 3, 21, 1, 23, 25, 397, DateTimeKind.Local).AddTicks(4874));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 21, 1, 23, 25, 397, DateTimeKind.Local).AddTicks(4895));
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

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$FHiP9xVORVvxow3IEAqiz.tSXLCEGRuMCsQv8N8lL63Xx4let./am");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$qrRUYFXxgwvXHPSNUC0ng.aaUWAazoOjlvC3haD/VO62GiwkzQJ.6");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 20, 22, 39, 50, 552, DateTimeKind.Local).AddTicks(4724));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 20, 22, 39, 50, 552, DateTimeKind.Local).AddTicks(4743));
        }
    }
}
