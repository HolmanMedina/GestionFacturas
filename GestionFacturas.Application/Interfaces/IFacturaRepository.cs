using GestionFacturas.Domain.Entities;
using GestionFacturas.Domain.Enums;

namespace GestionFacturas.Application.Interfaces;

public interface IFacturaRepository
{
    Task<List<Factura>> ObtenerTodasAsync();

    Task<List<Factura>> ObtenerPorClienteAsync(
        string clienteId);

    Task<Factura?> ObtenerPorIdAsync(
        string id);

    Task<List<Factura>> ObtenerFacturasParaProcesarAsync();

    Task CrearAsync(Factura factura);

    Task ActualizarAsync(Factura factura);

    Task EliminarAsync(string id);

    Task ActualizarEstadoAsync(
        string facturaId,
        EstadoFactura nuevoEstado);
}