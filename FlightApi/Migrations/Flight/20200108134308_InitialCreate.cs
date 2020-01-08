using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightApi.Migrations.Flight
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Destination = table.Column<string>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    DepartDate = table.Column<string>(nullable: true),
                    ReturnDate = table.Column<string>(nullable: true),
                    NumberOfChanges = table.Column<string>(nullable: true),
                    TripClass = table.Column<string>(nullable: true),
                    ShowToAffiliates = table.Column<string>(nullable: true),
                    Actual = table.Column<string>(nullable: true),
                    Gate = table.Column<string>(nullable: true),
                    FoundAt = table.Column<string>(nullable: true),
                    Distance = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");
        }
    }
}
