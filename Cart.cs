using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    public class Cart
    {
        public string Owner { get; private set; } // Primary key relation to user.Username
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

            return carts.FirstOrDefault(cart => string.Equals(cart.Owner, username, StringComparison.OrdinalIgnoreCase));
        }
        private static List<Cart>? GetCarts()
        {
            string persistedCartsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\carts.json");
            string JSON = File.ReadAllText(persistedCartsPath).Trim();

            if (string.IsNullOrWhiteSpace(JSON.Trim())) return null;

            List<Cart> carts = JsonSerializer.Deserialize<List<Cart>>(JSON);

            Console.WriteLine(carts);

            return carts;
        }

        public void AddProduct(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentNullException("No product name supplied.");
            productName = productName.Trim();

            foreach (Product product in Products)
            {
                if (product.Name == productName)
                {
                    product.IncrementQuantity();
                    Total += product.Price;

                    return;
                }
            }

            List<Product>? productsWeSell = Product.GetProducts();
            if (productsWeSell == null) throw new KeyNotFoundException("No products found. products.json might be corrupted.");

            Product? newProduct = productsWeSell.FirstOrDefault(product=>string.Equals(product.Name.Trim(), productName, StringComparison.OrdinalIgnoreCase));
            if (newProduct == null) throw new NullReferenceException("No product by that name in products.json.");
            
            newProduct.IncrementQuantity();
            Products.Add(newProduct);
            Total += newProduct.Price;
        }
    }
}