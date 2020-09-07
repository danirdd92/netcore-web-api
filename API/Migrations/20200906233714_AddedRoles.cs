using Microsoft.EntityFrameworkCore.Migrations;

namespace web_api.Migrations
{
    public partial class AddedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "24a20c63-a31f-48c6-9a24-44668c1056d9", "0b194f10-5e47-4a51-b041-6a67a70f110a", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "530af3c5-08f1-4cec-b460-bc5e699944b0", "f4777385-bcb4-48c3-90c3-1b204a75f4b9", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "24a20c63-a31f-48c6-9a24-44668c1056d9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "530af3c5-08f1-4cec-b460-bc5e699944b0");
        }
    }
}
