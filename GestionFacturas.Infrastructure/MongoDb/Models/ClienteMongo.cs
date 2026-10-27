using MongoDB.Bson.Serialization.Attributes;

namespace GestionFacturas.Infrastructure.MongoDb.Models;

public class ClienteMongo
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("activo")]
    public bool Activo { get; set; }
}