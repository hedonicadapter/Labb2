using Labb2Clean.db;

namespace Labb2Clean
{
    internal class Program
    {
        
        private static Mongo _db = new();

        static async Task Main(string[] args)
        {
            
            Authentication auth = Authentication.Instance;

            while (true)
            {
                if (auth.CurrentUser == null)
                {
                    string authOptionPicked = Authentication.ShowAuthOptions();

                    if (authOptionPicked == "Sign up") await auth.SignUpFlow();
                    else if (authOptionPicked == "Sign in") await auth.SignInFlow();
                    else if (authOptionPicked == "Exit") return;
                }

                Console.Clear();

                // Loops forever or returns false if user selects "Sign out"
                if (await Shopping.ShoppingLoop(auth.CurrentUser.Cart) == false)
                {
                    Console.Clear();
                    auth.SignOut();
                }
            }

        }
    }
}