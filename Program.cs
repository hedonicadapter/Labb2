namespace Labb2Clean
{
    internal class Program
    {
        // förvarning: behöver jobba på clean code
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
                    else if (authOptionPicked == "Exit") return;
                }

                Console.Clear();

                // Loops forever or returns false if user selects "Sign out"
                if (Shopping.ShoppingLoop(Auth.CurrentUser.Cart) == false)
                {
                    Console.Clear();
                    Auth.SignOut();
                }
            }

        }
    }
}