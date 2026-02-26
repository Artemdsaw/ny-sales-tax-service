using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SurveySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompositeTaxRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StateRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: true),
                    CountyRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: true),
                    CityRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: true),
                    SpecialRates = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: true),
                    Jurisdictions = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StateRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    CountyRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    CityRate = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    SpecialRates = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    Boundary = table.Column<Geometry>(type: "geometry", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxZones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "TaxZones");
        }
    }
}
