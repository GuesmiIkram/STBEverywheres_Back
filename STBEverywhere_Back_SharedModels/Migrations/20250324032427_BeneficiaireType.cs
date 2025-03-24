using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class BeneficiaireType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RaisonSociale",
                table: "Beneficiaires",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Beneficiaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$tN7IisJTBnuRGXIQY9h.0.122S/DXsfu/.TLq9HCGCXnFpeqMwKoq");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$fFEqstai1B7GR.JyGtryqeLiZ/aNUJzUMpnicu3EDbWSuGDPsccJq");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 24, 4, 24, 26, 172, DateTimeKind.Local).AddTicks(1911));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 24, 4, 24, 26, 172, DateTimeKind.Local).AddTicks(1992));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaisonSociale",
                table: "Beneficiaires");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Beneficiaires");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$Ww/cyFVJTVUIp/ZLYqryL.CeDRRbjYGcRFE4xKZUvNyGVFn1j5LHK");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$8yoiCN8YAi.ICHMndmUs0ufJMBnsvLcI.crvb/82oOkLYvm9xmedC");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 24, 1, 52, 53, 822, DateTimeKind.Local).AddTicks(5310));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 24, 1, 52, 53, 822, DateTimeKind.Local).AddTicks(5337));
        }
    }
}
