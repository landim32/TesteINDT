using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContratacaoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_PropostaId",
                table: "Contratos",
                column: "PropostaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratos");
        }
    }
}
