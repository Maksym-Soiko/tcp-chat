using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockingUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blocked_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockerId = table.Column<int>(type: "int", nullable: false),
                    BlockedId = table.Column<int>(type: "int", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blocked_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_blocked_users_users_BlockedId",
                        column: x => x.BlockedId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_blocked_users_users_BlockerId",
                        column: x => x.BlockerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blocked_users_BlockedId",
                table: "blocked_users",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_blocked_users_BlockerId",
                table: "blocked_users",
                column: "BlockerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blocked_users");
        }
    }
}
