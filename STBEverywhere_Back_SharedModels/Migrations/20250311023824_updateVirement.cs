using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class updateVirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FichierBeneficaires",
                table: "Virements",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$yoZtUaTRP.rDCtREVCGBXOasWJbv7Qro7t/ekX.9mujKjn0yJD1RK");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$4BHNbE7xwwtCAVXvWo07TuWUzORo1gxHu..HxTC78iV8JYputtjba");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 3, 38, 23, 411, DateTimeKind.Local).AddTicks(2641));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 3, 38, 23, 411, DateTimeKind.Local).AddTicks(2667));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Virements",
                keyColumn: "FichierBeneficaires",
                keyValue: null,
                column: "FichierBeneficaires",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FichierBeneficaires",
                table: "Virements",
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
                value: "$2a$11$leN23BRiS7mQfBeZy2FnpeFxlziHzsFlDGPln4tV39sv4pvSgAGRG");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$PX9dTCWICBmiRZF/DoMape7mG6/vX.exESpCcIteJtjKvr78SBSAC");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 2, 41, 1, 731, DateTimeKind.Local).AddTicks(9877));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 11, 2, 41, 1, 731, DateTimeKind.Local).AddTicks(9899));
        }
    }
}
