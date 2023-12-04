using System.Text.Json;
using System.Text.RegularExpressions;
using Labb2Clean.DAL;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb2Clean.Models
{
    public class Cart : IGenericMongoDoc
    {
        private double _total = 0;
        private int _discount = 0;

        [BsonId]
        public Guid Id { get; set; }
        public string Owner { get; private set; } // Primary key relation to user.Username
        public List<Product> Products { get; set; }
        public int Discount
        {
            get
            {
                return _discount;
            }
            private set
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

        public void Clear()
        {
            Products = new List<Product>();
            Total = 0;
        }

        public async Task AddProduct(string productName, TDAL<Product> _PDAL)
        {
            if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentNullException("No product name supplied.");
            productName = Regex.Replace(productName.Trim(), "[^a-zA-Z]", "");

            foreach (Product product in Products)
            {
                if (product.Name == productName)
                {
                    product.IncrementQuantity();
                    Total = product.Price;

                    return;
                }
            }

            List<Product>? productsWeSell = await _PDAL.GetAll();
            if (productsWeSell == null) throw new KeyNotFoundException("No products found. ./bin/Debug/net7.0/db/products.json might be corrupted.");

            Console.WriteLine(productName);
            Product? newProduct = productsWeSell.FirstOrDefault(product => string.Equals(product.Name.Trim(), productName, StringComparison.OrdinalIgnoreCase));
            if (newProduct == null) throw new NullReferenceException("No product by that name in products.json.");

            newProduct.IncrementQuantity();
            Products.Add(newProduct);
            Total = newProduct.Price;
        }

        public void ApplyDiscount(int discount)
        {
            Discount = discount;
        }
    }
}