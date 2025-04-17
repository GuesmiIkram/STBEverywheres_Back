using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STBEverywhere_Back_SharedModels.Migrations
{
    /// <inheritdoc />
    public partial class notif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationsReclamation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotificationType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RelatedPackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsReclamation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsReclamation_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 4, 17, 13, 9, 52, 363, DateTimeKind.Local).AddTicks(684));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 4, 17, 13, 9, 52, 363, DateTimeKind.Local).AddTicks(750));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$FeQZ5dDtreGnhWR0jWEpiOpOnua.Kez.ms2ZGVh829N989HskjShe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$nMGzPuctp7GxW2/XzfiUd.W3JAn2RZgnyJGlevgpFf1FRwmJXtBUm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$b92RNQ/Tw8meb.ykjOWrlOxufa0v6S.aXnkBvt2.VqmHIPJARQ516");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$V5MrCNM94abzqsBHAIxnzuLTUC6fI2w.ujb5Inxii0Jnpy8hUHmay");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$eG1JlTes8trrU7JgUgMI3eP4ofj6r0xrR.vRXj5GHDYO8Q9y70Yqu");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsReclamation_ClientId",
                table: "NotificationsReclamation",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationsReclamation");

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 1,
                column: "DateCreation",
                value: new DateTime(2025, 4, 15, 14, 33, 2, 993, DateTimeKind.Local).AddTicks(6884));

            migrationBuilder.UpdateData(
                table: "DemandesCarte",
                keyColumn: "Iddemande",
                keyValue: 2,
                column: "DateCreation",
                value: new DateTime(2025, 4, 15, 14, 33, 2, 993, DateTimeKind.Local).AddTicks(6952));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$4w6q.9zysWG8gvZaP414we/cYEdKMhCQkVAtLKvqB.k7R3ivnFuoS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$wYQTdpE.IfIJVPK2fQh7CupbZQL4c5HRpHuMeoMQHVV1CmBJhDfN.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$w4yfwHmwXZmcJO7EU2Wxo.x/R58jF2VB0BW.dp1Z032rVMfmAdW5G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$ZdgxNND75Cc7wWPUUHU6bOU6E1ibwGI3b/hxGY/aOBuTkMQ4sbk3.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$8GRVNRs7NrU6P0yn2kyw2.T/0TSHLxeJzfAjQLqwhezJ8QMA01016");
        }
    }
}
