using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioApi.Migrations
{
    /// <inheritdoc />
    public partial class FixBlogpostProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicatieDatum",
                table: "Blogposts",
                newName: "Publicatiedatum");

            migrationBuilder.RenameColumn(
                name: "Inhooud",
                table: "Blogposts",
                newName: "Inhoud");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Publicatiedatum",
                table: "Blogposts",
                newName: "PublicatieDatum");

            migrationBuilder.RenameColumn(
                name: "Inhoud",
                table: "Blogposts",
                newName: "Inhooud");
        }
    }
}
