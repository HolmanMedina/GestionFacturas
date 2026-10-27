namespace GestionFacturas.Application.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(
        string destinatario,
        string asunto,
        string mensaje,
        byte[]? archivoAdjunto = null,
        string? nombreArchivo = null);
}