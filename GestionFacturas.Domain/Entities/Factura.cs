using GestionFacturas.Domain.Enums;

namespace GestionFacturas.Domain.Entities;

public class Factura
{
    public string Id { get; set; } = string.Empty;

    public string ClienteId { get; set; } = string.Empty;

    public string NumeroFactura { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public EstadoFactura Estado { get; set; }
}