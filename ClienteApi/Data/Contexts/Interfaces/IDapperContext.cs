using System.Data;

namespace ClienteApi.Data.Contexts.Interfaces
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
