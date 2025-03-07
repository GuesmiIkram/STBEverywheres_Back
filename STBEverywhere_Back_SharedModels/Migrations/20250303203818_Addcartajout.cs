using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class Addcartajout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CarteAjouter",
                table: "DemandesCarte",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$/0HpxUQxwlosB7xhn02umOSmZKtjKyBq11zFoAiO0NHN7bIScp/8u");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$HBtMZq0UcScA9qkZ1gBjI.SXCU/34cyYzDO8y7ZJFJ0fRjDZOViwu");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                columns: new[] { "CarteAjouter", "DateCreation" },
                values: new object[] { false, new DateTime(2025, 3, 3, 20, 38, 18, 18, DateTimeKind.Local).AddTicks(9450) });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                columns: new[] { "CarteAjouter", "DateCreation" },
                values: new object[] { false, new DateTime(2025, 3, 3, 20, 38, 18, 18, DateTimeKind.Local).AddTicks(9510) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarteAjouter",
                table: "DemandesCarte");

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
    }
}
