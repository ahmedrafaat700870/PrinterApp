using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrinterApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManufacturingAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManufacturingAdditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturingAdditions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingAdditions_AdditionName",
                table: "ManufacturingAdditions",
                column: "AdditionName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManufacturingAdditions");
        }
    }
}
