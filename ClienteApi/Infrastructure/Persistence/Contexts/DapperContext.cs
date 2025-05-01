using System.Data;
using Npgsql;
using ClienteApi.Core.Domain.Interfaces;

namespace ClienteApi.Infrastructure.Persistence.Contexts
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("PostgreSQL");
            return new NpgsqlConnection(connectionString);
        }
    }
}