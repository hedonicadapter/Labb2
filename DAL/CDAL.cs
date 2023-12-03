using Labb2Clean.db;
using Labb2Clean.Models;
using MongoDB.Driver;

namespace Labb2Clean.DAL;

public class CDAL : TDAL<Cart>
{
    public CDAL(Mongo mongo) : base(mongo, "Carts"){}
    
    public async Task<Cart?> GetCartByOwner(string owner)
    {
        return await _collection.Find(doc => doc.Owner == owner).SingleOrDefaultAsync();
    }
}