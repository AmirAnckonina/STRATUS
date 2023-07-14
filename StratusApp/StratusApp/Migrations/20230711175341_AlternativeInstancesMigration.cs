using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratusApp.Migrations
{
    /// <inheritdoc />
    public partial class AlternativeInstancesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropPrimaryKey(
                name: "PK_StratusUsers",
                table: "StratusUsers");

            migrationBuilder.RenameTable(
                name: "StratusUsers",
                newName: "Users");*/

            /*migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");*/

            migrationBuilder.CreateTable(
                name: "AlternativeInstances",
                columns: table => new
                {
                    InstanceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlyRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vCPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Memory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetworkPerformance = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeInstances", x => x.InstanceName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternativeInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");
        }
    }
}
