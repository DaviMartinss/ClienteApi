using static ClienteApi.Infrastructure.ExternalServices.ViaCepService.ViaCepService;

namespace ClienteApi.Application.Interfaces
{
    public interface IViaCepService
    {
        Task<(bool Success, ViaCepResponse? Data)> GetEnderecoAsync(string cep);
    }
}
