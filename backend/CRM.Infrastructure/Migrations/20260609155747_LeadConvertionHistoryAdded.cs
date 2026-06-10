using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeadConvertionHistoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConvertedAtUtc",
                table: "Leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConvertedByUserId",
                table: "Leads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConvertedCustomerId",
                table: "Leads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LeadConversionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConvertedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConvertedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadConversionHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadConversionHistories_Tenant_ConvertedAt",
                table: "LeadConversionHistories",
                columns: new[] { "TenantId", "ConvertedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_LeadConversionHistories_Tenant_Customer",
                table: "LeadConversionHistories",
                columns: new[] { "TenantId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_LeadConversionHistories_Tenant_Lead",
                table: "LeadConversionHistories",
                columns: new[] { "TenantId", "LeadId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadConversionHistories");

            migrationBuilder.DropColumn(
                name: "ConvertedAtUtc",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ConvertedByUserId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ConvertedCustomerId",
                table: "Leads");
        }
    }
}
