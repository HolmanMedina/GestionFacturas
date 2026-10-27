using GestionFacturas.Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace GestionFacturas.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task EnviarAsync(
        string destinatario,
        string asunto,
        string mensaje,
        byte[]? archivoAdjunto = null,
        string? nombreArchivo = null)
    {
        var email = new MimeMessage();

        email.From.Add(
            MailboxAddress.Parse(
                _settings.From));

        email.To.Add(
            MailboxAddress.Parse(
                destinatario));

        email.Subject = asunto;

        var builder = new BodyBuilder
        {
            TextBody = mensaje
        };

        if (
            archivoAdjunto != null &&
            archivoAdjunto.Length > 0 &&
            !string.IsNullOrWhiteSpace(nombreArchivo))
        {
            builder.Attachments.Add(
                nombreArchivo,
                archivoAdjunto,
                new ContentType(
                    "application",
                    "pdf"));
        }

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}