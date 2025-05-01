using ClienteApi.Data.Repositories;
using ClienteApi.Data.Repositories.Interfaces;
using ClienteApi.Enums;
using ClienteApi.Models;
using ClienteApi.Models.Responses;
using ClienteApi.Services;
using ClienteApi.Services.Interfaces;
using ClienteApi.Util.Languages;
using Microsoft.AspNetCore.Mvc;

namespace ClienteApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _repository;
        private readonly IViaCepService _viaCepService;
        private readonly IRabbitMQProducer _producer;

        public ClienteController(IClienteRepository repository, IViaCepService viaCepService, IRabbitMQProducer producer)
        {
            _repository = repository;
            _viaCepService = viaCepService;
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            try
            {
                // Verifica email duplicado
                var existing = await _repository.GetByEmailAsync(cliente.Email);
                if (existing != null)
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.EmailAlreadyRegistered,
                        Message = ClienteMsg.EX0001
                    });

                // Consulta o ViaCEP
                var (success, endereco) = await _viaCepService.GetEnderecoAsync(cliente.Cep);
                if (!success)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.CepNotFound,
                        Message = ClienteMsg.EX0002
                    });
                }

                cliente.Logradouro = endereco!.Logradouro;
                cliente.Bairro = endereco.Bairro;
                cliente.Cidade = endereco.Localidade;
                cliente.Estado = endereco.Uf;

                var id = await _repository.CreateAsync(cliente);

                _producer.SendMessage(cliente);

                return CreatedAtAction(nameof(GetClienteById), new { id = id }, cliente);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorCode = ApiErrorCode.InternalServer,
                    Message = ClienteApiResponseMsg.EX0001
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] Cliente cliente)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return NotFound();

                // Consulta o ViaCEP
                var (success, endereco) = await _viaCepService.GetEnderecoAsync(cliente.Cep);
                if (!success)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorCode = ApiErrorCode.CepNotFound,
                        Message = ClienteMsg.EX0002
                    });
                }

                cliente.Id = id;
                cliente.Logradouro = endereco!.Logradouro;
                cliente.Bairro = endereco.Bairro;
                cliente.Cidade = endereco.Localidade;
                cliente.Estado = endereco.Uf;

                await _repository.UpdateAsync(cliente);

                return Ok(cliente);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorCode = ApiErrorCode.InternalServer,
                    Message = ClienteApiResponseMsg.EX0001
                });
            }
        }
    }
}
