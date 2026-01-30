using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtual_Power_Grid_Simulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PowerConsumers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MaxPeakLoad = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerConsumers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerPlants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CurrentPower = table.Column<decimal>(type: "numeric", nullable: false),
                    IsWorking = table.Column<bool>(type: "boolean", nullable: false),
                    MaxCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    MinStableLoad = table.Column<decimal>(type: "numeric", nullable: false),
                    SetPoint = table.Column<decimal>(type: "numeric", nullable: false),
                    RampRatePerTick = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerPlants", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerConsumers");

            migrationBuilder.DropTable(
                name: "PowerPlants");
        }
    }
}
