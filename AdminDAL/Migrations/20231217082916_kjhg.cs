using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminDAL.Migrations
{
    /// <inheritdoc />
    public partial class kjhg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SalesLT");

            migrationBuilder.CreateSequence<int>(
                name: "SalesOrderNumber",
                schema: "SalesLT");

            migrationBuilder.CreateTable(
                name: "EntityTbl",
                columns: table => new
                {
                    EntityName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTbl", x => x.EntityName);
                });

            migrationBuilder.CreateTable(
                name: "UserAdminRole",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminUserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdminRole", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    FeatureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeatureDataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    AdminComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "(N'')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.FeatureID);
                    table.ForeignKey(
                        name: "FK_Features_EntityTbl_EntityName",
                        column: x => x.EntityName,
                        principalTable: "EntityTbl",
                        principalColumn: "EntityName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_EntityName",
                table: "Features",
                column: "EntityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "UserAdminRole");

            migrationBuilder.DropTable(
                name: "EntityTbl");

            migrationBuilder.DropSequence(
                name: "SalesOrderNumber",
                schema: "SalesLT");
        }
    }
}
