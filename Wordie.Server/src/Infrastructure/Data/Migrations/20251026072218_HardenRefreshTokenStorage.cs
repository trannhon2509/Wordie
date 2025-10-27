using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wordie.Server.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class HardenRefreshTokenStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                // Ensure RefreshTokens table exists with TokenHash column. Avoid creating other identity tables
                var sql = @"
    IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
    BEGIN
        CREATE TABLE [RefreshTokens](
            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [TokenHash] NVARCHAR(MAX) NOT NULL,
            [UserId] NVARCHAR(MAX) NOT NULL,
            [Expires] DATETIME2 NOT NULL,
            [Created] DATETIME2 NOT NULL
        );
    END
    ELSE
    BEGIN
        -- If RefreshTokens exists but TokenHash column is missing, add it
        IF COL_LENGTH('dbo.RefreshTokens','TokenHash') IS NULL
        BEGIN
            ALTER TABLE [RefreshTokens] ADD [TokenHash] NVARCHAR(MAX) NULL;
        END
        -- If Id column is missing add a non-identity Id (best-effort). If complex schema exists, run manual migration.
        IF COL_LENGTH('dbo.RefreshTokens','Id') IS NULL
        BEGIN
            ALTER TABLE [RefreshTokens] ADD [Id] INT NULL;
            -- Note: making it IDENTITY on an existing table with rows is non-trivial; leave as nullable if data exists.
        END
    END
    ";

                migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TodoLists");
        }
    }
}
