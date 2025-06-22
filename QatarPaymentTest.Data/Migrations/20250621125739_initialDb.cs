using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QatarPaymentTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "citext", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "citext", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsInactive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FieldType = table.Column<string>(type: "text", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContacts",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ContactId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContacts", x => new { x.CompanyId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_CompanyContacts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCustomFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CustomFieldId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCustomFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyCustomFieldValues_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyCustomFieldValues_CustomFields_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactCustomFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactId = table.Column<int>(type: "integer", nullable: false),
                    CustomFieldId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactCustomFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactCustomFieldValues_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactCustomFieldValues_CustomFields_CustomFieldId",
                        column: x => x.CustomFieldId,
                        principalTable: "CustomFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Leading technology company", "Microsoft Corporation", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Search and technology giant", "Google LLC", null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Consumer electronics and software", "Apple Inc.", null }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "Email", "FirstName", "IsInactive", "IsPrimary", "LastName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "ahmed.m@example.com", "أحمد", false, true, "محمد", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "sara.a@example.com", "سارة", false, true, "أحمد", null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "mohamed.a@example.com", "محمد", false, true, "علي", null }
                });

            migrationBuilder.InsertData(
                table: "CustomFields",
                columns: new[] { "Id", "CreatedAt", "DefaultValue", "DeletedAt", "Description", "EntityType", "FieldType", "IsActive", "IsRequired", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "تاريخ ميلاد جهة الاتصال", "Contact", "Date", true, true, "تاريخ الميلاد", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "رقم هاتف جهة الاتصال", "Contact", "Text", true, true, "رقم الهاتف", null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "عدد الموظفين في الشركة", "Company", "Number", true, false, "عدد الموظفين", null }
                });

            migrationBuilder.InsertData(
                table: "CompanyContacts",
                columns: new[] { "CompanyId", "ContactId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "CompanyCustomFieldValues",
                columns: new[] { "Id", "CompanyId", "CreatedAt", "CustomFieldId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, null, "150000" },
                    { 2, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, null, "180000" },
                    { 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, null, "160000" }
                });

            migrationBuilder.InsertData(
                table: "ContactCustomFieldValues",
                columns: new[] { "Id", "ContactId", "CreatedAt", "CustomFieldId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "1990-01-01" },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, "+974 3000 0001" },
                    { 3, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "1992-05-15" },
                    { 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, "+974 3000 0002" },
                    { 5, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, "1988-12-30" },
                    { 6, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, "+974 3000 0003" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_Unique",
                table: "Companies",
                column: "Name",
                unique: true)
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_Search",
                table: "Companies",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContacts_ContactId",
                table: "CompanyContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCustomFieldValues_CompanyId_CustomFieldId",
                table: "CompanyCustomFieldValues",
                columns: new[] { "CompanyId", "CustomFieldId" },
                unique: true)
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCustomFieldValues_CustomFieldId",
                table: "CompanyCustomFieldValues",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCustomFieldValues_Value",
                table: "CompanyCustomFieldValues",
                column: "Value")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactCustomFieldValues_ContactId_CustomFieldId",
                table: "ContactCustomFieldValues",
                columns: new[] { "ContactId", "CustomFieldId" },
                unique: true)
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_ContactCustomFieldValues_CustomFieldId",
                table: "ContactCustomFieldValues",
                column: "CustomFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactCustomFieldValues_Value",
                table: "ContactCustomFieldValues",
                column: "Value")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email_Unique",
                table: "Contacts",
                column: "Email",
                unique: true)
                .Annotation("Npgsql:IndexMethod", "btree");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email_Search",
                table: "Contacts",
                column: "Email")
                .Annotation("Npgsql:IndexMethod", "gist")
                .Annotation("Npgsql:IndexOperators", new[] { "gist_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFields_Name_EntityType",
                table: "CustomFields",
                columns: new[] { "Name", "EntityType" },
                unique: true)
                .Annotation("Npgsql:IndexMethod", "btree");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyContacts");

            migrationBuilder.DropTable(
                name: "CompanyCustomFieldValues");

            migrationBuilder.DropTable(
                name: "ContactCustomFieldValues");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "CustomFields");
        }
    }
}
