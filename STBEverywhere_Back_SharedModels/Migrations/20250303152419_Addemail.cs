using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class Addemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailEnvoye",
                table: "DemandesCarte",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                column: "Statut",
                value: "Active");

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
                column: "MotDePasse",
                value: "$2a$11$Qgg8emDgvYRbsNSFVCHF0ebh1C4kROLPxWwNq18k4X6Ef3.lDXhxG");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$ThHY6X3G6KImzE1gNCGqN.Ej.knq26wThiUP6A80dDamNRGL3LwBy");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "DateCreation", "EmailEnvoye" },
                values: new object[] { new DateTime(2025, 3, 3, 15, 24, 19, 253, DateTimeKind.Local).AddTicks(8620), false });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                columns: new[] { "DateCreation", "EmailEnvoye" },
                values: new object[] { new DateTime(2025, 3, 3, 15, 24, 19, 253, DateTimeKind.Local).AddTicks(8706), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailEnvoye",
                table: "DemandesCarte");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                column: "Statut",
                value: "Livrée");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                column: "Statut",
                value: "En cours");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$Mjzycs3Vxd/57kj.w7je1ONmNa46JQqfanzNUpUJjA5c/JT.kp1jO");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$PJ.qJrSvdBjsaazYMuu.quXz5UQtm1CCeybGwgaPdyydbnBXWqMzq");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 2, 10, 16, 52, 374, DateTimeKind.Local).AddTicks(6203));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 2, 10, 16, 52, 374, DateTimeKind.Local).AddTicks(6295));
        }
    }
}
