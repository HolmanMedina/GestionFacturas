using GestionFacturas.Application.Interfaces;
using GestionFacturas.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionFacturas.Infrastructure.Pdf;

public class FacturaPdfService : IFacturaPdfService
{
    public byte[] GenerarFacturaPdf(
        Factura factura,
        Cliente cliente)
    {
        var documento = Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);

                pagina.Margin(40);

                pagina.DefaultTextStyle(
                    x => x.FontSize(10));

                pagina.Header()
                    .Column(columna =>
                    {
                        columna.Item()
                            .Text("GESTIÓN DE FACTURAS")
                            .FontSize(22)
                            .Bold();

                        columna.Item()
                            .Text("Documento de factura")
                            .FontSize(11);
                    });

                pagina.Content()
                    .PaddingTop(25)
                    .Column(columna =>
                    {
                        columna.Spacing(15);

                        columna.Item()
                            .Text($"FACTURA {factura.NumeroFactura}")
                            .FontSize(18)
                            .Bold();

                        columna.Item()
                            .LineHorizontal(1);

                        columna.Item()
                            .Text("INFORMACIÓN DEL CLIENTE")
                            .FontSize(13)
                            .Bold();

                        columna.Item()
                            .Text(
                                $"Cliente: {cliente.Nombre}");

                        columna.Item()
                            .Text(
                                $"Email: {cliente.Email}");

                        columna.Item()
                            .Text(
                                $"Identificación: {cliente.Id}");

                        columna.Item()
                            .PaddingTop(10)
                            .Text("INFORMACIÓN DE LA FACTURA")
                            .FontSize(13)
                            .Bold();

                        columna.Item()
                            .Text(
                                $"Número de factura: " +
                                $"{factura.NumeroFactura}");

                        columna.Item()
                            .Text(
                                $"Descripción: " +
                                $"{factura.Descripcion}");

                        columna.Item()
                            .Text(
                                $"Fecha de vencimiento: " +
                                $"{factura.FechaVencimiento:dd/MM/yyyy}");

                        columna.Item()
                            .Text(
                                $"Estado: " +
                                ObtenerEstadoTexto(
                                    factura.Estado));

                        columna.Item()
                            .PaddingTop(15)
                            .Background(Colors.Grey.Lighten3)
                            .Padding(15)
                            .Row(fila =>
                            {
                                fila.RelativeItem()
                                    .Text("VALOR TOTAL")
                                    .Bold();

                                fila.ConstantItem(150)
                                    .AlignRight()
                                    .Text(
                                        factura.Valor
                                            .ToString(
                                                "C",
                                                new System.Globalization
                                                    .CultureInfo("es-CO")))
                                    .Bold()
                                    .FontSize(16);
                            });

                        columna.Item()
                            .PaddingTop(25)
                            .Text(
                                "Este documento fue generado " +
                                "automáticamente por el sistema " +
                                "de gestión de facturas.")
                            .FontSize(9);
                    });

                pagina.Footer()
                    .AlignCenter()
                    .Text(
                        $"Factura {factura.NumeroFactura} - " +
                        $"{DateTime.Now:dd/MM/yyyy HH:mm}");
            });
        });

        return documento.GeneratePdf();
    }

    private static string ObtenerEstadoTexto(
        GestionFacturas.Domain.Enums.EstadoFactura estado)
    {
        return estado switch
        {
            GestionFacturas.Domain.Enums.EstadoFactura.Pendiente =>
                "Pendiente",

            GestionFacturas.Domain.Enums.EstadoFactura.PrimerRecordatorio =>
                "Primer recordatorio",

            GestionFacturas.Domain.Enums.EstadoFactura.SegundoRecordatorio =>
                "Segundo recordatorio",

            GestionFacturas.Domain.Enums.EstadoFactura.Desactivado =>
                "Desactivado",

            _ => "Desconocido"
        };
    }
}