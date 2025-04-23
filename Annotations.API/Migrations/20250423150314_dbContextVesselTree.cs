using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class dbContextVesselTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VesselPoints",
                columns: table => new
                {
                    PointId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselPoints", x => x.PointId);
                });

            migrationBuilder.CreateTable(
                name: "VesselTrees",
                columns: table => new
                {
                    VesselTreeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselTrees", x => x.VesselTreeId);
                    table.ForeignKey(
                        name: "FK_VesselTrees_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VesselSegments",
                columns: table => new
                {
                    SegmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartPointPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndPointPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Thickness = table.Column<double>(type: "REAL", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    VesselTreeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselSegments", x => x.SegmentId);
                    table.ForeignKey(
                        name: "FK_VesselSegments_VesselPoints_EndPointPointId",
                        column: x => x.EndPointPointId,
                        principalTable: "VesselPoints",
                        principalColumn: "PointId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselSegments_VesselPoints_StartPointPointId",
                        column: x => x.StartPointPointId,
                        principalTable: "VesselPoints",
                        principalColumn: "PointId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselSegments_VesselTrees_VesselTreeId",
                        column: x => x.VesselTreeId,
                        principalTable: "VesselTrees",
                        principalColumn: "VesselTreeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegments_EndPointPointId",
                table: "VesselSegments",
                column: "EndPointPointId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegments_StartPointPointId",
                table: "VesselSegments",
                column: "StartPointPointId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegments_VesselTreeId",
                table: "VesselSegments",
                column: "VesselTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselTrees_CreatedByUserId",
                table: "VesselTrees",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VesselSegments");

            migrationBuilder.DropTable(
                name: "VesselPoints");

            migrationBuilder.DropTable(
                name: "VesselTrees");
        }
    }
}
