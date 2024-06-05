using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoicePk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectedFees_Invoices_InvoiceId",
                table: "CollectedFees");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Invoices_Id",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_CollectedFees_InvoiceId",
                table: "CollectedFees");

            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                table: "CollectedFees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CollectedFees_InvoiceId_ContractId",
                table: "CollectedFees",
                columns: new[] { "InvoiceId", "ContractId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CollectedFees_Invoices_InvoiceId_ContractId",
                table: "CollectedFees",
                columns: new[] { "InvoiceId", "ContractId" },
                principalTable: "Invoices",
                principalColumns: new[] { "Id", "ContractId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectedFees_Invoices_InvoiceId_ContractId",
                table: "CollectedFees");

            migrationBuilder.DropIndex(
                name: "IX_CollectedFees_InvoiceId_ContractId",
                table: "CollectedFees");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "CollectedFees");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Invoices_Id",
                table: "Invoices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CollectedFees_InvoiceId",
                table: "CollectedFees",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectedFees_Invoices_InvoiceId",
                table: "CollectedFees",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
