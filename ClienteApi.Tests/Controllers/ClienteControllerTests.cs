using ClienteApi.Controllers;
using ClienteApi.Data.Contexts.Interfaces;
using ClienteApi.Data.Repositories.Interfaces;
using ClienteApi.Enums;
using ClienteApi.Models;
using ClienteApi.Models.Responses;
using ClienteApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Data;

namespace ClienteApi.Tests.Controllers
{
    public class ClienteControllerTests
    {
        private readonly Mock<IClienteRepository> _repositoryMock;
        private readonly Mock<IViaCepService> _viaCepServiceMock;
        private readonly Mock<IRabbitMQProducer> _producerMock;
        private readonly Mock<IDapperContext> _dapperContextMock;
        private readonly ClienteController _controller;

        public ClienteControllerTests()
        {
            var mockConnection = new Mock<IDbConnection>();
            _dapperContextMock = new Mock<IDapperContext>();
            _dapperContextMock.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            _repositoryMock = new Mock<IClienteRepository>();
            _viaCepServiceMock = new Mock<IViaCepService>();
            _producerMock = new Mock<IRabbitMQProducer>();

            _controller = new ClienteController(
                _repositoryMock.Object,
                _viaCepServiceMock.Object,
                _producerMock.Object
            );
        }

        [Fact]
        public async Task CreateCliente_EmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var cliente = new Cliente { Email = "teste@email.com", Cep = "12345678" };
            _repositoryMock.Setup(r => r.GetByEmailAsync(cliente.Email)).ReturnsAsync(new Cliente());

            // Act
            var result = await _controller.CreateCliente(cliente);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(badRequest.Value);
            Assert.Equal(ApiErrorCode.EmailAlreadyRegistered, response.ErrorCode);
        }

        [Fact]
        public async Task CreateCliente_CepInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var cliente = new Cliente { Email = "novo@email.com", Cep = "00000000" };
            _repositoryMock.Setup(r => r.GetByEmailAsync(cliente.Email)).ReturnsAsync((Cliente?)null);
            _viaCepServiceMock.Setup(v => v.GetEnderecoAsync(cliente.Cep)).ReturnsAsync((false, null));

            // Act
            var result = await _controller.CreateCliente(cliente);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(badRequest.Value);
            Assert.Equal(ApiErrorCode.CepNotFound, response.ErrorCode);
        }

        [Fact]
        public async Task CreateCliente_Valido_DeveRetornarCreated()
        {
            // Arrange
            var cliente = new Cliente { Email = "valido@email.com", Cep = "12345678" };
            _repositoryMock.Setup(r => r.GetByEmailAsync(cliente.Email)).ReturnsAsync((Cliente?)null);
            _viaCepServiceMock.Setup(v => v.GetEnderecoAsync(cliente.Cep))
                .ReturnsAsync((true, new ClienteApi.Services.ViaCepService.ViaCepResponse
                {
                    Logradouro = "Rua Teste",
                    Bairro = "Centro",
                    Localidade = "Cidade",
                    Uf = "UF"
                }));
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Cliente>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateCliente(cliente);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var clienteRetornado = Assert.IsType<Cliente>(created.Value);
            Assert.Equal("valido@email.com", clienteRetornado.Email);
        }

        [Fact]
        public async Task GetClienteById_ClienteExiste_DeveRetornarOkComCliente()
        {
            // Arrange
            var cliente = new Cliente { Id = 1, Nome = "Teste" };
            var mockRepo = new Mock<IClienteRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
            var controller = new ClienteController(mockRepo.Object, Mock.Of<IViaCepService>(), Mock.Of<IRabbitMQProducer>());

            // Act
            var result = await controller.GetClienteById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(cliente, okResult.Value);
        }

        [Fact]
        public async Task GetClienteById_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IClienteRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cliente?)null);
            var controller = new ClienteController(mockRepo.Object, Mock.Of<IViaCepService>(), Mock.Of<IRabbitMQProducer>());

            // Act
            var result = await controller.GetClienteById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCliente_CepInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var cliente = new Cliente { Cep = "00000000" };
            var mockRepo = new Mock<IClienteRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Cliente());
            var mockViaCep = new Mock<IViaCepService>();
            mockViaCep.Setup(s => s.GetEnderecoAsync(cliente.Cep)).ReturnsAsync((false, null!));
            var controller = new ClienteController(mockRepo.Object, mockViaCep.Object, Mock.Of<IRabbitMQProducer>());

            // Act
            var result = await controller.UpdateCliente(1, cliente);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiErrorResponse>(badRequest.Value);
            Assert.Equal(ApiErrorCode.CepNotFound, response.ErrorCode);
        }

        [Fact]
        public async Task UpdateCliente_ClienteNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IClienteRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cliente?)null);
            var controller = new ClienteController(mockRepo.Object, Mock.Of<IViaCepService>(), Mock.Of<IRabbitMQProducer>());

            // Act
            var result = await controller.UpdateCliente(1, new Cliente());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCliente_Valido_DeveRetornarOkComCliente()
        {
            // Arrange
            var clienteAtualizado = new Cliente
            {
                Id = 1,
                Nome = "Novo Nome",
                Email = "novo@email.com",
                Cep = "12345678"
            };

            var clienteExistente = new Cliente
            {
                Id = 1,
                Nome = "Antigo Nome",
                Email = "antigo@email.com",
                Cep = "87654321"
            };

            var mockRepo = new Mock<IClienteRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(clienteExistente);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);

            var mockViaCep = new Mock<IViaCepService>();
            mockViaCep.Setup(v => v.GetEnderecoAsync(clienteAtualizado.Cep)).ReturnsAsync((true, new ClienteApi.Services.ViaCepService.ViaCepResponse
            {
                Logradouro = "Rua Atualizada",
                Bairro = "Centro",
                Localidade = "Cidade",
                Uf = "UF"
            }));

            var controller = new ClienteController(mockRepo.Object, mockViaCep.Object, Mock.Of<IRabbitMQProducer>());

            // Act
            var result = await controller.UpdateCliente(1, clienteAtualizado);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var clienteRetornado = Assert.IsType<Cliente>(okResult.Value);
            Assert.Equal("Novo Nome", clienteRetornado.Nome);
            Assert.Equal("novo@email.com", clienteRetornado.Email);
        }
    }
}