using System.Text.Json;

namespace Labb2Clean
{
    public class Cart
    {
        public string Owner { get; private set; } // Primary key relation to user.Username
        public List<Product> Products { get; private set; }
        public int Total { get; private set; }

        public Cart(string owner, List<Product>? products = null, int total = 0)
        {
            Owner = owner;
            Total = 0;

            if (products != null)
            {
                Products = products;

                foreach (Product product in products)
                {
                    Total += product.Price;
                }

            }
            else Products = new List<Product>();
        }
        public static Cart? GetCart(string owner)
        {
            List<Cart>? carts = GetCarts();
            if (carts == null) return null;

            return carts.FirstOrDefault(cart => string.Equals(cart.Owner, owner, StringComparison.OrdinalIgnoreCase));
        }
        private static List<Cart>? GetCarts()
        {
            string persistedCartsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\carts.json");
            string JSON = File.ReadAllText(persistedCartsPath).Trim();

            if (string.IsNullOrWhiteSpace(JSON.Trim())) return null;

            List<Cart> carts = JsonSerializer.Deserialize<List<Cart>>(JSON);

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

                    Persist();
                    return;
                }
            }

            List<Product>? productsWeSell = Product.GetProducts();
            if (productsWeSell == null) throw new KeyNotFoundException("No products found. products.json might be corrupted.");

            Product? newProduct = productsWeSell.FirstOrDefault(product => string.Equals(product.Name.Trim(), productName, StringComparison.OrdinalIgnoreCase));
            if (newProduct == null) throw new NullReferenceException("No product by that name in products.json.");

            newProduct.IncrementQuantity();
            Products.Add(newProduct);
            Total += newProduct.Price;

            Persist();
        }

        public void Persist()
        {
            List<Cart>? carts = GetCarts();

            if (carts == null)
            {
                carts = new List<Cart>();
                carts.Add(this);
            }
            else
            {
                var thisCartExists = carts.FindIndex(cart => cart.Owner == this.Owner);

                // Upsert
                if (thisCartExists == -1)
                {
                    carts.Add(this);
                }
                else
                {
                    carts[thisCartExists] = this;
                }
            }

            string json = JsonSerializer.Serialize(carts);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\carts.json");

            File.WriteAllText(path, json);
        }
    }
}