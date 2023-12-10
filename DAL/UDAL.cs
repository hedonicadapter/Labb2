using Labb2Clean.db;
using Labb2Clean.Models;
using MongoDB.Driver;

namespace Labb2Clean.DAL;

public class UDAL : TDAL<User>
{
    public UDAL(Mongo mongo) : base(mongo, "Users"){}
    
    public async Task<User?> GetUserByCredentials(string username, string? password)
    {
        // Omit and encrypt passwords in real world situations && only select fields you need
        return password == null ? 
            await _collection.Find(doc => doc.Username == username).SingleOrDefaultAsync() 
            : await _collection.Find(doc => doc.Username == username && doc.Password == password).SingleOrDefaultAsync();
    }
    
    public async Task<User?> GetUser(string username)
    {
        return await GetUserByCredentials(username, null);
    }
    public async Task<User?> GetUser(string username, string password)
    {
        return await GetUserByCredentials(username, password);
    }

    private async Task<List<User>?> GetUsers()
    {
        return await GetAll();
    }
}