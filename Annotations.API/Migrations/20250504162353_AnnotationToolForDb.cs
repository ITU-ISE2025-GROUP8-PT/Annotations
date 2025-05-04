using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class AnnotationToolForDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "VesselAnnotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselAnnotation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Annotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ImgId = table.Column<int>(type: "INTEGER", nullable: false),
                    AnnotationTreeId = table.Column<int>(type: "INTEGER", nullable: false),
                    VesselAnnotationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annotation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotation_Images_ImgId",
                        column: x => x.ImgId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotation_VesselAnnotation_AnnotationTreeId",
                        column: x => x.AnnotationTreeId,
                        principalTable: "VesselAnnotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotation_VesselAnnotation_VesselAnnotationId",
                        column: x => x.VesselAnnotationId,
                        principalTable: "VesselAnnotation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VesselPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    VesselAnnotationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselPoint_VesselAnnotation_VesselAnnotationId",
                        column: x => x.VesselAnnotationId,
                        principalTable: "VesselAnnotation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VesselSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Thickness = table.Column<double>(type: "REAL", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    VesselAnnotationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselSegment_VesselAnnotation_VesselAnnotationId",
                        column: x => x.VesselAnnotationId,
                        principalTable: "VesselAnnotation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VesselSegment_VesselPoint_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "VesselPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VesselSegment_VesselPoint_StartPointId",
                        column: x => x.StartPointId,
                        principalTable: "VesselPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_AnnotationTreeId",
                table: "Annotation",
                column: "AnnotationTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_ImgId",
                table: "Annotation",
                column: "ImgId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_UserId",
                table: "Annotation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_VesselAnnotationId",
                table: "Annotation",
                column: "VesselAnnotationId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselPoint_VesselAnnotationId",
                table: "VesselPoint",
                column: "VesselAnnotationId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegment_EndPointId",
                table: "VesselSegment",
                column: "EndPointId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegment_StartPointId",
                table: "VesselSegment",
                column: "StartPointId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselSegment_VesselAnnotationId",
                table: "VesselSegment",
                column: "VesselAnnotationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Annotation");

            migrationBuilder.DropTable(
                name: "VesselSegment");

            migrationBuilder.DropTable(
                name: "VesselPoint");

            migrationBuilder.DropTable(
                name: "VesselAnnotation");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
