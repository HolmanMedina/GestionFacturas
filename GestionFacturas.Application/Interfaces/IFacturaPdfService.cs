using GestionFacturas.Domain.Entities;

namespace GestionFacturas.Application.Interfaces;

public interface IFacturaPdfService
{
    byte[] GenerarFacturaPdf(
        Factura factura,
        Cliente cliente);
}