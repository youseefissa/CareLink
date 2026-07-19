using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientSleepWindow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "SleepWindowEnd",
                table: "PatientProfiles",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SleepWindowStart",
                table: "PatientProfiles",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SleepWindowEnd",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "SleepWindowStart",
                table: "PatientProfiles");
        }
    }
}
