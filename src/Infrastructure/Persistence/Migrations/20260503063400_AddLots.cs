using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starting_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    min_bid_step = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    current_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    current_winner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lots", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lots_ends_at",
                table: "lots",
                column: "ends_at");

            migrationBuilder.CreateIndex(
                name: "ix_lots_seller_id",
                table: "lots",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_lots_status",
                table: "lots",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lots");
        }
    }
}
