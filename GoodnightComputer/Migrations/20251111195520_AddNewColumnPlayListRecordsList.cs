using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodnightComputer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnPlayListRecordsList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationsList",
                table: "PlaylistEntities",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationsList",
                table: "PlaylistEntities");
        }
    }
}
