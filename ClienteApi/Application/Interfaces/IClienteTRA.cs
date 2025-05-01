using ClienteApi.Application.DTOs;

namespace ClienteApi.Application.Interfaces
{
    public interface IClienteTRA
    {
        Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> CreateClienteAsync(ClienteRequest request);

        Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> UpdateClienteAsync(int id, ClienteRequest request);

        Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> GetClienteByIdAsync(int id);
    }
}
