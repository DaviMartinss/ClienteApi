using ClienteApi.Application.DTOs;
using ClienteApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClienteApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteTRA _clienteTRA;

        public ClienteController(IClienteTRA clienteTRA)
        {
            _clienteTRA = clienteTRA;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] ClienteRequest request)
        {
            var (success, clienteResponse, error) = await _clienteTRA.CreateClienteAsync(request);

            if (!success)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetClienteById), new { id = clienteResponse?.Id }, clienteResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var (success, clienteResponse, error) = await _clienteTRA.GetClienteByIdAsync(id);

            if (!success)
                return NotFound(error);

            return Ok(clienteResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteRequest request)
        {
            var (success, clienteResponse, error) = await _clienteTRA.UpdateClienteAsync(id, request);

            if (!success)
                return NotFound(error);

            return Ok(clienteResponse);
        }
    }
}