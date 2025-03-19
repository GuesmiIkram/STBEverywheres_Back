using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class up : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TypeCarte",
                table: "DemandesCarte",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Statut",
                table: "DemandesCarte",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomCarte",
                table: "DemandesCarte",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCarte",
                table: "Cartes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Statut",
                table: "Cartes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NomCarte",
                table: "Cartes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                columns: new[] { "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { "VisaClassic", "Active", "International" });

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                columns: new[] { "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { "Mastercard", "Active", "National" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$xVf7sbaLWfM6/kdlwXAnOevjWGoraQEgvvF3VCLqOxLAVs6de/wM2");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$P830J74dpZbyHyDv0nSg0uMP5t.FBuOSksGvi1/iE5QV0TKu3ukGi");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "DateCreation", "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { new DateTime(2025, 3, 16, 23, 41, 56, 984, DateTimeKind.Local).AddTicks(6574), "VisaClassic", "DisponibleEnAgence", "International" });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                columns: new[] { "DateCreation", "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { new DateTime(2025, 3, 16, 23, 41, 56, 984, DateTimeKind.Local).AddTicks(6679), "Mastercard", "EnPreparation", "National" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TypeCarte",
                table: "DemandesCarte",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Statut",
                table: "DemandesCarte",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "NomCarte",
                table: "DemandesCarte",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TypeCarte",
                table: "Cartes",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Statut",
                table: "Cartes",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "NomCarte",
                table: "Cartes",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "1111222233334444",
                columns: new[] { "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { 0, 0, 1 });

            migrationBuilder.UpdateData(
                table: "Cartes",
                keyColumn: "NumCarte",
                keyValue: "5555666677778888",
                columns: new[] { "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { 1, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$b47f1ZKXEBKGFn2OKTaTFO4MGpSLWx0AkyqIXcrNwZQad7q4zmpf.");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$cvTSbwr/Tx2txwysz/DuW.CsFbJldRqsgSBVpA8CICoJc/5/vHPEa");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "DateCreation", "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { new DateTime(2025, 3, 16, 23, 25, 23, 152, DateTimeKind.Local).AddTicks(2252), 0, 1, 1 });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                columns: new[] { "DateCreation", "NomCarte", "Statut", "TypeCarte" },
                values: new object[] { new DateTime(2025, 3, 16, 23, 25, 23, 152, DateTimeKind.Local).AddTicks(2352), 1, 5, 0 });
        }
    }
}
