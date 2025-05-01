using static ClienteApi.Infrastructure.ExternalServices.ViaCepService.Implementations.ViaCepService;

namespace ClienteApi.Infrastructure.ExternalServices.ViaCepService.Interfaces
{
    public interface IViaCepService
    {
        Task<(bool Success, ViaCepResponse? Data)> GetEnderecoAsync(string cep);
    }
}
