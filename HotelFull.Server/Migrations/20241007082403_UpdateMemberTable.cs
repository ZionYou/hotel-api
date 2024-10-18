using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelFull.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberTable : Migration
    {
        /// <inheritdoc />
        /// test
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderItemID",
                table: "ProductOrderItem");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "IDNumber",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Member");

            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "ProductType",
                newName: "ProductTypeName");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProductOrder",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStat",
                table: "ProductOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStat",
                table: "ProductOrder");

            migrationBuilder.RenameColumn(
                name: "ProductTypeName",
                table: "ProductType",
                newName: "ProductType");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemID",
                table: "ProductOrderItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductOrder",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "ProductOrder",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Member",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Member",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Member",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "Member",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Member",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
