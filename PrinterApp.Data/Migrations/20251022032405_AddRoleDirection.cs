using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrinterApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RollDirections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DirectionNumber = table.Column<int>(type: "int", nullable: false),
                    DirectionImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollDirections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RollDirections_DirectionNumber",
                table: "RollDirections",
                column: "DirectionNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RollDirections");
        }
    }
}
