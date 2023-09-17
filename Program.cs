namespace Labb2Clean
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Authentication Auth = Authentication.Instance;

            while (true)
            {
                if (Auth.CurrentUser == null)
                {
                    string authOptionPicked = Authentication.ShowAuthOptions();

                    if (authOptionPicked == "Sign up") Auth.SignUpFlow();
                    else if (authOptionPicked == "Sign in") Auth.SignInFlow();
                }

                Cart? currentCart = Cart.GetCart(Auth.CurrentUser.Username) ?? new Cart(Auth.CurrentUser.Username);

                Console.Clear();

                // Returns false if user selects "Sign out" or "Exit"
                if (Shopping.ShoppingLoop(currentCart) == false)
                {
                    Console.Clear();
                    Auth.SignOut();
                }
            }

        }
    }
}