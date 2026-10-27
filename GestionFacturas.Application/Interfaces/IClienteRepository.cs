using GestionFacturas.Domain.Entities;

namespace GestionFacturas.Application.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> ObtenerTodosAsync();

    Task<Cliente?> ObtenerPorIdAsync(string id);

    Task<bool> ExisteEmailAsync(string email);

    Task CrearAsync(Cliente cliente);

    Task ActualizarAsync(Cliente cliente);

    Task EliminarAsync(string id);
}