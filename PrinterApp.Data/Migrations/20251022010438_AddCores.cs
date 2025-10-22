using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrinterApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoreName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CoreCoefficient = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    WidthCor = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HeightCor = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cores_CoreName",
                table: "Cores",
                column: "CoreName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cores");
        }
    }
}
