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
        private readonly Mock<IClienteTRA> _clienteTRAMock;
        private readonly ClienteController _controller;

        public ClienteControllerTests()
        {
            _clienteTRAMock = new Mock<IClienteTRA>();
            _controller = new ClienteController(_clienteTRAMock.Object);
        }

        [Fact]
        public async Task CreateCliente_EmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new ClienteRequest { Email = "teste@email.com", Cep = "12345678" };
            var error = new ApiErrorResponse { ErrorCode = ApiErrorCode.EmailAlreadyRegistered };

            _clienteTRAMock.Setup(t => t.CreateClienteAsync(request)).ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.CreateCliente(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(badRequest.Value);
            Assert.Equal(ApiErrorCode.EmailAlreadyRegistered, response.ErrorCode);
        }

        [Fact]
        public async Task CreateCliente_Valido_DeveRetornarCreated()
        {
            // Arrange
            var request = new ClienteRequest { Email = "valido@email.com", Cep = "12345678" };
            var response = new ClienteResponse { Id = 1, Email = request.Email };

            _clienteTRAMock.Setup(t => t.CreateClienteAsync(request)).ReturnsAsync((true, response, null));

            // Act
            var result = await _controller.CreateCliente(request);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var cliente = Assert.IsType<ClienteResponse>(created.Value);
            Assert.Equal(1, cliente.Id);
        }

        [Fact]
        public async Task GetClienteById_ClienteExiste_DeveRetornarOkComCliente()
        {
            // Arrange
            var response = new ClienteResponse { Id = 1, Nome = "Teste" };
            _clienteTRAMock.Setup(t => t.GetClienteByIdAsync(1)).ReturnsAsync((true, response, null));

            // Act
            var result = await _controller.GetClienteById(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var cliente = Assert.IsType<ClienteResponse>(ok.Value);
            Assert.Equal(1, cliente.Id);
        }

        [Fact]
        public async Task GetClienteById_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var error = new ApiErrorResponse { ErrorCode = ApiErrorCode.ClientNotFound };
            _clienteTRAMock.Setup(t => t.GetClienteByIdAsync(1)).ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.GetClienteById(1);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(notFound.Value);
            Assert.Equal(ApiErrorCode.ClientNotFound, response.ErrorCode);
        }

        [Fact]
        public async Task UpdateCliente_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var request = new ClienteRequest { Nome = "Novo", Email = "novo@email.com", Cep = "12345678" };
            var error = new ApiErrorResponse { ErrorCode = ApiErrorCode.ClientNotFound };

            _clienteTRAMock.Setup(t => t.UpdateClienteAsync(1, request)).ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.UpdateCliente(1, request);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(notFound.Value);
            Assert.Equal(ApiErrorCode.ClientNotFound, response.ErrorCode);
        }

        [Fact]
        public async Task UpdateCliente_Valido_DeveRetornarOkComCliente()
        {
            // Arrange
            var request = new ClienteRequest { Nome = "Atualizado", Email = "atualizado@email.com", Cep = "87654321" };
            var response = new ClienteResponse { Id = 1, Nome = request.Nome, Email = request.Email };

            _clienteTRAMock.Setup(t => t.UpdateClienteAsync(1, request)).ReturnsAsync((true, response, null));

            // Act
            var result = await _controller.UpdateCliente(1, request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var cliente = Assert.IsType<ClienteResponse>(ok.Value);
            Assert.Equal(request.Email, cliente.Email);
        }
    }
}