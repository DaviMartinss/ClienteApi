using ClienteApi.Data.Repositories;
using ClienteApi.Models;
using ClienteApi.Models.Responses;
using ClienteApi.Services;
using ClienteApi.Controllers;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ClienteApi.Data.Contexts;
using ClienteApi.Enums;
using System.Data;
using static ClienteApi.Services.ViaCepService;

namespace ClienteApi.Tests.Controllers
{
    public class ClienteControllerTests
    {
        private readonly Mock<ClienteRepository> _repositoryMock;
        private readonly Mock<ViaCepService> _viaCepServiceMock;
        private readonly Mock<RabbitMQProducer> _producerMock;
        private readonly Mock<DapperContext> _dapperContextMock;
        private readonly ClienteController _controller;

        public ClienteControllerTests()
        {
            // Mock do DapperContext e método CreateConnection
            _dapperContextMock = new Mock<DapperContext>();
            var mockConnection = new Mock<IDbConnection>(); // Mock da conexão
            _dapperContextMock.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);

            // Mock do ClienteRepository, passando o DapperContext mockado como dependência
            _repositoryMock = new Mock<ClienteRepository>(_dapperContextMock.Object);
            _viaCepServiceMock = new Mock<ViaCepService>();
            _producerMock = new Mock<RabbitMQProducer>();

            // Criando o controller com os mocks
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
            _repositoryMock.Setup(r => r.GetByEmailAsync(cliente.Email)).ReturnsAsync((Cliente)null!);
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
            _repositoryMock.Setup(r => r.GetByEmailAsync(cliente.Email)).ReturnsAsync((Cliente)null!);
            _viaCepServiceMock.Setup(v => v.GetEnderecoAsync(cliente.Cep))
                .ReturnsAsync((true, new ViaCepResponse
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
            var mockRepo = new Mock<ClienteRepository>(_dapperContextMock.Object);
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);

            var controller = new ClienteController(mockRepo.Object, null!, null!);

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
            var mockRepo = new Mock<ClienteRepository>(_dapperContextMock.Object);
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cliente?)null);

            var controller = new ClienteController(mockRepo.Object, null!, null!);

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

            var mockRepo = new Mock<ClienteRepository>(_dapperContextMock.Object);
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Cliente());

            var mockViaCep = new Mock<ViaCepService>();
            mockViaCep.Setup(s => s.GetEnderecoAsync(cliente.Cep)).ReturnsAsync((false, null!));

            var controller = new ClienteController(mockRepo.Object, mockViaCep.Object, null!);

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
            var mockRepo = new Mock<ClienteRepository>(_dapperContextMock.Object);
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cliente?)null);

            var controller = new ClienteController(mockRepo.Object, null!, null!);

            // Act
            var result = await controller.UpdateCliente(1, new Cliente());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
