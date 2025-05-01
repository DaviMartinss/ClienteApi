using ClienteApi.Models;

namespace ClienteApi.Data.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> GetByEmailAsync(string email);
        Task<int> CreateAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
    }

}
