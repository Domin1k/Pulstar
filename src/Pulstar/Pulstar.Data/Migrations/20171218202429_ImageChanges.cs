namespace Pulstar.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ImageChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                 name: "Image",
                 table: "Products",
                 nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
