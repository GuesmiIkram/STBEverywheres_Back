using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class upDateDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$Ot9sGyph.Wct7y4VmCsg7e/nbzagUCY0mzOuQp7dK3d5oIkv4Frbu");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$agsvQ0.u0m7iGMo4z2Z5zechaDjUDbYOxuXy.jOMafLMgdtVyQ/yK");

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "12345678923537902652",
                column: "DateCreation",
                value: new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "65432110223463790345",
                columns: new[] { "ClientId", "DateCreation" },
                values: new object[] { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 19, 1, 33, 56, 569, DateTimeKind.Local).AddTicks(4096));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 19, 1, 33, 56, 569, DateTimeKind.Local).AddTicks(4116));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$XUuE32HP7CcOrJvjYS.CDOFBASn69Oa78cRfVS4k5LwUkGdg7glPm");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$84obSXOXS8g359rjbr7MOuD0cY8xBH1lTrQpptb8PFEgo17c9L3we");

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "12345678923537902652",
                column: "DateCreation",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "RIB",
                keyValue: "65432110223463790345",
                columns: new[] { "ClientId", "DateCreation" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 19, 1, 18, 11, 295, DateTimeKind.Local).AddTicks(81));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 19, 1, 18, 11, 295, DateTimeKind.Local).AddTicks(100));
        }
    }
}
