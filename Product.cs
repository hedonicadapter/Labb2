using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    public class Product
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public int Price { get; private set; }

        public Product(string name, int quantity = 0, int price = 0)
        {
            Price = price;
            Name = name;
            Quantity = quantity;
        }

        public static void CreateProducts()
        {
            List<Product> products = new List<Product>();
            products.Add(new Product("Borger", 0, 5));
            products.Add(new Product("Soda", 0, 99));
            products.Add(new Product("Uranium", 0, 1));

            string json = JsonSerializer.Serialize(products);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\products.json");

            File.WriteAllText(path, json);
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