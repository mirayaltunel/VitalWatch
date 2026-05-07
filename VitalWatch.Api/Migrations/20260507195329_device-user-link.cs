using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VitalWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class deviceuserlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Devices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Diseases",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "IsActive", "IsDeleted", "Name", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Epilepsi", null, null },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "KOAH", null, null },
                    { 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Diyabet", null, null },
                    { 4, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Hipertansiyon", null, null },
                    { 5, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Alzheimer", null, null },
                    { 6, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Parkinson", null, null },
                    { 7, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Astım", null, null },
                    { 8, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Kalp Yetmezliği", null, null },
                    { 9, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Demans", null, null },
                    { 10, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Felç", null, null },
                    { 11, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "Diğer", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId",
                table: "Devices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Users_UserId",
                table: "Devices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Users_UserId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_UserId",
                table: "Devices");

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Diseases",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
