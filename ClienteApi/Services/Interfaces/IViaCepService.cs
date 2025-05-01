using static ClienteApi.Services.ViaCepService;

namespace ClienteApi.Services.Interfaces
{
    public interface IViaCepService
    {
        Task<(bool Success, ViaCepResponse? Data)> GetEnderecoAsync(string cep);
    }
}
