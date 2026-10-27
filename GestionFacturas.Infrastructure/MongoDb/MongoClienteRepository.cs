using GestionFacturas.Application.Interfaces;
using GestionFacturas.Domain.Entities;
using GestionFacturas.Infrastructure.MongoDb.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GestionFacturas.Infrastructure.MongoDb;

public class MongoClienteRepository : IClienteRepository
{
    private readonly IMongoCollection<ClienteMongo> _clientes;

    public MongoClienteRepository(
        IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var client = new MongoClient(
            settings.ConnectionString);

        var database = client.GetDatabase(
            settings.DatabaseName);

        _clientes = database.GetCollection<ClienteMongo>(
            settings.ClientesCollectionName);
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        var documentos =
            await _clientes
                .Find(_ => true)
                .ToListAsync();

        return documentos
            .Select(Mapear)
            .ToList();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(string id)
    {
        var documento =
            await _clientes
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

        if (documento == null)
        {
            return null;
        }

        return Mapear(documento);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _clientes
            .Find(c => c.Email == email)
            .AnyAsync();
    }

    public async Task CrearAsync(Cliente cliente)
    {
        var documento = new ClienteMongo
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            Activo = cliente.Activo
        };

        await _clientes.InsertOneAsync(documento);
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        var filtro = Builders<ClienteMongo>
            .Filter
            .Eq(c => c.Id, cliente.Id);

        var actualizacion =
            Builders<ClienteMongo>
                .Update
                .Set(c => c.Nombre, cliente.Nombre)
                .Set(c => c.Email, cliente.Email)
                .Set(c => c.Activo, cliente.Activo);

        await _clientes.UpdateOneAsync(
            filtro,
            actualizacion);
    }

    public async Task EliminarAsync(string id)
    {
        var filtro = Builders<ClienteMongo>
            .Filter
            .Eq(c => c.Id, id);

        await _clientes.DeleteOneAsync(filtro);
    }

    private static Cliente Mapear(ClienteMongo cliente)
    {
        return new Cliente
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            Activo = cliente.Activo
        };
    }
}