using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroFarm.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUTOR",
                columns: table => new
                {
                    ID_PRODUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTOR", x => x.ID_PRODUTOR);
                });

            migrationBuilder.CreateTable(
                name: "PROPRIEDADE",
                columns: table => new
                {
                    ID_PROPRIEDADE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PRODUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME_FAZENDA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AREA_HECTARES = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MUNICIPIO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DT_REGISTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROPRIEDADE", x => x.ID_PROPRIEDADE);
                    table.ForeignKey(
                        name: "FK_PROPRIEDADE_PRODUTOR_ID_PRODUTOR",
                        column: x => x.ID_PRODUTOR,
                        principalTable: "PRODUTOR",
                        principalColumn: "ID_PRODUTOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ALERTA",
                columns: table => new
                {
                    ID_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PROPRIEDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_ALERTA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NIVEL_RISCO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DT_ALERTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTA", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_ALERTA_PROPRIEDADE_ID_PROPRIEDADE",
                        column: x => x.ID_PROPRIEDADE,
                        principalTable: "PROPRIEDADE",
                        principalColumn: "ID_PROPRIEDADE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LEITURA_SATELITAL",
                columns: table => new
                {
                    ID_LEITURA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PROPRIEDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DT_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NDVI = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TEMPERATURA = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    UMIDADE = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    PRECIPITACAO = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    FONTE_SATELITE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURA_SATELITAL", x => x.ID_LEITURA);
                    table.ForeignKey(
                        name: "FK_LEITURA_SATELITAL_PROPRIEDADE_ID_PROPRIEDADE",
                        column: x => x.ID_PROPRIEDADE,
                        principalTable: "PROPRIEDADE",
                        principalColumn: "ID_PROPRIEDADE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_ID_PROPRIEDADE",
                table: "ALERTA",
                column: "ID_PROPRIEDADE");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SATELITAL_ID_PROPRIEDADE",
                table: "LEITURA_SATELITAL",
                column: "ID_PROPRIEDADE");

            migrationBuilder.CreateIndex(
                name: "IX_PROPRIEDADE_ID_PRODUTOR",
                table: "PROPRIEDADE",
                column: "ID_PRODUTOR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTA");

            migrationBuilder.DropTable(
                name: "LEITURA_SATELITAL");

            migrationBuilder.DropTable(
                name: "PROPRIEDADE");

            migrationBuilder.DropTable(
                name: "PRODUTOR");
        }
    }
}
