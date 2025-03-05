using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class LengthCapsAndRequiredFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureID",
                table: "AspNetUsers",
                newName: "ProfilePictureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureId",
                table: "AspNetUsers",
                newName: "ProfilePictureID");
        }
    }
}
