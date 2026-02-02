using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropostaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "propostas");

            migrationBuilder.CreateTable(
                name: "Propostas",
                schema: "propostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ClienteCpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    SeguroId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoSeguro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ValorCobertura = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorPremio = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Propostas",
                schema: "propostas");
        }
    }
}
