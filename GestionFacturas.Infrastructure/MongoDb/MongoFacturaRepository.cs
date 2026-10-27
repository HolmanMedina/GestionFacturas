using GestionFacturas.Application.Interfaces;
using GestionFacturas.Domain.Entities;
using GestionFacturas.Domain.Enums;
using GestionFacturas.Infrastructure.MongoDb.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GestionFacturas.Infrastructure.MongoDb;

public class MongoFacturaRepository : IFacturaRepository
{
    private readonly IMongoCollection<FacturaMongo> _facturas;

    public MongoFacturaRepository(
        IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var client =
            new MongoClient(
                settings.ConnectionString);

        var database =
            client.GetDatabase(
                settings.DatabaseName);

        _facturas =
            database.GetCollection<FacturaMongo>(
                settings.CollectionName);
    }

    public async Task<List<Factura>> ObtenerTodasAsync()
    {
        var documentos =
            await _facturas
                .Find(_ => true)
                .ToListAsync();

        return documentos
            .Select(Mapear)
            .ToList();
    }

    public async Task<List<Factura>> ObtenerPorClienteAsync(
        string clienteId)
    {
        var documentos =
            await _facturas
                .Find(f => f.ClienteId == clienteId)
                .ToListAsync();

        return documentos
            .Select(Mapear)
            .ToList();
    }

    public async Task<Factura?> ObtenerPorIdAsync(
        string id)
    {
        var documento =
            await _facturas
                .Find(f => f.Id == id)
                .FirstOrDefaultAsync();

        return documento == null
            ? null
            : Mapear(documento);
    }

    public async Task<List<Factura>>
        ObtenerFacturasParaProcesarAsync()
    {
        var ahora = DateTime.Now;

        var documentos =
            await _facturas
                .Find(f =>
                    (
                        f.Estado == "pendiente" &&
                        f.FechaVencimiento <= ahora
                    )
                    ||
                    f.Estado == "primerrecordatorio"
                    ||
                    f.Estado == "segundorecordatorio")
                .ToListAsync();

        return documentos
            .Select(Mapear)
            .ToList();
    }

    public async Task CrearAsync(Factura factura)
    {
        var documento = new FacturaMongo
        {
            Id = factura.Id,
            ClienteId = factura.ClienteId,
            NumeroFactura = factura.NumeroFactura,
            Descripcion = factura.Descripcion,
            Valor = factura.Valor,
            FechaVencimiento =
                factura.FechaVencimiento,
            Estado = ConvertirEstado(
                factura.Estado)
        };

        await _facturas
            .InsertOneAsync(documento);
    }

    public async Task ActualizarAsync(
        Factura factura)
    {
        var filtro =
            Builders<FacturaMongo>
                .Filter
                .Eq(f => f.Id, factura.Id);

        var actualizacion =
            Builders<FacturaMongo>
                .Update
                .Set(f => f.ClienteId,
                    factura.ClienteId)
                .Set(f => f.NumeroFactura,
                    factura.NumeroFactura)
                .Set(f => f.Descripcion,
                    factura.Descripcion)
                .Set(f => f.Valor,
                    factura.Valor)
                .Set(f => f.FechaVencimiento,
                    factura.FechaVencimiento)
                .Set(f => f.Estado,
                    ConvertirEstado(
                        factura.Estado));

        await _facturas.UpdateOneAsync(
            filtro,
            actualizacion);
    }

    public async Task EliminarAsync(string id)
    {
        var filtro =
            Builders<FacturaMongo>
                .Filter
                .Eq(f => f.Id, id);

        await _facturas.DeleteOneAsync(filtro);
    }

    public async Task ActualizarEstadoAsync(
        string facturaId,
        EstadoFactura nuevoEstado)
    {
        var filtro =
            Builders<FacturaMongo>
                .Filter
                .Eq(f => f.Id, facturaId);

        var actualizacion =
            Builders<FacturaMongo>
                .Update
                .Set(f => f.Estado,
                    ConvertirEstado(nuevoEstado));

        await _facturas.UpdateOneAsync(
            filtro,
            actualizacion);
    }

    private static string ConvertirEstado(
        EstadoFactura estado)
    {
        return estado switch
        {
            EstadoFactura.PrimerRecordatorio =>
                "primerrecordatorio",

            EstadoFactura.SegundoRecordatorio =>
                "segundorecordatorio",

            EstadoFactura.Desactivado =>
                "desactivado",

            EstadoFactura.Pendiente =>
                "pendiente",

            _ => throw new ArgumentOutOfRangeException(
                nameof(estado),
                estado,
                "Estado de factura no válido.")
        };
    }

    private static Factura Mapear(
        FacturaMongo factura)
    {
        var estado = factura.Estado switch
        {
            "primerrecordatorio" =>
                EstadoFactura.PrimerRecordatorio,

            "segundorecordatorio" =>
                EstadoFactura.SegundoRecordatorio,

            "desactivado" =>
                EstadoFactura.Desactivado,

            "pendiente" =>
                EstadoFactura.Pendiente,

            _ => throw new ArgumentException(
                $"Estado no reconocido: {factura.Estado}")
        };

        return new Factura
        {
            Id = factura.Id,
            ClienteId = factura.ClienteId,
            NumeroFactura =
                factura.NumeroFactura,
            Descripcion =
                factura.Descripcion,
            Valor =
                factura.Valor,
            FechaVencimiento =
                factura.FechaVencimiento,
            Estado = estado
        };
    }
}