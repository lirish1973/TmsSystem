using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TmsSystem.Migrations
{
    public partial class AddCurrencyHotelFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. הוסף עמודת Currency לטבלת tripoffers
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "tripoffers",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD")
                .Annotation("MySql:CharSet", "utf8mb4");

            // 2. הוסף עמודת HotelName לטבלת tripdays
            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "tripdays",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // 3. צור טבלת hotels
            migrationBuilder.CreateTable(
                name: "hotels",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HotelName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotels", x => x.HotelId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Currency", table: "tripoffers");
            migrationBuilder.DropColumn(name: "HotelName", table: "tripdays");
            migrationBuilder.DropTable(name: "hotels");
        }
    }
}
