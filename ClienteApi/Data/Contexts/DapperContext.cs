using ClienteApi.Data.Contexts.Interfaces;
using System.Data;
using Npgsql;

namespace ClienteApi.Data.Contexts
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