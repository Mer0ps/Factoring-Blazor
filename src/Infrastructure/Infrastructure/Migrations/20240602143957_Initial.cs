using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Numerics;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "JobExecutions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    TimeSpan = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExecutions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ScAdmins",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScAdmins", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "WhitelistedTokens",
                columns: table => new
                {
                    TokenIdentifier = table.Column<string>(type: "text", nullable: false),
                    MoneyMarketAddress = table.Column<string>(type: "text", nullable: true),
                    HTokenIdentifier = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhitelistedTokens", x => x.TokenIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "LegalForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalForms_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OnChainId = table.Column<long>(type: "bigint", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    VATNumber = table.Column<string>(type: "text", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: false),
                    LegalFormId = table.Column<int>(type: "integer", nullable: false),
                    BusinessActivityId = table.Column<int>(type: "integer", nullable: false),
                    IsKyc = table.Column<bool>(type: "boolean", nullable: false),
                    WithdrawAddress = table.Column<string>(type: "text", nullable: false),
                    Fee = table.Column<int>(type: "integer", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_BusinessActivities_BusinessActivityId",
                        column: x => x.BusinessActivityId,
                        principalTable: "BusinessActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_LegalForms_LegalFormId",
                        column: x => x.LegalFormId,
                        principalTable: "LegalForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IdAccount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrators_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AccountSupplierId = table.Column<long>(type: "bigint", nullable: false),
                    AccountClientId = table.Column<long>(type: "bigint", nullable: false),
                    IsSigned = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Accounts_AccountClientId",
                        column: x => x.AccountClientId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Accounts_AccountSupplierId",
                        column: x => x.AccountSupplierId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<BigInteger>(type: "numeric", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EuriborRate = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => new { x.Id, x.ContractId });
                    table.UniqueConstraint("AK_Invoices_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectedFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    CommissionFee = table.Column<BigInteger>(type: "numeric", nullable: false),
                    FinancingFee = table.Column<BigInteger>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectedFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectedFees_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    TxExecuteAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceHistories_Invoices_InvoiceId_ContractId",
                        columns: x => new { x.InvoiceId, x.ContractId },
                        principalTable: "Invoices",
                        principalColumns: new[] { "Id", "ContractId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BusinessActivityId",
                table: "Accounts",
                column: "BusinessActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LegalFormId",
                table: "Accounts",
                column: "LegalFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_IdAccount",
                table: "Administrators",
                column: "IdAccount");

            migrationBuilder.CreateIndex(
                name: "IX_CollectedFees_InvoiceId",
                table: "CollectedFees",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AccountClientId",
                table: "Contracts",
                column: "AccountClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AccountSupplierId",
                table: "Contracts",
                column: "AccountSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceHistories_InvoiceId_ContractId",
                table: "InvoiceHistories",
                columns: new[] { "InvoiceId", "ContractId" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ContractId",
                table: "Invoices",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalForms_CountryCode",
                table: "LegalForms",
                column: "CountryCode");

            /* Data initialization */

            var executionDate = DateTime.UtcNow;

            migrationBuilder.InsertData(
            table: "Countries",
            columns: new[] { "Code", "Name", "CreatedAt", "LastModified" },
            values: new object[,]
            {
                { "FR", "France", executionDate, executionDate },
                { "DE", "Germany", executionDate, executionDate },
                { "ES", "Spain", executionDate, executionDate },
                { "IT", "Italy", executionDate, executionDate },
                { "UK", "United Kingdom", executionDate, executionDate },
                { "BE", "Belgium", executionDate, executionDate }
            });

            migrationBuilder.InsertData(
            table: "LegalForms",
            columns: new[] { "Name", "CountryCode", "CreatedAt", "LastModified" },
            values: new object[,]
            {
                { "SA (Société Anonyme)", "FR", executionDate, executionDate },
                { "SARL (Société à Responsabilité Limitée)", "FR", executionDate, executionDate },
                { "SAS (Société par Actions Simplifiée)", "FR", executionDate, executionDate },
                { "SNC (Société en Nom Collectif)", "FR", executionDate, executionDate },
                { "GmbH (Gesellschaft mit beschränkter Haftung)", "DE", executionDate, executionDate },
                { "AG (Aktiengesellschaft)", "DE", executionDate, executionDate },
                { "OHG (Offene Handelsgesellschaft)", "DE", executionDate, executionDate },
                { "KG (Kommanditgesellschaft)", "DE", executionDate, executionDate },
                { "S.L. (Sociedad Limitada)", "ES", executionDate, executionDate },
                { "S.A. (Sociedad Anónima)", "ES", executionDate, executionDate },
                { "S.C. (Sociedad Colectiva)", "ES", executionDate, executionDate },
                { "S.Com (Sociedad Comanditaria)", "ES", executionDate, executionDate },
                { "S.r.l. (Società a responsabilità limitata)", "IT", executionDate, executionDate },
                { "S.p.A. (Società per Azioni)", "IT", executionDate, executionDate },
                { "S.a.s. (Società in accomandita semplice)", "IT", executionDate, executionDate },
                { "S.n.c. (Società in nome collettivo)", "IT", executionDate, executionDate },
                { "Ltd (Private Limited Company)", "UK", executionDate, executionDate },
                { "PLC (Public Limited Company)", "UK", executionDate, executionDate },
                { "LLP (Limited Liability Partnership)", "UK", executionDate, executionDate },
                { "Sole Trader (Entrepreneur Individuel)", "UK", executionDate, executionDate },
                { "SPRL (Société Privée à Responsabilité Limitée)", "BE", executionDate, executionDate },
                { "SA (Société Anonyme)", "BE", executionDate, executionDate },
                { "SCRL (Société Coopérative à Responsabilité Limitée)", "BE", executionDate, executionDate },
                { "SNC (Société en Nom Collectif)", "BE", executionDate, executionDate }
            });

            migrationBuilder.InsertData(
            table: "BusinessActivities",
            columns: new[] { "Code", "Description", "CreatedAt", "LastModified" },
            values: new object[,]
            {
                { "A01", "Crop and animal production, hunting and related service activities", executionDate, executionDate },
                { "A02", "Forestry and logging", executionDate, executionDate },
                { "A03", "Fishing and aquaculture", executionDate, executionDate },
                { "B05", "Mining of coal and lignite", executionDate, executionDate },
                { "B06", "Extraction of crude petroleum and natural gas", executionDate, executionDate },
                { "B07", "Mining of metal ores", executionDate, executionDate },
                { "B08", "Other mining and quarrying", executionDate, executionDate },
                { "B09", "Mining support service activities", executionDate, executionDate },
                { "C10", "Manufacture of food products", executionDate, executionDate },
                { "C11", "Manufacture of beverages", executionDate, executionDate },
                { "C12", "Manufacture of tobacco products", executionDate, executionDate },
                { "C13", "Manufacture of textiles", executionDate, executionDate },
                { "C14", "Manufacture of wearing apparel", executionDate, executionDate },
                { "C15", "Manufacture of leather and related products", executionDate, executionDate },
                { "C16", "Manufacture of wood and of products of wood and cork, except furniture; manufacture of articles of straw and plaiting materials", executionDate, executionDate },
                { "C17", "Manufacture of paper and paper products", executionDate, executionDate },
                { "C18", "Printing and reproduction of recorded media", executionDate, executionDate },
                { "C19", "Manufacture of coke and refined petroleum products", executionDate, executionDate },
                { "C20", "Manufacture of chemicals and chemical products", executionDate, executionDate },
                { "C21", "Manufacture of basic pharmaceutical products and pharmaceutical preparations", executionDate, executionDate },
                { "C22", "Manufacture of rubber and plastic products", executionDate, executionDate },
                { "C23", "Manufacture of other non-metallic mineral products", executionDate, executionDate },
                { "C24", "Manufacture of basic metals", executionDate, executionDate },
                { "C25", "Manufacture of fabricated metal products, except machinery and equipment", executionDate, executionDate },
                { "C26", "Manufacture of computer, electronic and optical products", executionDate, executionDate },
                { "C27", "Manufacture of electrical equipment", executionDate, executionDate },
                { "C28", "Manufacture of machinery and equipment n.e.c.", executionDate, executionDate },
                { "C29", "Manufacture of motor vehicles, trailers and semi-trailers", executionDate, executionDate },
                { "C30", "Manufacture of other transport equipment", executionDate, executionDate },
                { "C31", "Manufacture of furniture", executionDate, executionDate },
                { "C32", "Other manufacturing", executionDate, executionDate },
                { "C33", "Repair and installation of machinery and equipment", executionDate, executionDate },
                { "D35", "Electricity, gas, steam and air conditioning supply", executionDate, executionDate },
                { "E36", "Water collection, treatment and supply", executionDate, executionDate },
                { "E37", "Sewerage", executionDate, executionDate },
                { "E38", "Waste collection, treatment and disposal activities; materials recovery", executionDate, executionDate },
                { "E39", "Remediation activities and other waste management services", executionDate, executionDate },
                { "F41", "Construction of buildings", executionDate, executionDate },
                { "F42", "Civil engineering", executionDate, executionDate },
                { "F43", "Specialised construction activities", executionDate, executionDate },
                { "G45", "Wholesale and retail trade and repair of motor vehicles and motorcycles", executionDate, executionDate },
                { "G46", "Wholesale trade, except of motor vehicles and motorcycles", executionDate, executionDate },
                { "G47", "Retail trade, except of motor vehicles and motorcycles", executionDate, executionDate },
                { "H49", "Land transport and transport via pipelines", executionDate, executionDate },
                { "H50", "Water transport", executionDate, executionDate },
                { "H51", "Air transport", executionDate, executionDate },
                { "H52", "Warehousing and support activities for transportation", executionDate, executionDate },
                { "H53", "Postal and courier activities", executionDate, executionDate },
                { "I55", "Accommodation", executionDate, executionDate },
                { "I56", "Food and beverage service activities", executionDate, executionDate },
                { "J58", "Publishing activities", executionDate, executionDate },
                { "J59", "Motion picture, video and television programme production, sound recording and music publishing activities", executionDate, executionDate },
                { "J60", "Programming and broadcasting activities", executionDate, executionDate },
                { "J61", "Telecommunications", executionDate, executionDate },
                { "J62", "Computer programming, consultancy and related activities", executionDate, executionDate },
                { "J63", "Information service activities", executionDate, executionDate },
                { "K64", "Financial service activities, except insurance and pension funding", executionDate, executionDate },
                { "K65", "Insurance, reinsurance and pension funding, except compulsory social security", executionDate, executionDate },
                { "K66", "Activities auxiliary to financial services and insurance activities", executionDate, executionDate },
                { "L68", "Real estate activities", executionDate, executionDate },
                { "M69", "Legal and accounting activities", executionDate, executionDate },
                { "M70", "Activities of head offices; management consultancy activities", executionDate, executionDate },
                { "M71", "Architectural and engineering activities; technical testing and analysis", executionDate, executionDate },
                { "M72", "Scientific research and development", executionDate, executionDate },
                { "M73", "Advertising and market research", executionDate, executionDate },
                { "M74", "Other professional, scientific and technical activities", executionDate, executionDate },
                { "M75", "Veterinary activities", executionDate, executionDate },
                { "N77", "Rental and leasing activities", executionDate, executionDate },
                { "N78", "Employment activities", executionDate, executionDate },
                { "N79", "Travel agency, tour operator and other reservation service and related activities", executionDate, executionDate },
                { "N80", "Security and investigation activities", executionDate, executionDate },
                { "N81", "Services to buildings and landscape activities", executionDate, executionDate },
                { "N82", "Office administrative, office support and other business support activities", executionDate, executionDate },
                { "O84", "Public administration and defence; compulsory social security", executionDate, executionDate },
                { "P85", "Education", executionDate, executionDate },
                { "Q86", "Human health activities", executionDate, executionDate },
                { "Q87", "Residential care activities", executionDate, executionDate },
                { "Q88", "Social work activities without accommodation", executionDate, executionDate },
                { "R90", "Creative, arts and entertainment activities", executionDate, executionDate },
                { "R91", "Libraries, archives, museums and other cultural activities", executionDate, executionDate },
                { "R92", "Gambling and betting activities", executionDate, executionDate },
                { "R93", "Sports activities and amusement and recreation activities", executionDate, executionDate },
                { "S94", "Activities of membership organisations", executionDate, executionDate },
                { "S95", "Repair of computers and personal and household goods", executionDate, executionDate },
                { "S96", "Other personal service activities", executionDate, executionDate },
                { "T97", "Activities of households as employers of domestic personnel", executionDate, executionDate },
                { "T98", "Undifferentiated goods- and services-producing activities of private households for own use", executionDate, executionDate },
                { "U99", "Activities of extraterritorial organisations and bodies", executionDate, executionDate }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "CollectedFees");

            migrationBuilder.DropTable(
                name: "InvoiceHistories");

            migrationBuilder.DropTable(
                name: "JobExecutions");

            migrationBuilder.DropTable(
                name: "ScAdmins");

            migrationBuilder.DropTable(
                name: "WhitelistedTokens");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BusinessActivities");

            migrationBuilder.DropTable(
                name: "LegalForms");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
