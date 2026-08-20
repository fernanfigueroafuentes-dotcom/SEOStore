using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEOStore.Infrastructure.Migrations;

/// <inheritdoc />
public partial class FilterProductUniqueIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Products_SKU",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_Slug",
            table: "Products");

        migrationBuilder.CreateIndex(
            name: "IX_Products_SKU",
            table: "Products",
            column: "SKU",
            unique: true,
            filter: "\"IsDeleted\" = FALSE");

        migrationBuilder.CreateIndex(
            name: "IX_Products_Slug",
            table: "Products",
            column: "Slug",
            unique: true,
            filter: "\"IsDeleted\" = FALSE");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Products_SKU",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_Slug",
            table: "Products");

        migrationBuilder.CreateIndex(
            name: "IX_Products_SKU",
            table: "Products",
            column: "SKU",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_Slug",
            table: "Products",
            column: "Slug",
            unique: true);
    }
}
