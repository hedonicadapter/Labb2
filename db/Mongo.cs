using MongoDB.Driver;

namespace Labb2Clean.db;

public class Mongo
{
    private string _connectionUri = "mongodb+srv://busta:BYhdZc5jv_7cyT9@cluster0.uthxz3x.mongodb.net/DBLabb3?retryWrites=true&w=majority";
    private IMongoDatabase _db;

    public Mongo()
    {
        var settings = MongoClientSettings.FromConnectionString(_connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        
        var client = new MongoClient(settings);
        _db = client.GetDatabase("DBLabb3");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _db.GetCollection<T>(collectionName);
    }
}