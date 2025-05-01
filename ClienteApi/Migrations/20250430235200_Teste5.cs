using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClienteApi.Migrations
{
    /// <inheritdoc />
    public partial class Teste5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_cliente",
                table: "cliente");

            migrationBuilder.RenameTable(
                name: "cliente",
                newName: "clientes");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "clientes",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Logradouro",
                table: "clientes",
                newName: "logradouro");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "clientes",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "clientes",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "clientes",
                newName: "cidade");

            migrationBuilder.RenameColumn(
                name: "Cep",
                table: "clientes",
                newName: "cep");

            migrationBuilder.RenameColumn(
                name: "Bairro",
                table: "clientes",
                newName: "bairro");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "clientes",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_clientes",
                table: "clientes",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_clientes",
                table: "clientes");

            migrationBuilder.RenameTable(
                name: "clientes",
                newName: "cliente");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "cliente",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "logradouro",
                table: "cliente",
                newName: "Logradouro");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "cliente",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "cliente",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "cidade",
                table: "cliente",
                newName: "Cidade");

            migrationBuilder.RenameColumn(
                name: "cep",
                table: "cliente",
                newName: "Cep");

            migrationBuilder.RenameColumn(
                name: "bairro",
                table: "cliente",
                newName: "Bairro");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "cliente",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cliente",
                table: "cliente",
                column: "Id");
        }
    }
}
