namespace Pulstar.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CreditCardsIsActiveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_AspNetUsers_OwnerId",
                table: "CreditCard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCard",
                table: "CreditCard");

            migrationBuilder.RenameTable(
                name: "CreditCard",
                newName: "CreditCards");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCard_OwnerId",
                table: "CreditCards",
                newName: "IX_CreditCards_OwnerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CreditCards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCards",
                table: "CreditCards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_AspNetUsers_OwnerId",
                table: "CreditCards",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_AspNetUsers_OwnerId",
                table: "CreditCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCards",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CreditCards");

            migrationBuilder.RenameTable(
                name: "CreditCards",
                newName: "CreditCard");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCards_OwnerId",
                table: "CreditCard",
                newName: "IX_CreditCard_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCard",
                table: "CreditCard",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_AspNetUsers_OwnerId",
                table: "CreditCard",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
