using MongoDB.Bson.Serialization.Attributes;

namespace Labb2Clean;

public interface IGenericMongoDoc
{
    [BsonId]
    public Guid Id { get; set; }
}