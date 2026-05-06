using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLotPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lot_photos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    thumb_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    medium_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    large_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lot_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_lot_photos_lots_lot_id",
                        column: x => x.lot_id,
                        principalTable: "lots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lot_photos_lot_id",
                table: "lot_photos",
                column: "lot_id");

            migrationBuilder.CreateIndex(
                name: "ix_lot_photos_lot_id_sort_order",
                table: "lot_photos",
                columns: new[] { "lot_id", "sort_order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lot_photos");
        }
    }
}
