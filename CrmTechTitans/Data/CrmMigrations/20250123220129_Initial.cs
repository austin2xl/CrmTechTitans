using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTechTitans.Data.CrmMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Province = table.Column<int>(type: "INTEGER", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Linkedin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NAICS = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Interactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    interaction = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MembershipType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CompanySize = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyWebsite = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MemberSince = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastContactDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MembershipStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IndustryMembers",
                columns: table => new
                {
                    IndustryID = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryMembers", x => new { x.IndustryID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_IndustryMembers_Industries_IndustryID",
                        column: x => x.IndustryID,
                        principalTable: "Industries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndustryMembers_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberAddresses",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressID = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAddresses", x => new { x.MemberID, x.AddressID });
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberAddresses_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberContacts",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberContacts", x => new { x.ContactID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_MemberContacts_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberContacts_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberOpportunities",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    OpportunityID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberOpportunities", x => new { x.OpportunityID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_MemberOpportunities_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberOpportunities_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndustryMembers_MemberID",
                table: "IndustryMembers",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAddresses_AddressID",
                table: "MemberAddresses",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberContacts_MemberID",
                table: "MemberContacts",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberOpportunities_MemberID",
                table: "MemberOpportunities",
                column: "MemberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndustryMembers");

            migrationBuilder.DropTable(
                name: "Interactions");

            migrationBuilder.DropTable(
                name: "MemberAddresses");

            migrationBuilder.DropTable(
                name: "MemberContacts");

            migrationBuilder.DropTable(
                name: "MemberOpportunities");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Opportunities");
        }
    }
}
