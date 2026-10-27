using GestionFacturas.Application.Services;
using GestionFacturas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionFacturas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly FacturaService _facturaService;

    public FacturasController(
        FacturaService facturaService)
    {
        _facturaService = facturaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var facturas =
            await _facturaService
                .ObtenerTodasAsync();

        return Ok(facturas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(
        string id)
    {
        var factura =
            await _facturaService
                .ObtenerPorIdAsync(id);

        if (factura == null)
        {
            return NotFound(new
            {
                mensaje = "Factura no encontrada."
            });
        }

        return Ok(factura);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> ObtenerPorCliente(
        string clienteId)
    {
        var facturas =
            await _facturaService
                .ObtenerPorClienteAsync(clienteId);

        return Ok(facturas);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] Factura factura)
    {
        try
        {
            await _facturaService
                .CrearAsync(factura);

            return Ok(new
            {
                mensaje =
                    "Factura creada correctamente.",
                factura
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
        [FromBody] Factura factura)
    {
        try
        {
            await _facturaService
                .ActualizarAsync(id, factura);

            var actualizada =
                await _facturaService
                    .ObtenerPorIdAsync(id);

            return Ok(new
            {
                mensaje =
                    "Factura actualizada correctamente.",
                factura = actualizada
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
            await _facturaService
                .EliminarAsync(id);

            return Ok(new
            {
                mensaje =
                    "Factura eliminada correctamente."
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

    [HttpPost("procesar-recordatorios")]
    public async Task<IActionResult>
        ProcesarRecordatorios()
    {
        await _facturaService
            .ProcesarRecordatoriosAsync();

        return Ok(new
        {
            mensaje =
                "Proceso de recordatorios ejecutado correctamente."
        });
    }

    [HttpPost("{id}/procesar-recordatorio")]
    public async Task<IActionResult>
        ProcesarRecordatorioIndividual(
            string id)
    {
        try
        {
            await _facturaService
                .ProcesarRecordatorioAsync(id);

            return Ok(new
            {
                mensaje =
                    "Recordatorio procesado correctamente."
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
}