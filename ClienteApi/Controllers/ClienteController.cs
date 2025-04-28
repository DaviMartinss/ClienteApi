using ClienteApi.Data;
using ClienteApi.Models;
using ClienteApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClienteApi.Controllers
{

    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteRepository _repository;
        private readonly ViaCepService _viaCepService;

        public ClienteController(ClienteRepository repository, ViaCepService viaCepService)
        {
            _repository = repository;
            _viaCepService = viaCepService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            try
            {
                // Verifica email duplicado
                var existing = await _repository.GetByEmailAsync(cliente.Email);
                if (existing != null)
                    return BadRequest(new { message = "Email já cadastrado." });

                // Consulta o ViaCEP
                var (success, endereco) = await _viaCepService.GetEnderecoAsync(cliente.Cep);
                if (!success)
                    return BadRequest(new { message = "CEP inválido." });

                cliente.Logradouro = endereco!.Logradouro;
                cliente.Bairro = endereco.Bairro;
                cliente.Cidade = endereco.Localidade;
                cliente.Estado = endereco.Uf;

                var id = await _repository.CreateAsync(cliente);

                return CreatedAtAction(nameof(GetClienteById), new { id = id }, cliente);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ocorreu um erro. Tente novamente mais tarde." });
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
                    return BadRequest(new { message = "CEP inválido." });

                cliente.Id = id;
                cliente.Logradouro = endereco!.Logradouro;
                cliente.Bairro = endereco.Bairro;
                cliente.Cidade = endereco.Localidade;
                cliente.Estado = endereco.Uf;

                await _repository.UpdateAsync(cliente);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ocorreu um erro. Tente novamente mais tarde." });
            }
        }
    }
}