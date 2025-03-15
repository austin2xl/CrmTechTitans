using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class AddTypePropertiesToAddressAndContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "Contacts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Addresses",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Addresses");
        }
    }
}
