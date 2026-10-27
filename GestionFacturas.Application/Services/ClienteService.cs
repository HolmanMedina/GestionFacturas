using GestionFacturas.Application.Interfaces;
using GestionFacturas.Domain.Entities;

namespace GestionFacturas.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync()
    {
        return await _clienteRepository.ObtenerTodosAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(string id)
    {
        return await _clienteRepository.ObtenerPorIdAsync(id);
    }

    public async Task CrearAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ArgumentException(
                "El nombre del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(cliente.Email))
            throw new ArgumentException(
                "El email del cliente es obligatorio.");

        var existeEmail =
            await _clienteRepository
                .ExisteEmailAsync(cliente.Email);

        if (existeEmail)
            throw new ArgumentException(
                "Ya existe un cliente registrado con ese email.");

        var clientes =
            await _clienteRepository.ObtenerTodosAsync();

        var siguienteNumero = 1;

        if (clientes.Count > 0)
        {
            var numeros = clientes
                .Where(c => c.Id.StartsWith("CLI"))
                .Select(c =>
                {
                    var numeroTexto =
                        c.Id.Replace("CLI", "");

                    return int.TryParse(
                        numeroTexto,
                        out var numero)
                        ? numero
                        : 0;
                });

            if (numeros.Any())
            {
                siguienteNumero = numeros.Max() + 1;
            }
        }

        cliente.Id =
            $"CLI{siguienteNumero:D3}";

        cliente.Activo = true;

        await _clienteRepository
            .CrearAsync(cliente);
    }

    public async Task ActualizarAsync(
        string id,
        Cliente cliente)
    {
        var existente =
            await _clienteRepository
                .ObtenerPorIdAsync(id);

        if (existente == null)
        {
            throw new ArgumentException(
                "Cliente no encontrado.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Nombre))
        {
            throw new ArgumentException(
                "El nombre del cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(cliente.Email))
        {
            throw new ArgumentException(
                "El email del cliente es obligatorio.");
        }

        if (!string.Equals(
                existente.Email,
                cliente.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            var emailExiste =
                await _clienteRepository
                    .ExisteEmailAsync(cliente.Email);

            if (emailExiste)
            {
                throw new ArgumentException(
                    "Ya existe otro cliente con ese email.");
            }
        }

        existente.Nombre = cliente.Nombre;
        existente.Email = cliente.Email;
        existente.Activo = cliente.Activo;

        await _clienteRepository
            .ActualizarAsync(existente);
    }

    public async Task EliminarAsync(string id)
    {
        var cliente =
            await _clienteRepository
                .ObtenerPorIdAsync(id);

        if (cliente == null)
        {
            throw new ArgumentException(
                "Cliente no encontrado.");
        }

        await _clienteRepository
            .EliminarAsync(id);
    }
}