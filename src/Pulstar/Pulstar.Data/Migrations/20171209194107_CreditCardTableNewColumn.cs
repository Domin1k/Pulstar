namespace Pulstar.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CreditCardTableNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CardType",
                table: "CreditCard",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardType",
                table: "CreditCard");
        }
    }
}
