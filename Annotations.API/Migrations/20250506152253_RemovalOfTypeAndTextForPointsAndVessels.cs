using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class RemovalOfTypeAndTextForPointsAndVessels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "VesselSegment");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VesselSegment");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "VesselPoint");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VesselPoint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "VesselSegment",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VesselSegment",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "VesselPoint",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VesselPoint",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
