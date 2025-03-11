using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class updateStatusDemChequier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
