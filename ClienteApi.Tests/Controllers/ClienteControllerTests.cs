using ClienteApi.Application.DTOs;
using ClienteApi.Application.Interfaces;
using ClienteApi.Core.Domain.Enums;
using ClienteApi.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClienteApi.Tests.Controllers
{
    public class ClienteControllerTests
    {
        private readonly Mock<IClienteTRA> _clienteTraMock;
        private readonly ClienteController _clienteController;

        public ClienteControllerTests()
        {
            _clienteTraMock = new Mock<IClienteTRA>();
            _clienteController = new ClienteController(_clienteTraMock.Object);
        }

        [Fact]
        public async Task CreateCliente_EmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var clienteRequest = new ClienteRequest { Email = "teste@email.com", Cep = "12345678" };
            var erroEsperado = new ApiErrorResponse { ErrorCode = ApiErrorCode.EmailAlreadyRegistered };

            _clienteTraMock
                .Setup(servico => servico.CreateClienteAsync(clienteRequest))
                .ReturnsAsync((false, null, erroEsperado));

            // Act
            var resultado = await _clienteController.CreateCliente(clienteRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var erroRetornado = Assert.IsType<ApiErrorResponse>(badRequestResult.Value);
            Assert.Equal(ApiErrorCode.EmailAlreadyRegistered, erroRetornado.ErrorCode);
        }

        [Fact]
        public async Task CreateCliente_Valido_DeveRetornarCreated()
        {
            // Arrange
            var clienteRequest = new ClienteRequest { Email = "valido@email.com", Cep = "12345678" };
            var clienteResponse = new ClienteResponse { Id = 1, Email = clienteRequest.Email };

            _clienteTraMock
                .Setup(servico => servico.CreateClienteAsync(clienteRequest))
                .ReturnsAsync((true, clienteResponse, null));

            // Act
            var resultado = await _clienteController.CreateCliente(clienteRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            var clienteCriado = Assert.IsType<ClienteResponse>(createdResult.Value);
            Assert.Equal(1, clienteCriado.Id);
        }

        [Fact]
        public async Task GetClienteById_ClienteExiste_DeveRetornarOkComCliente()
        {
            // Arrange
            var clienteResponse = new ClienteResponse { Id = 1, Nome = "Teste" };

            _clienteTraMock
                .Setup(servico => servico.GetClienteByIdAsync(1))
                .ReturnsAsync((true, clienteResponse, null));

            // Act
            var resultado = await _clienteController.GetClienteById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clienteRetornado = Assert.IsType<ClienteResponse>(okResult.Value);
            Assert.Equal(1, clienteRetornado.Id);
        }

        [Fact]
        public async Task GetClienteById_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var erroEsperado = new ApiErrorResponse { ErrorCode = ApiErrorCode.ClientNotFound };

            _clienteTraMock
                .Setup(servico => servico.GetClienteByIdAsync(1))
                .ReturnsAsync((false, null, erroEsperado));

            // Act
            var resultado = await _clienteController.GetClienteById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            var erroRetornado = Assert.IsType<ApiErrorResponse>(notFoundResult.Value);
            Assert.Equal(ApiErrorCode.ClientNotFound, erroRetornado.ErrorCode);
        }

        [Fact]
        public async Task UpdateCliente_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var clienteRequest = new ClienteRequest { Nome = "Novo", Email = "novo@email.com", Cep = "12345678" };
            var erroEsperado = new ApiErrorResponse { ErrorCode = ApiErrorCode.ClientNotFound };

            _clienteTraMock
                .Setup(servico => servico.UpdateClienteAsync(1, clienteRequest))
                .ReturnsAsync((false, null, erroEsperado));

            // Act
            var resultado = await _clienteController.UpdateCliente(1, clienteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            var erroRetornado = Assert.IsType<ApiErrorResponse>(notFoundResult.Value);
            Assert.Equal(ApiErrorCode.ClientNotFound, erroRetornado.ErrorCode);
        }

        [Fact]
        public async Task UpdateCliente_Valido_DeveRetornarOkComCliente()
        {
            // Arrange
            var clienteRequest = new ClienteRequest
            {
                Nome = "Atualizado",
                Email = "atualizado@email.com",
                Cep = "87654321"
            };
            var clienteAtualizado = new ClienteResponse
            {
                Id = 1,
                Nome = clienteRequest.Nome,
                Email = clienteRequest.Email
            };

            _clienteTraMock
                .Setup(servico => servico.UpdateClienteAsync(1, clienteRequest))
                .ReturnsAsync((true, clienteAtualizado, null));

            // Act
            var resultado = await _clienteController.UpdateCliente(1, clienteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var clienteRetornado = Assert.IsType<ClienteResponse>(okResult.Value);
            Assert.Equal(clienteRequest.Email, clienteRetornado.Email);
        }
    }
}
