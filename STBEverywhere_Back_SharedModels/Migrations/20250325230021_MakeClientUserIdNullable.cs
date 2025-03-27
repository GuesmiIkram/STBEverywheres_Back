using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class MakeClientUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "Clients",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpiry",
                table: "Clients",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ResetPasswordToken", "ResetPasswordTokenExpiry" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 25, 23, 0, 19, 794, DateTimeKind.Local).AddTicks(606));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 25, 23, 0, 19, 794, DateTimeKind.Local).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$MnFMSn98rp94HmODaU0Dlu51gHT37LgfHGJHgFGyLNa31.EPQmXq6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$VthxNz0Hqrd3XZcoFduqUO1AtSZ1xtAf2S3vrvqB7chYJZez8wKX.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$JEiXRzc6fjXku79DeYL0FeVzzwEzmPIEzhwLDOwfTVf/GITMwoSfe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiry",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 3, 25, 20, 24, 54, 651, DateTimeKind.Local).AddTicks(2077));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 3, 25, 20, 24, 54, 651, DateTimeKind.Local).AddTicks(2200));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$NIP/piS6PvzbKU98O5OLBOEuP9yrF8DC2QiF5Z5FyxNLmR4J29pdy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$6LEbkGEkjF84eAGJYvXgwO0jgIxb.9chueTxnLLqcp0JAqKd8CVEC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$6TvBHkaaNqzqfNKbm8cDo.lrjm78uE7ZC1Giee5g4cX47GKdk/YgK");
        }
    }
}
