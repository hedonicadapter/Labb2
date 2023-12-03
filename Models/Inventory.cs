namespace Labb2Clean.Models;

public class Inventory : IGenericMongoDoc
{
    public Guid Id { get; set; }
    public Product InventoryProduct { get; set; }
}