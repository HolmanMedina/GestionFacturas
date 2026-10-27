using GestionFacturas.Application.Interfaces;
using GestionFacturas.Application.Services;
using GestionFacturas.Domain.Entities;
using GestionFacturas.Domain.Enums;
using Moq;

namespace GestionFacturas.Tests;

public class FacturaServiceTests
{
    [Fact]
    public async Task ProcesarRecordatorios_PrimerRecordatorio_EnviaCorreoYActualizaEstado()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura = new Factura
        {
            Id = "FAC001",
            ClienteId = "CLI001",
            NumeroFactura = "FAC-001",
            Descripcion = "Servicio de prueba",
            Valor = 150000,
            FechaVencimiento = DateTime.Now,
            Estado = EstadoFactura.PrimerRecordatorio
        };

        var cliente = new Cliente
        {
            Id = "CLI001",
            Nombre = "Juan Perez",
            Email = "juan@example.com",
            Activo = true
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura> { factura });

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI001"))
            .ReturnsAsync(cliente);

        facturaPdfService
            .Setup(x => x.GenerarFacturaPdf(
                factura,
                cliente))
            .Returns(new byte[] { 1, 2, 3 });

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                factura,
                cliente),
            Times.Once);

        emailService.Verify(
            x => x.EnviarAsync(
                "juan@example.com",
                "Segundo recordatorio de pago",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-001.pdf"),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC001",
                EstadoFactura.SegundoRecordatorio),
            Times.Once);
    }


    [Fact]
    public async Task ProcesarRecordatorios_SegundoRecordatorio_EnviaCorreoYDesactiva()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura = new Factura
        {
            Id = "FAC003",
            ClienteId = "CLI002",
            NumeroFactura = "FAC-003",
            Descripcion = "Servicio de prueba",
            Valor = 250000,
            FechaVencimiento = DateTime.Now,
            Estado = EstadoFactura.SegundoRecordatorio
        };

        var cliente = new Cliente
        {
            Id = "CLI002",
            Nombre = "Maria Gomez",
            Email = "maria@example.com",
            Activo = true
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura> { factura });

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI002"))
            .ReturnsAsync(cliente);

        facturaPdfService
            .Setup(x => x.GenerarFacturaPdf(
                factura,
                cliente))
            .Returns(new byte[] { 1, 2, 3 });

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                factura,
                cliente),
            Times.Once);

        emailService.Verify(
            x => x.EnviarAsync(
                "maria@example.com",
                "Aviso de desactivación",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-003.pdf"),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC003",
                EstadoFactura.Desactivado),
            Times.Once);
    }


    [Fact]
    public async Task ProcesarRecordatorios_PendienteVencida_EnviaPrimerRecordatorio()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura = new Factura
        {
            Id = "FAC002",
            ClienteId = "CLI001",
            NumeroFactura = "FAC-002",
            Descripcion = "Factura vencida",
            Valor = 200000,
            FechaVencimiento = DateTime.Now.AddDays(-5),
            Estado = EstadoFactura.Pendiente
        };

        var cliente = new Cliente
        {
            Id = "CLI001",
            Nombre = "Juan Perez",
            Email = "juan@example.com",
            Activo = true
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura> { factura });

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI001"))
            .ReturnsAsync(cliente);

        facturaPdfService
            .Setup(x => x.GenerarFacturaPdf(
                factura,
                cliente))
            .Returns(new byte[] { 1, 2, 3 });

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                factura,
                cliente),
            Times.Once);

        emailService.Verify(
            x => x.EnviarAsync(
                "juan@example.com",
                "Primer recordatorio de pago",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-002.pdf"),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC002",
                EstadoFactura.PrimerRecordatorio),
            Times.Once);
    }


    [Fact]
    public async Task ProcesarRecordatorios_PendienteNoVencida_NoEnviaCorreo()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura = new Factura
        {
            Id = "FAC004",
            ClienteId = "CLI001",
            NumeroFactura = "FAC-004",
            Descripcion = "Factura pendiente",
            Valor = 175000,
            FechaVencimiento = DateTime.Now.AddDays(5),
            Estado = EstadoFactura.Pendiente
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura> { factura });

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        emailService.Verify(
            x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>()),
            Times.Never);

        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                It.IsAny<Factura>(),
                It.IsAny<Cliente>()),
            Times.Never);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                It.IsAny<string>(),
                It.IsAny<EstadoFactura>()),
            Times.Never);
    }


    [Fact]
    public async Task ProcesarRecordatorios_ClienteNoExiste_NoEnviaCorreo()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura = new Factura
        {
            Id = "FAC007",
            ClienteId = "CLI999",
            NumeroFactura = "FAC-007",
            Descripcion = "Factura de prueba",
            Valor = 100000,
            FechaVencimiento = DateTime.Now.AddDays(-3),
            Estado = EstadoFactura.PrimerRecordatorio
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura> { factura });

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI999"))
            .ReturnsAsync((Cliente?)null);

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert
        emailService.Verify(
            x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>()),
            Times.Never);

        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                It.IsAny<Factura>(),
                It.IsAny<Cliente>()),
            Times.Never);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                It.IsAny<string>(),
                It.IsAny<EstadoFactura>()),
            Times.Never);
    }


    [Fact]
    public async Task ProcesarRecordatorios_VariasFacturas_ProcesaCadaUnaCorrectamente()
    {
        // Arrange
        var facturaRepository = new Mock<IFacturaRepository>();
        var clienteRepository = new Mock<IClienteRepository>();
        var emailService = new Mock<IEmailService>();
        var facturaPdfService = new Mock<IFacturaPdfService>();

        var factura1 = new Factura
        {
            Id = "FAC001",
            ClienteId = "CLI001",
            NumeroFactura = "FAC-001",
            Descripcion = "Factura uno",
            Valor = 150000,
            FechaVencimiento = DateTime.Now,
            Estado = EstadoFactura.PrimerRecordatorio
        };

        var factura2 = new Factura
        {
            Id = "FAC003",
            ClienteId = "CLI002",
            NumeroFactura = "FAC-003",
            Descripcion = "Factura dos",
            Valor = 250000,
            FechaVencimiento = DateTime.Now,
            Estado = EstadoFactura.SegundoRecordatorio
        };

        var factura3 = new Factura
        {
            Id = "FAC005",
            ClienteId = "CLI003",
            NumeroFactura = "FAC-005",
            Descripcion = "Factura tres",
            Valor = 180000,
            FechaVencimiento = DateTime.Now,
            Estado = EstadoFactura.PrimerRecordatorio
        };

        var cliente1 = new Cliente
        {
            Id = "CLI001",
            Nombre = "Juan Perez",
            Email = "juan@example.com",
            Activo = true
        };

        var cliente2 = new Cliente
        {
            Id = "CLI002",
            Nombre = "Maria Gomez",
            Email = "maria@example.com",
            Activo = true
        };

        var cliente3 = new Cliente
        {
            Id = "CLI003",
            Nombre = "Carlos Rodriguez",
            Email = "carlos@example.com",
            Activo = true
        };

        facturaRepository
            .Setup(x => x.ObtenerFacturasParaProcesarAsync())
            .ReturnsAsync(new List<Factura>
            {
                factura1,
                factura2,
                factura3
            });

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI001"))
            .ReturnsAsync(cliente1);

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI002"))
            .ReturnsAsync(cliente2);

        clienteRepository
            .Setup(x => x.ObtenerPorIdAsync("CLI003"))
            .ReturnsAsync(cliente3);

        facturaPdfService
            .Setup(x => x.GenerarFacturaPdf(
                It.IsAny<Factura>(),
                It.IsAny<Cliente>()))
            .Returns(new byte[] { 1, 2, 3 });

        var service = new FacturaService(
            facturaRepository.Object,
            clienteRepository.Object,
            emailService.Object,
            facturaPdfService.Object);

        // Act
        await service.ProcesarRecordatoriosAsync();

        // Assert

        emailService.Verify(
            x => x.EnviarAsync(
                "juan@example.com",
                "Segundo recordatorio de pago",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-001.pdf"),
            Times.Once);

        emailService.Verify(
            x => x.EnviarAsync(
                "maria@example.com",
                "Aviso de desactivación",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-003.pdf"),
            Times.Once);

        emailService.Verify(
            x => x.EnviarAsync(
                "carlos@example.com",
                "Segundo recordatorio de pago",
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                "Factura-FAC-005.pdf"),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC001",
                EstadoFactura.SegundoRecordatorio),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC003",
                EstadoFactura.Desactivado),
            Times.Once);

        facturaRepository.Verify(
            x => x.ActualizarEstadoAsync(
                "FAC005",
                EstadoFactura.SegundoRecordatorio),
            Times.Once);

        facturaPdfService.Verify(
            x => x.GenerarFacturaPdf(
                It.IsAny<Factura>(),
                It.IsAny<Cliente>()),
            Times.Exactly(3));
    }
}