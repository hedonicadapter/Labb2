using System.Text.Json;

namespace Labb2Clean
{
    public class Cart
    {
        private double _total = 0;
        private int _discount = 0;

        public string Owner { get; private set; } // Primary key relation to user.Username
        public List<Product> Products { get; private set; }
        public int Discount
        {
            get
            {
                return _discount;
            }
            protected set
            {
                _discount = value;
            }
        }
        public double Total
        {
            get
            {
                return _total;
            }
            private set
            {
                if (value == 0) { _total = 0; return; }
                if (Discount > 0)
                {
                    double newTotal = _total + value;
                    double discountPercentage = Discount * 0.01;
                    double discountedAmount = newTotal * discountPercentage;

                    _total = Math.Abs(newTotal - discountedAmount);
                }
                else
                {
                    _total += value;
                }
            }
        }

        public Cart(string owner, List<Product>? products = null, int discount = 0)
        {
            Owner = owner;
            Discount = discount;
            Total = 0;

            if (products != null && products.Count > 0)
            {
                Products = products;

                foreach (Product product in products)
                {
                    if (product.Quantity > 0)
                    {
                        Total = product.Price * product.Quantity;

                    }
                    else Total = product.Price;
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

        public void Clear()
        {
            Products = new List<Product>();
            Total = 0;
            Persist();
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
                    Total = product.Price;

                    Persist();
                    return;
                }
            }

            List<Product>? productsWeSell = Product.GetProducts();
            if (productsWeSell == null) throw new KeyNotFoundException("No products found. ./bin/Debug/net7.0/db/products.json might be corrupted.");

            Product? newProduct = productsWeSell.FirstOrDefault(product => string.Equals(product.Name.Trim(), productName, StringComparison.OrdinalIgnoreCase));
            if (newProduct == null) throw new NullReferenceException("No product by that name in products.json.");

            newProduct.IncrementQuantity();
            Products.Add(newProduct);
            Total = newProduct.Price;

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