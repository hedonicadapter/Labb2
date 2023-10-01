using System.Text.RegularExpressions;
using Spectre.Console;

namespace Labb2Clean
{
    public static class Shopping
    {
        private static string[] AllowedCurrencies = new string[] { "SEK", "CNY", "USD" };
        public static string Currency { get; private set; } = "SEK";
        public static bool ShoppingLoop(Cart cart)
        {
            while (true)
            {
                string shoppingOptionPicked = ShowShoppingPortal(cart);

                switch (shoppingOptionPicked)
                {
                    case "Browse items":
                        string productPicked = "";
                        while ((productPicked = ShowBrowseItems()) != "Go back")
                        {
                            string productName = Regex.Replace(productPicked, $@"[\d\s-]+{Currency}$", string.Empty);
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

                    case "Checkout":
                        cart.Clear();
                        Console.WriteLine("Enjoy!");

                        break;
                    case "Switch currency":
                        CurrencyPicker();
                        break;
                    case "Sign out":
                        return false;
                    case "Exit":
                        Environment.Exit(0);

                        break; // never called but ig it's consistent? c# moment
                }

            }
        }
        public static string ShowShoppingPortal(Cart cart)
        {
            string[] choices =
                cart.Products.Count > 0 ?
                    new[] { "Browse items", "Go to cart", "Checkout", "Switch currency", "", "Sign out", "Exit" }
                    : new[] { "Browse items", "Go to cart", "Switch currency", "", "Sign out", "Exit" };

            var shoppingOptions = new SelectionPrompt<string>()
                .Title("Customer portal")
                    .AddChoices(choices);

            return AnsiConsole.Prompt(shoppingOptions);
        }

        public static string ShowBrowseItems()
        {
            List<Product>? products = Product.GetProducts();
            // Should check for null but im not about to do error handling and allat and it wont be null unless someone (Robin) tampers with the "backend"

            var productAisle = new SelectionPrompt<string>();

            foreach (Product product in products)
            {
                double convertedPrice = SEKToCurrency(product.Price);
                productAisle.AddChoice($"{product.Name} {convertedPrice}{Currency}");
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
                double convertedPrice = SEKToCurrency(product.Price);
                Console.WriteLine($"{product.Name} {product.Quantity}pc {convertedPrice}{Currency}/pc cost: {convertedPrice * product.Quantity}{Currency}");
            }

            double convertedTotal = SEKToCurrency(cart.Total);
            Console.WriteLine($"Total: {convertedTotal}{Currency}");

            if (cart.Products.Count > 0) cartOptions.AddChoice("Checkout");
            return AnsiConsole.Prompt(cartOptions);
        }

        public static void CurrencyPicker()
        {
            var shoppingOptions = new SelectionPrompt<string>()
                .Title("Pick a currency")
                    .AddChoices(new[] { "SEK", "CNY", "USD", "Go back" });

            string selection = AnsiConsole.Prompt(shoppingOptions);
            if (AllowedCurrencies.Contains(selection)) Currency = selection;
        }

        public static double SEKToCurrency(double price)
        {
            switch (Currency)
            {
                case "USD":
                    return price * 0.092;
                case "CNY":
                    return price * 0.67;
                default:
                case "SEK":
                    return price;
            }
        }
    }
}