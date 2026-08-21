using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorLeadConversionHistoryToGeneric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeadConversionHistories_Tenant_Customer",
                table: "LeadConversionHistories");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "LeadConversionHistories",
                newName: "RelatedEntityId");

            migrationBuilder.AddColumn<int>(
                name: "ConversionType",
                table: "LeadConversionHistories",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_LeadConversionHistories_Tenant_Conversion",
                table: "LeadConversionHistories",
                columns: new[] { "TenantId", "ConversionType", "RelatedEntityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeadConversionHistories_Tenant_Conversion",
                table: "LeadConversionHistories");

            migrationBuilder.DropColumn(
                name: "ConversionType",
                table: "LeadConversionHistories");

            migrationBuilder.RenameColumn(
                name: "RelatedEntityId",
                table: "LeadConversionHistories",
                newName: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadConversionHistories_Tenant_Customer",
                table: "LeadConversionHistories",
                columns: new[] { "TenantId", "CustomerId" });
        }
    }
}
