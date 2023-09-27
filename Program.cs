namespace Labb2Clean
{
    internal class Program
    {
        // förvarning: behöver jobba på clean code
        static void Main(string[] args)
        {
            Authentication auth = Authentication.Instance;

            while (true)
            {
                if (auth.CurrentUser == null)
                {
                    string authOptionPicked = Authentication.ShowAuthOptions();

                    if (authOptionPicked == "Sign up") auth.SignUpFlow();
                    else if (authOptionPicked == "Sign in") auth.SignInFlow();
                    else if (authOptionPicked == "Exit") return;
                }

                Console.Clear();

                // Loops forever or returns false if user selects "Sign out"
                if (Shopping.ShoppingLoop(auth.CurrentUser.Cart) == false)
                {
                    Console.Clear();
                    auth.SignOut();
                }
            }

        }
    }
}