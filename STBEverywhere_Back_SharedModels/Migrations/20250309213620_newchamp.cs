using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class newchamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plafond",
                table: "Cartes");

            migrationBuilder.AddColumn<string>(
                name: "Nature",
                table: "Cartes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "PlafondDAP",
                table: "Cartes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlafondTPE",
                table: "Cartes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                columns: new[] { "Nature", "PlafondDAP", "PlafondTPE", "Solde" },
                values: new object[] { "postpayée", 20000m, 40000m, 1000.50m });

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                columns: new[] { "Nature", "PlafondDAP", "PlafondTPE" },
                values: new object[] { "postpayée", 20000m, 40000m });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$wkcRMXy.iSj9j/WpA9073enTPsP.YLTs6hrRcqSfdjqS.QtHANucq");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$kr1VNWBmxojCwv4UofisW.lfJiyknxOU.5T8CcTh2BIVs7srLmd3C");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 21, 36, 20, 83, DateTimeKind.Local).AddTicks(4926));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 21, 36, 20, 83, DateTimeKind.Local).AddTicks(4996));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nature",
                table: "Cartes");

            migrationBuilder.DropColumn(
                name: "PlafondDAP",
                table: "Cartes");

            migrationBuilder.DropColumn(
                name: "PlafondTPE",
                table: "Cartes");

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
                columns: new[] { "Plafond", "Solde" },
                values: new object[] { 1000m, 10000m });

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
                value: "$2a$11$Da6TEss1JZpEk0mOjth84.3pEDvrHcS4wdNwiezu6LIfIztmiEpNC");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$7oxoR/swNrWdFESEpmTf3u84uWbFfncFxTMC5m9b2Civ5HYmQBI3W");

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
    }
}
