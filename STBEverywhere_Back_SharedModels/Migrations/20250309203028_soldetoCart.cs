using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class soldetoCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Solde",
                table: "Cartes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                column: "Solde",
                value: 10000m);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                column: "Solde",
                value: 5000m);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$Da6TEss1JZpEk0mOjth84.3pEDvrHcS4wdNwiezu6LIfIztmiEpNC");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$7oxoR/swNrWdFESEpmTf3u84uWbFfncFxTMC5m9b2Civ5HYmQBI3W");

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "65432110223463790345",
                column: "Type",
                value: "Epargne");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 20, 30, 27, 620, DateTimeKind.Local).AddTicks(9755));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 20, 30, 27, 620, DateTimeKind.Local).AddTicks(9854));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Solde",
                table: "Cartes");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$XlaYFD77jUU7AK3elJxseuGkO/EBIRy0hiIq.S2qSNOkFOwt.vkiy");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$/2XLqdoHJxlypBMon/gRt.vo9mgnY.RKi5A8z/G4OXmMvM/6fL5H6");

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "65432110223463790345",
                column: "Type",
                value: "Épargne");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 11, 5, 8, 863, DateTimeKind.Local).AddTicks(3342));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 11, 5, 8, 863, DateTimeKind.Local).AddTicks(3411));
        }
    }
}
