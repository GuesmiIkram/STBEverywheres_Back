using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class Addemails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailEnvoyeLivree",
                table: "DemandesCarte",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$ghx8dlsF8Pd7k9usyJGLo.NhpHnoR5e9ou958V6GhzQPQpH77yKKK");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$R562JoDJ2vmSE7DcCR4B0.jI88tHOutUfeTddPyl8w2TK3.JQbJ/a");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "DateCreation", "EmailEnvoyeLivree" },
                values: new object[] { new DateTime(2025, 3, 3, 18, 29, 8, 356, DateTimeKind.Local).AddTicks(9035), false });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                columns: new[] { "DateCreation", "EmailEnvoyeLivree" },
                values: new object[] { new DateTime(2025, 3, 3, 18, 29, 8, 356, DateTimeKind.Local).AddTicks(9117), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailEnvoyeLivree",
                table: "DemandesCarte");

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
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 15, 24, 19, 253, DateTimeKind.Local).AddTicks(8620));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 15, 24, 19, 253, DateTimeKind.Local).AddTicks(8706));
        }
    }
}
