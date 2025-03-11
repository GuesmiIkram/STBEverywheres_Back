using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class fileVirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FichierBeneficaires",
                table: "Virements",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TypeVirement",
                table: "Virements",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondFeuille",
                table: "FeuillesChequiers",
                type: "decimal(10,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FichierBeneficaires",
                table: "Virements");

            migrationBuilder.DropColumn(
                name: "TypeVirement",
                table: "Virements");

            migrationBuilder.AlterColumn<decimal>(
                name: "PlafondFeuille",
                table: "FeuillesChequiers",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$Wr3DS9Zr3gNN7HI32/z0YOHgpkPwla2ZMEHsnK8mzJtj5lTazme0G");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$79TC0KkwrK6xNwJ.XGzf0u/kZspZttCxySGMdUQGz1HGjihJW7wSq");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 12, 59, 16, 683, DateTimeKind.Local).AddTicks(41));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 9, 12, 59, 16, 683, DateTimeKind.Local).AddTicks(59));
        }
    }
}
