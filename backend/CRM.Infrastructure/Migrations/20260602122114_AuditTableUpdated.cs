using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Tenant_Name",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_Name",
                table: "Customers",
                columns: new[] { "TenantId", "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityHistory",
                table: "AuditLogs",
                columns: new[] { "TenantId", "EntityType", "EntityId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Tenant_Name",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityHistory",
                table: "AuditLogs");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_Name",
                table: "Customers",
                columns: new[] { "TenantId", "Name", "IsDeleted" });
        }
    }
}
