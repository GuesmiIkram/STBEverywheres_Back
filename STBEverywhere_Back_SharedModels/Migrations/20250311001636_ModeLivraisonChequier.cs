using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class ModeLivraisonChequier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Agence",
                table: "DemandesChequiers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AdresseComplete",
                table: "DemandesChequiers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "DemandesChequiers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ModeLivraison",
                table: "DemandesChequiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$vZXwXuA8kN2oPSpefE8y7usn0m1i0p2F3BMRGSe7afA4kSNdZe2jq");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$8rfGDFkBPxitri6yp.G.4OMDQdY/EV0EaTbBcSYfK0ZX.PqnRBaz2");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 1, 16, 36, 81, DateTimeKind.Local).AddTicks(384));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 1, 16, 36, 81, DateTimeKind.Local).AddTicks(403));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdresseComplete",
                table: "DemandesChequiers");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "DemandesChequiers");

            migrationBuilder.DropColumn(
                name: "ModeLivraison",
                table: "DemandesChequiers");

            migrationBuilder.UpdateData(
                table: "DemandesChequiers",
                keyColumn: "Agence",
                keyValue: null,
                column: "Agence",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Agence",
                table: "DemandesChequiers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$1MFaKbKMkh3GzO6flxSU4eWYUxpX2cTTqK.wOhQqhFzRk8dV/EGIu");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$F9yaVvo0c6DwS0ncT6SODen2fZo/nNB2l09jIn4phEtOAyrZhFGTy");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 10, 20, 53, 25, 468, DateTimeKind.Local).AddTicks(4648));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 10, 20, 53, 25, 468, DateTimeKind.Local).AddTicks(4675));
        }
    }
}
