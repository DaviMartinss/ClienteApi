using System.Data;

namespace ClienteApi.Core.Domain.Interfaces
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
