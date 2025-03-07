using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class AddPlafondToCarte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Plafond",
                table: "Cartes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                column: "Plafond",
                value: 1000m);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                column: "Plafond",
                value: 1000m);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plafond",
                table: "Cartes");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$OGp/VPw0SQqCIHVb8U6zC.tL4DnGPgvg.pnHsoUVEGNqg3IKjRLpG");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$JXfooqFFSNxWp0ns8R/s1.uhnwAtDVsYaTTDA1O6GWMGXsjeqRCeK");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 2, 10, 11, 28, 645, DateTimeKind.Local).AddTicks(2049));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 2, 10, 11, 28, 645, DateTimeKind.Local).AddTicks(2119));
        }
    }
}
