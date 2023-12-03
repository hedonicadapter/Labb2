using System.Text.Json;
using Labb2Clean.DAL;

namespace Labb2Clean
{
    public class Product: IGenericMongoDoc
    {
        public Guid Id { get; set; }
        public string Name { get; }
        public int Quantity { get; private set; }
        public int Price { get; }

        public Product(string name, int quantity = 0, int price = 0)
        {
            Price = price;
            Name = name;
            Quantity = quantity;
        }

        public static List<Product>? GetProducts()
        {
            string persistedProductsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\products.json");
            string JSON = File.ReadAllText(persistedProductsPath);

            if (string.IsNullOrWhiteSpace(JSON)) return null;

            var products = JsonSerializer.Deserialize<List<Product>>(JSON);

            return products;
        }
        public void IncrementQuantity()
        {
            Quantity += 1;
        }
    }
}