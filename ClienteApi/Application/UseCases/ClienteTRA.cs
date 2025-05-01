using ClienteApi.Application.DTOs;
using ClienteApi.Application.Interfaces;
using ClienteApi.Application.Mappings;
using ClienteApi.Core.Domain.Entities;
using ClienteApi.Core.Domain.Enums;
using ClienteApi.Core.Domain.Interfaces;
using ClienteApi.Core.Util.Languages;

namespace ClienteApi.Application.UseCases
{
    public class ClienteTRA : IClienteTRA
    {
        private readonly IClienteRepository _repository;
        private readonly IViaCepService _viaCepService;
        private readonly IRabbitMQProducer _producer;

        public ClienteTRA(IClienteRepository repository, IViaCepService viaCepService, IRabbitMQProducer producer)
        {
            _repository = repository;
            _viaCepService = viaCepService;
            _producer = producer;
        }

        public async Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> CreateClienteAsync(ClienteRequest request)
        {
            try
            {
                var existing = await _repository.GetByEmailAsync(request.Email);
                if (existing != null)
                {
                    return (false, null, new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.EmailAlreadyRegistered,
                        Message = ClienteMsg.EX0001
                    });
                }

                var cliente = ClienteMapping.ToEntity(request);
                var (sucesso, erro) = await PreencherEnderecoAsync(cliente);

                if (!sucesso)
                    return (false, null, new ApiErrorResponse { ErrorCode = ApiErrorCode.CepNotFound, Message = erro });

                var id = await _repository.CreateAsync(cliente);
                _producer.SendMessage(cliente);

                return (true, ClienteMapping.ToResponse(cliente), null);
            }
            catch (Exception)
            {
                return (false, null, new ApiErrorResponse
                {
                    ErrorCode = ApiErrorCode.InternalServer,
                    Message = ClienteApiResponseMsg.EX0001
                });
            }
        }

        public async Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> UpdateClienteAsync(int id, ClienteRequest request)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return (false, null, new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.ClientNotFound,
                        Message = ClienteMsg.EX0003
                    });

                existing.Nome = request.Nome;
                existing.Email = request.Email;
                existing.Cep = request.Cep;

                var (sucesso, erro) = await PreencherEnderecoAsync(existing);
                if (!sucesso)
                    return (false, null, new ApiErrorResponse { ErrorCode = ApiErrorCode.CepNotFound, Message = erro });

                await _repository.UpdateAsync(existing);

                return (true, ClienteMapping.ToResponse(existing), null);
            }
            catch (Exception)
            {
                return (false, null, new ApiErrorResponse
                {
                    ErrorCode = ApiErrorCode.InternalServer,
                    Message = ClienteApiResponseMsg.EX0001
                });
            }
        }

        public async Task<(bool Success, ClienteResponse? Cliente, ApiErrorResponse? Error)> GetClienteByIdAsync(int id)
        {
            try
            {
                var cliente = await _repository.GetByIdAsync(id);
                if (cliente == null)
                    return (false, null, new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.ClientNotFound,
                        Message = ClienteMsg.EX0003
                    });

                return (true, ClienteMapping.ToResponse(cliente), null);
            }
            catch (Exception)
            {
                return (false, null, new ApiErrorResponse
                {
                    ErrorCode = ApiErrorCode.InternalServer,
                    Message = ClienteApiResponseMsg.EX0001
                });
            }
        }

        private async Task<(bool Success, string? ErrorMessage)> PreencherEnderecoAsync(Cliente cliente)
        {
            var (success, endereco) = await _viaCepService.GetEnderecoAsync(cliente.Cep);
            if (!success || endereco == null)
                return (false, ClienteMsg.EX0002);

            cliente.Logradouro = endereco.Logradouro;
            cliente.Bairro = endereco.Bairro;
            cliente.Cidade = endereco.Localidade;
            cliente.Estado = endereco.Uf;

            return (true, null);
        }
    }
}