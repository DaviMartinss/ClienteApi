using System.Data;

namespace ClienteApi.Infrastructure.Persistence.Interfaces
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
