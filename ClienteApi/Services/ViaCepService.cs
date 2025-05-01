using ClienteApi.Services.Interfaces;

namespace ClienteApi.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;

        public ViaCepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, ViaCepResponse? Data)> GetEnderecoAsync(string cep)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ViaCepResponse>($"https://viacep.com.br/ws/{cep}/json/");
                if (response == null || response.Erro)
                    return (false, null);

                return (true, response);
            }
            catch
            {
                return (false, null);
            }
        }


        public class ViaCepResponse
        {
            public string Logradouro { get; set; } = string.Empty;
            public string Bairro { get; set; } = string.Empty;
            public string Localidade { get; set; } = string.Empty;
            public string Uf { get; set; } = string.Empty;
            public bool Erro { get; set; }
        }
    }
}