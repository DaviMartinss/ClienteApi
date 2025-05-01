using ClienteApi.Core.Domain.Entities;

namespace ClienteApi.Core.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> GetByEmailAsync(string email);
        Task<int> CreateAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
    }

}
