using ClienteApi.Core.Domain.Entities;
using ClienteApi.Core.Domain.Interfaces;
using ClienteApi.Infrastructure.Persistence.Interfaces;
using Dapper;

namespace ClienteApi.Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDapperContext _context;

        public ClienteRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM clientes WHERE id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Cliente>(query, new { Id = id });
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            try
            {
                var query = "SELECT * FROM Clientes WHERE email = @Email";
                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Cliente>(query, new { Email = email });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateAsync(Cliente cliente)
        {
            try
            {
                var query = @"
                INSERT INTO clientes (nome, email, cep, logradouro, bairro, cidade, estado)
                VALUES (@Nome, @Email, @Cep, @Logradouro, @Bairro, @Cidade, @Estado)
                RETURNING id;
            ";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, cliente);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            try
            {
                var query = @"
                UPDATE clientes
                SET nome = @Nome, email = @Email, cep = @Cep, 
                    logradouro = @Logradouro, bairro = @Bairro,
                    cidade = @Cidade, estado = @Estado
                WHERE id = @Id;
            ";
                using var connection = _context.CreateConnection();
                await connection.ExecuteAsync(query, cliente);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}