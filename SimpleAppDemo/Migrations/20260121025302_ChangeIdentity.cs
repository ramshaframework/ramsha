using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RamshaIdentityRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBase = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RamshaIdentityRoleClaims_RamshaIdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RamshaIdentityRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RamshaIdentityUserClaims_RamshaIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RamshaIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_RamshaIdentityUserLogins_RamshaIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RamshaIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RamshaIdentityUserRoles_RamshaIdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RamshaIdentityRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RamshaIdentityUserRoles_RamshaIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RamshaIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RamshaIdentityUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamshaIdentityUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_RamshaIdentityUserTokens_RamshaIdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RamshaIdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RamshaIdentityRoleClaims_RoleId",
                table: "RamshaIdentityRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RamshaIdentityUserClaims_UserId",
                table: "RamshaIdentityUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RamshaIdentityUserLogins_UserId",
                table: "RamshaIdentityUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RamshaIdentityUserRoles_RoleId",
                table: "RamshaIdentityUserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RamshaIdentityRoleClaims");

            migrationBuilder.DropTable(
                name: "RamshaIdentityUserClaims");

            migrationBuilder.DropTable(
                name: "RamshaIdentityUserLogins");

            migrationBuilder.DropTable(
                name: "RamshaIdentityUserRoles");

            migrationBuilder.DropTable(
                name: "RamshaIdentityUserTokens");

            migrationBuilder.DropTable(
                name: "RamshaIdentityRoles");

            migrationBuilder.DropTable(
                name: "RamshaIdentityUsers");
        }
    }
}
