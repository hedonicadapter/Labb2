using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    public class Cart
    {
        public string Owner { get; private set; }
        public List<Product> Products { get; private set; }
        public double Total { get; private set; }


        public Cart(string username, string password, List<Product>? products = null) {
            Cart? existingCart = GetCart(username);
            Owner = username;

            if (existingCart != null)
            {
                Owner = existingCart.Owner;
                Products = existingCart.Products;
                Total = existingCart.Total;

                return;
            }

            if (products != null)
            {
                Products = products;

                foreach (Product product in products)
                {
                    Total += product.Price;
                }

            } else Products = new List<Product>();
        }
        public static Cart? GetCart(string username){
            List<Cart>? carts = GetCarts();
            if (carts == null) return null;

            return carts.FirstOrDefault(cart => cart.Owner == username);
        }
        private static List<Cart>? GetCarts()
        {
            string persistedCartsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\persistedCarts.json");
            string JSON = File.ReadAllText(persistedCartsPath).Trim();

            if (string.IsNullOrEmpty(JSON.Trim())) return null;

            List<Cart> carts = JsonSerializer.Deserialize<List<Cart>>(JSON);

            Console.WriteLine(carts);

            return carts;
        }

        public void AddProduct(string productName)
        {
            if (productName == null) throw new ArgumentNullException("No product name supplied.");

            foreach (Product product in Products)
            {
                if (product.Name == productName)
                {
                    product.IncrementQuantity();
                    Total += product.Price;

                    return;
                }
            }

            Product newItem = new Product(productName, 1);
            Products.Add(newItem);
            Total += newItem.Price;
        }
    }
}