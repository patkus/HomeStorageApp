using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeStorageApp.Drugs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDrugsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "drugs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PrimaryUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    barcodes = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_drugs_system_units_PrimaryUnitId",
                        column: x => x.PrimaryUnitId,
                        principalTable: "system_units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "derived_units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DrugId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BaseUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    IsDefaultPurchaseUnit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    barcodes = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_derived_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_derived_units_drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "system_units",
                columns: new[] { "Id", "Category", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Piece", "tabletka", "tab" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Piece", "kapsułka", "kaps" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Piece", "sztuka", "szt" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Piece", "dawka", "daw" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Volume", "mililitr", "ml" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Volume", "litr", "l" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Volume", "kropla", "kr" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "Weight", "miligram", "mg" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Weight", "gram", "g" },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "Weight", "kilogram", "kg" },
                    { new Guid("00000000-0000-0000-0000-000000000031"), "Other", "saszetka", "sasz" },
                    { new Guid("00000000-0000-0000-0000-000000000032"), "Other", "ampułka", "amp" },
                    { new Guid("00000000-0000-0000-0000-000000000033"), "Other", "fiolka", "fiol" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_derived_units_base_unit_id",
                table: "derived_units",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "ix_derived_units_drug_id",
                table: "derived_units",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "ix_derived_units_drug_id_is_default",
                table: "derived_units",
                columns: new[] { "DrugId", "IsDefaultPurchaseUnit" });

            migrationBuilder.CreateIndex(
                name: "ix_derived_units_drug_id_name_unique",
                table: "derived_units",
                columns: new[] { "DrugId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_drugs_is_archived",
                table: "drugs",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "ix_drugs_name",
                table: "drugs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "ix_drugs_primary_unit_id",
                table: "drugs",
                column: "PrimaryUnitId");

            migrationBuilder.CreateIndex(
                name: "ix_drugs_user_id",
                table: "drugs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_drugs_user_id_is_archived",
                table: "drugs",
                columns: new[] { "UserId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "ix_system_units_category",
                table: "system_units",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "ix_system_units_name",
                table: "system_units",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "ix_system_units_symbol",
                table: "system_units",
                column: "Symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "derived_units");

            migrationBuilder.DropTable(
                name: "drugs");

            migrationBuilder.DropTable(
                name: "system_units");
        }
    }
}
