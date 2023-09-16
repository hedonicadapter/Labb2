using Spectre.Console;

namespace Labb2Clean
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Don't really need to run more than once but w/e
            Product.CreateProducts();

            // Don't think currentUser is /actually/ nullable but w/e
            User? currentUser = Authorization.AuthFlow();
            Console.Clear();
            Cart? currentCart = Cart.GetCart(currentUser.Username) ?? new Cart(currentUser.Username, currentUser.Password);

            Shopping.ShoppingLoop(currentCart);
        }

        
    }
}