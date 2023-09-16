using Spectre.Console;

namespace Labb2Clean
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Don't think currentUser is /actually/ nullable but w/e
            User? currentUser = Authorization.AuthFlow();
            Cart? currentCart = Cart.GetCart(currentUser.Username);

            
        }

        
    }
}