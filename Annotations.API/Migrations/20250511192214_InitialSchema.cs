using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Annotations.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "VesselAnnotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselAnnotation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    AnnotatedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Datasets_Users_AnnotatedByUserId",
                        column: x => x.AnnotatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Datasets_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Datasets_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "VesselPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
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
                        name: "FK_Annotation_Images_ImgId",
                        column: x => x.ImgId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
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
                name: "DatasetEntry",
                columns: table => new
                {
                    DatasetId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetEntry", x => new { x.DatasetId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_DatasetEntry_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetEntry_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VesselSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    EndPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Thickness = table.Column<double>(type: "REAL", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
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
                name: "IX_DatasetEntry_ImageId",
                table: "DatasetEntry",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_AnnotatedByUserId",
                table: "Datasets",
                column: "AnnotatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_CreatedByUserId",
                table: "Datasets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_ReviewedByUserId",
                table: "Datasets",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadedByUserId",
                table: "Images",
                column: "UploadedByUserId");

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
                name: "DatasetEntry");

            migrationBuilder.DropTable(
                name: "VesselSegment");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "VesselPoint");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VesselAnnotation");
        }
    }
}
