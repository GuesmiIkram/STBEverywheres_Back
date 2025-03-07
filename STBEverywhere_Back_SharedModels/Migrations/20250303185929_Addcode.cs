using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class Addcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodePIN",
                table: "Cartes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 4)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CodeCVV",
                table: "Cartes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 3)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                columns: new[] { "CodeCVV", "CodePIN" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                columns: new[] { "CodeCVV", "CodePIN" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$wSsSLO4o8ZI2JffscLLl4eNuwALR5FuSGTQHEdFlpvWT2f0LwKuRu");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$4VomOUl1g0VNM.M2TC35oeZsr6iLqdquBf9YEHYDZaiEV9jijDZPS");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 18, 59, 29, 11, DateTimeKind.Local).AddTicks(2417));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 18, 59, 29, 11, DateTimeKind.Local).AddTicks(2502));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CodePIN",
                table: "Cartes",
                type: "int",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CodeCVV",
                table: "Cartes",
                type: "int",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                columns: new[] { "CodeCVV", "CodePIN" },
                values: new object[] { 123, 1234 });

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                columns: new[] { "CodeCVV", "CodePIN" },
                values: new object[] { 153, 1534 });

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
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 18, 29, 8, 356, DateTimeKind.Local).AddTicks(9035));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 3, 18, 29, 8, 356, DateTimeKind.Local).AddTicks(9117));
        }
    }
}
