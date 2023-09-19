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

                Console.Clear();

                // Returns false if user selects "Sign out" or "Exit"
                if (Shopping.ShoppingLoop(Auth.CurrentUser.Cart) == false)
                {
                    Console.Clear();
                    Auth.SignOut();
                }
            }

        }
    }
}