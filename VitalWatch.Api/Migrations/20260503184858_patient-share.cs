using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class patientshare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientShareCode",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientShareCode",
                table: "Patients");
        }
    }
}
