using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infrastructure.Data.Migrations {
    public partial class SeedOrders : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            // Orders
            migrationBuilder.InsertData(
                schema: "orders",
                table: "Orders",
                columns: new[] { "Id", "CustomerId" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("33333333-3333-3333-3333-333333333333") }
                });

            // Order lines
            migrationBuilder.InsertData(
                schema: "orders",
                table: "OrderLines",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 2, 10.50m },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 1, 25.00m },

                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 3, 12.75m },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 2, 19.99m },

                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000003"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 1, 99.00m },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000003"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 4, 5.25m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000001"));
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000002"));
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000003"));
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000004"));
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000005"));
            migrationBuilder.DeleteData("orders", "OrderLines", "Id", new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData("orders", "Orders", "Id", new Guid("10000000-0000-0000-0000-000000000001"));
            migrationBuilder.DeleteData("orders", "Orders", "Id", new Guid("10000000-0000-0000-0000-000000000002"));
            migrationBuilder.DeleteData("orders", "Orders", "Id", new Guid("10000000-0000-0000-0000-000000000003"));
        }
    }
}
