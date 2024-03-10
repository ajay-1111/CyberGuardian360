using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberGuardian360.Migrations
{
    public partial class CG360_DBChanges_C3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CSUserCartInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCost = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSUserCartInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CSUserOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSUserOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CSUserOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSUserOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CSUserOrderItems_CSProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "CSProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CSUserOrderItems_CSUserOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "CSUserOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CSUserOrderItems_OrderId",
                table: "CSUserOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CSUserOrderItems_ProductId",
                table: "CSUserOrderItems",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CSUserCartInfo");

            migrationBuilder.DropTable(
                name: "CSUserOrderItems");

            migrationBuilder.DropTable(
                name: "CSUserOrders");
        }
    }
}
