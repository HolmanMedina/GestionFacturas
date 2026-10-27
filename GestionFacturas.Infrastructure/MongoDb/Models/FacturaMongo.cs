using MongoDB.Bson.Serialization.Attributes;

namespace GestionFacturas.Infrastructure.MongoDb.Models;

public class FacturaMongo
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("clienteId")]
    public string ClienteId { get; set; } = string.Empty;

    [BsonElement("numeroFactura")]
    public string NumeroFactura { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [BsonElement("valor")]
    public decimal Valor { get; set; }

    [BsonElement("fechaVencimiento")]
    public DateTime FechaVencimiento { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = string.Empty;
}