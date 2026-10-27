using GestionFacturas.Application.Services;
using GestionFacturas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionFacturas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(
        ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var clientes =
            await _clienteService.ObtenerTodosAsync();

        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(
        string id)
    {
        var cliente =
            await _clienteService.ObtenerPorIdAsync(id);

        if (cliente == null)
        {
            return NotFound(new
            {
                mensaje = "Cliente no encontrado."
            });
        }

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] Cliente cliente)
    {
        try
        {
            await _clienteService.CrearAsync(cliente);

            return Ok(new
            {
                mensaje =
                    "Cliente creado correctamente.",
                cliente
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(
        string id,
        [FromBody] Cliente cliente)
    {
        try
        {
            await _clienteService
                .ActualizarAsync(id, cliente);

            var actualizado =
                await _clienteService
                    .ObtenerPorIdAsync(id);

            return Ok(new
            {
                mensaje =
                    "Cliente actualizado correctamente.",
                cliente = actualizado
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(
        string id)
    {
        try
        {
            await _clienteService
                .EliminarAsync(id);

            return Ok(new
            {
                mensaje =
                    "Cliente eliminado correctamente."
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                mensaje = ex.Message
            });
        }
    }
}