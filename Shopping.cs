using System.Text.RegularExpressions;
using Spectre.Console;

namespace Labb2Clean
{
    public static class Shopping
    {
        public static bool ShoppingLoop(Cart cart)
        {
            string shoppingOptionPicked;

            while ((shoppingOptionPicked = ShowShoppingPortal(cart)) != "Exit")
            {
                switch (shoppingOptionPicked)
                {
                    case "Browse items":
                        string productPicked = "";
                        while ((productPicked = ShowBrowseItems()) != "Go back")
                        {
                            string productName = Regex.Replace(productPicked, @"[\d\s-]+kr$", string.Empty);
                            cart.AddProduct(productName);
                        }

                        break;
                    case "Go to cart":
                        string cartOptionPicked = ShowCart(cart);
                        if (cartOptionPicked == "Checkout")
                        {
                            cart.Clear();
                            Console.WriteLine("Enjoy!");
                        }
                        break;
                    case "Sign out":
                        return false;
                    case "Checkout":
                        cart.Clear();
                        Console.WriteLine("Enjoy!");
                        break;
                    default: return false;
                }

            }
            return false;
        }
        public static string ShowShoppingPortal(Cart cart)
        {
            string[] choices =
                cart.Products.Count > 0 ?
                    new[] { "Browse items", "Go to cart", "Checkout", "", "Sign out", "Exit" }
                    : new[] { "Browse items", "Go to cart", "", "Sign out", "Exit" };

            var shoppingOptions = new SelectionPrompt<string>()
                .Title("Customer portal")
                    .AddChoices(choices);

            return AnsiConsole.Prompt(shoppingOptions);
        }

        public static string ShowBrowseItems()
        {
            List<Product>? products = Product.GetProducts();
            // Should check for null but im not about to do error handling and allat

            var productAisle = new SelectionPrompt<string>();

            foreach (Product product in products)
            {
                productAisle.AddChoice($"{product.Name} {product.Price}kr");
            }

            productAisle.AddChoice("Go back");


            return AnsiConsole.Prompt(productAisle);
        }

        public static string ShowCart(Cart cart)
        {
            var cartOptions = new SelectionPrompt<string>()
                .Title("Cart")
                    .AddChoices(new[] { "Go back" });

            if (cart.Products.Count == 0) return AnsiConsole.Prompt(cartOptions.Title("Cart is empty."));

            foreach (Product product in cart.Products)
            {
                Console.WriteLine($"{product.Name} {product.Quantity}pc {product.Price}kr/pc cost: {product.Price * product.Quantity}");
            }
            Console.WriteLine($"Total: {cart.Total}kr");

            if (cart.Products.Count > 0) cartOptions.AddChoice("Checkout");
            return AnsiConsole.Prompt(cartOptions);
        }
    }
}