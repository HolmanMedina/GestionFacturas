using GestionFacturas.Application.Interfaces;
using GestionFacturas.Domain.Entities;
using GestionFacturas.Domain.Enums;

namespace GestionFacturas.Application.Services;

public class FacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly IFacturaPdfService _facturaPdfService;

    public FacturaService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IEmailService emailService,
        IFacturaPdfService facturaPdfService)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _facturaPdfService = facturaPdfService;
    }

    public async Task<List<Factura>> ObtenerTodasAsync()
    {
        return await _facturaRepository
            .ObtenerTodasAsync();
    }

    public async Task<List<Factura>>
        ObtenerPorClienteAsync(string clienteId)
    {
        return await _facturaRepository
            .ObtenerPorClienteAsync(clienteId);
    }

    public async Task<Factura?> ObtenerPorIdAsync(
        string id)
    {
        return await _facturaRepository
            .ObtenerPorIdAsync(id);
    }

    public async Task CrearAsync(Factura factura)
    {
        if (string.IsNullOrWhiteSpace(
            factura.ClienteId))
        {
            throw new ArgumentException(
                "Debe seleccionar un cliente.");
        }

        if (string.IsNullOrWhiteSpace(
            factura.NumeroFactura))
        {
            throw new ArgumentException(
                "El número de factura es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(
            factura.Descripcion))
        {
            throw new ArgumentException(
                "La descripción es obligatoria.");
        }

        if (factura.Valor <= 0)
        {
            throw new ArgumentException(
                "El valor debe ser mayor que cero.");
        }

        var cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(
                    factura.ClienteId);

        if (cliente == null)
        {
            throw new ArgumentException(
                "El cliente seleccionado no existe.");
        }

        var facturas =
            await _facturaRepository
                .ObtenerTodasAsync();

        var siguienteNumero = 1;

        var numeros = facturas
            .Where(f =>
                f.Id.StartsWith("FAC"))
            .Select(f =>
            {
                var texto =
                    f.Id.Replace("FAC", "");

                return int.TryParse(
                    texto,
                    out var numero)
                    ? numero
                    : 0;
            });

        if (numeros.Any())
        {
            siguienteNumero =
                numeros.Max() + 1;
        }

        factura.Id =
            $"FAC{siguienteNumero:D3}";

        factura.Estado =
            EstadoFactura.Pendiente;

        await _facturaRepository
            .CrearAsync(factura);
    }

    public async Task ActualizarAsync(
        string id,
        Factura factura)
    {
        var existente =
            await _facturaRepository
                .ObtenerPorIdAsync(id);

        if (existente == null)
        {
            throw new ArgumentException(
                "Factura no encontrada.");
        }

        var cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(
                    factura.ClienteId);

        if (cliente == null)
        {
            throw new ArgumentException(
                "El cliente seleccionado no existe.");
        }

        if (string.IsNullOrWhiteSpace(
            factura.NumeroFactura))
        {
            throw new ArgumentException(
                "El número de factura es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(
            factura.Descripcion))
        {
            throw new ArgumentException(
                "La descripción es obligatoria.");
        }

        if (factura.Valor <= 0)
        {
            throw new ArgumentException(
                "El valor debe ser mayor que cero.");
        }

        existente.ClienteId =
            factura.ClienteId;

        existente.NumeroFactura =
            factura.NumeroFactura;

        existente.Descripcion =
            factura.Descripcion;

        existente.Valor =
            factura.Valor;

        existente.FechaVencimiento =
            factura.FechaVencimiento;

        existente.Estado =
            factura.Estado;

        await _facturaRepository
            .ActualizarAsync(existente);
    }

    public async Task EliminarAsync(string id)
    {
        var factura =
            await _facturaRepository
                .ObtenerPorIdAsync(id);

        if (factura == null)
        {
            throw new ArgumentException(
                "Factura no encontrada.");
        }

        await _facturaRepository
            .EliminarAsync(id);
    }

    public async Task ProcesarRecordatoriosAsync()
    {
        var facturas =
            await _facturaRepository
                .ObtenerFacturasParaProcesarAsync();

        foreach (var factura in facturas)
        {
            await ProcesarRecordatorioInternoAsync(
                factura);
        }
    }

    public async Task ProcesarRecordatorioAsync(
        string facturaId)
    {
        var factura =
            await _facturaRepository
                .ObtenerPorIdAsync(facturaId);

        if (factura == null)
        {
            throw new ArgumentException(
                "Factura no encontrada.");
        }

        if (factura.Estado == EstadoFactura.Pendiente)
        {
            if (factura.FechaVencimiento.Date >= DateTime.Now.Date)
            {
                throw new ArgumentException(
                    "La factura aún no está vencida.");
            }
        }
        else if (factura.Estado == EstadoFactura.Pendiente &&
    factura.FechaVencimiento > DateTime.Now)
        {
            throw new ArgumentException(
                "La factura todavía no se encuentra vencida.");
        }

        if (factura.Estado != EstadoFactura.Pendiente &&
            factura.Estado != EstadoFactura.PrimerRecordatorio &&
            factura.Estado != EstadoFactura.SegundoRecordatorio)
        {
            throw new ArgumentException(
                "La factura no se encuentra disponible para procesamiento.");
        }

        await ProcesarRecordatorioInternoAsync(
            factura);
    }

    private async Task
    ProcesarRecordatorioInternoAsync(
        Factura factura)
    {
        var cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(
                    factura.ClienteId);

        if (cliente == null)
        {
            return;
        }

        if (factura.Estado ==
            EstadoFactura.Pendiente)
        {
            if (factura.FechaVencimiento > DateTime.Now)
            {
                return;
            }

            var pdf =
                _facturaPdfService
                    .GenerarFacturaPdf(
                        factura,
                        cliente);

            await _emailService.EnviarAsync(
                cliente.Email,
                "Primer recordatorio de pago",
                $"Estimado {cliente.Nombre}, " +
                $"su factura {factura.NumeroFactura} " +
                $"se encuentra vencida. " +
                $"Este es un primer recordatorio de pago. " +
                $"Adjuntamos la factura en formato PDF.",
                pdf,
                $"Factura-{factura.NumeroFactura}.pdf");

            await _facturaRepository
                .ActualizarEstadoAsync(
                    factura.Id,
                    EstadoFactura.PrimerRecordatorio);

            return;
        }

        if (factura.Estado ==
            EstadoFactura.PrimerRecordatorio)
        {
            var pdf =
                _facturaPdfService
                    .GenerarFacturaPdf(
                        factura,
                        cliente);

            await _emailService.EnviarAsync(
                cliente.Email,
                "Segundo recordatorio de pago",
                $"Estimado {cliente.Nombre}, " +
                $"su factura {factura.NumeroFactura} " +
                $"ha pasado a segundo recordatorio. " +
                $"Adjuntamos la factura en formato PDF.",
                pdf,
                $"Factura-{factura.NumeroFactura}.pdf");

            await _facturaRepository
                .ActualizarEstadoAsync(
                    factura.Id,
                    EstadoFactura.SegundoRecordatorio);

            return;
        }

        if (factura.Estado ==
            EstadoFactura.SegundoRecordatorio)
        {
            var pdf =
                _facturaPdfService
                    .GenerarFacturaPdf(
                        factura,
                        cliente);

            await _emailService.EnviarAsync(
                cliente.Email,
                "Aviso de desactivación",
                $"Estimado {cliente.Nombre}, " +
                $"su factura {factura.NumeroFactura} " +
                $"se encuentra en segundo recordatorio " +
                $"y será desactivado. " +
                $"Adjuntamos la factura en formato PDF.",
                pdf,
                $"Factura-{factura.NumeroFactura}.pdf");

            await _facturaRepository
                .ActualizarEstadoAsync(
                    factura.Id,
                    EstadoFactura.Desactivado);

            return;
        }
    }
}