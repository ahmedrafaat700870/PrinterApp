using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrinterApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRawMaterail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RawMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RawMaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AreaSquareMeters = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PricePerSquareMeter = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PricePerLinearMeter = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMaterials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterials_RawMaterialName",
                table: "RawMaterials",
                column: "RawMaterialName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RawMaterials");
        }
    }
}
