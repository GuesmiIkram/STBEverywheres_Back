using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDemandeCarteAndClientRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$ZXz9/GyG7UjSU69VuiBVK.HYVWKy.DNt7JmGh8G3g703tlGOyJnfa");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$FDAwT0p0Q2m2ZFzm/qlnEuz4e5/7QHPUUyrQ8G3ogShq7ziYfCgi6");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 2, 28, 14, 50, 20, 613, DateTimeKind.Local).AddTicks(801));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 2, 28, 14, 50, 20, 613, DateTimeKind.Local).AddTicks(866));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "MotDePasse",
                value: "$2a$11$A3iRYlESYrHni9bs3o.gn.KaU381ALoLZRqFyMj4A/1UGLFSu18gq");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "MotDePasse",
                value: "$2a$11$r6fv0b1JJU3VmL.VN2zLNOZ7bgc0IllJFTEr874zxU/4z4npUU9MK");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 2, 28, 14, 41, 40, 877, DateTimeKind.Local).AddTicks(768));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 2, 28, 14, 41, 40, 877, DateTimeKind.Local).AddTicks(831));
        }
    }
}
