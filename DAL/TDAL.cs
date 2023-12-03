using Labb2Clean.db;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Labb2Clean.DAL;

public class TDAL<T> where T : IGenericMongoDoc
{
    protected readonly IMongoCollection<T> _collection;

    public TDAL(Mongo mongo, string collectionName)
    {
        _collection = mongo.GetCollection<T>(collectionName);
    }
    
    public async Task<List<T>> GetAll()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<T> Get(Guid id)
    {
        return await _collection.Find(doc => doc.Id == id).SingleOrDefaultAsync();
    }

    public void Upsert(T obj)
    {
        _collection.ReplaceOne(doc => doc.Id == obj.Id, obj, new ReplaceOptions { IsUpsert = true });
    }

    public void Delete(Guid? id)
    {
        _collection.DeleteOne(doc => doc.Id == id);
    }
}