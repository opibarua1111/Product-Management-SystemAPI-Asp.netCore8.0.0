using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class OrderRecordupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "OrderRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrderRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                table: "OrderRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "OrderRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "OrderRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrderRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId",
                table: "Variants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRecords_OrderId",
                table: "OrderRecords",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRecords_VariantId",
                table: "OrderRecords",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRecords_Orders_OrderId",
                table: "OrderRecords",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRecords_Variants_VariantId",
                table: "OrderRecords",
                column: "VariantId",
                principalTable: "Variants",
                principalColumn: "VariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRecords_Orders_OrderId",
                table: "OrderRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderRecords_Variants_VariantId",
                table: "OrderRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_Variants_ProductId",
                table: "Variants");

            migrationBuilder.DropIndex(
                name: "IX_OrderRecords_OrderId",
                table: "OrderRecords");

            migrationBuilder.DropIndex(
                name: "IX_OrderRecords_VariantId",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "OrderRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "OrderRecords");
        }
    }
}
