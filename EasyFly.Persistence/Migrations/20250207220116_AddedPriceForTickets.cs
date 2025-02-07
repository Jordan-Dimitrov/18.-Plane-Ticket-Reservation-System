using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyFly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceForTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_AspNetUsers_UserId",
                table: "Audit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit",
                table: "Audit");

            migrationBuilder.RenameTable(
                name: "Audit",
                newName: "Audits");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_UserId",
                table: "Audits",
                newName: "IX_Audits_UserId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Tickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audits",
                table: "Audits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_AspNetUsers_UserId",
                table: "Audits",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_AspNetUsers_UserId",
                table: "Audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audits",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Tickets");

            migrationBuilder.RenameTable(
                name: "Audits",
                newName: "Audit");

            migrationBuilder.RenameIndex(
                name: "IX_Audits_UserId",
                table: "Audit",
                newName: "IX_Audit_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit",
                table: "Audit",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_AspNetUsers_UserId",
                table: "Audit",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
