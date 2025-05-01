using ClienteApi.Application.DTOs;
using ClienteApi.Core.Domain.Entities;

namespace ClienteApi.Application.Mappings
{
    public static class ClienteMapping
    {
        public static ClienteResponse ToResponse(Cliente cliente) => new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Cep = cliente.Cep,
            Logradouro = cliente.Logradouro,
            Bairro = cliente.Bairro,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado
        };

        public static Cliente ToEntity(ClienteRequest request) => new Cliente
        {
            Nome = request.Nome,
            Email = request.Email,
            Cep = request.Cep
        };
    }
}