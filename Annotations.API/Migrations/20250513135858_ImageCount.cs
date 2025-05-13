using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class ImageCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageCount",
                table: "Datasets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCount",
                table: "Datasets");
        }
    }
}
