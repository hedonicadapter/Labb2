using Spectre.Console;

namespace Labb2Clean
{
    // Enligt min kundundersökning är dessa funktioner
    // bättre som egen klass ist för metoder på User :3
    internal sealed class Authentication
    {
        // Singleton-syntaxen är plagiat, tar ingen cred, kan bara singletons i JS utan thread safety
        // -> https://csharpindepth.com/articles/singleton
        private static readonly Lazy<Authentication> _lazy = new(() => new Authentication());
        private static string _validationResult = "Invalid"; // default validation result
        private static Dictionary<string, string> _credentials = new();
        public static Authentication Instance { get { return _lazy.Value; } }
        public User? CurrentUser { get; private set; }

        private string ValidateSignIn(Dictionary<string, string> credentials)
        {
            string username = credentials["username"];
            string password = credentials["password"];

            User? user = User.GetUser(username);

            if (user != null)
            {
                if (user.Password == password) return "Valid";
                else if (user.Password != password) return "Valid username";
            }
            else return "Invalid";

            return _validationResult;
        }
        private string ValidateSignUp(Dictionary<string, string> credentials)
        {
            string username = credentials["username"];

            User? user = User.GetUser(username);

            if (user != null)
            {
                return "Invalid";
            }
            else return "Valid";
        }

        public void SignUpFlow()
        {
            while (_validationResult == "Invalid")
            {
                _credentials = GetUserCredentials();
                _validationResult = ValidateSignUp(_credentials);

                ShowCredentialFeedback();
            }

            SetCurrentUser(true);
        }
        public void SignInFlow()
        {
            while (_validationResult == "Invalid" || _validationResult == "Valid username")
            {
                _credentials = GetUserCredentials();
                _validationResult = ValidateSignIn(_credentials);

                if (_validationResult == "Invalid")
                {
                    var signUpPrompt = new SelectionPrompt<string>().Title("Do you want to sign up instead?")
                    .AddChoices(new[] { "Yes", "No" });

                    string reply = AnsiConsole.Prompt(signUpPrompt);

                    if (reply == "Yes")
                    {
                        SignUpFlow();
                        return;
                    }
                }

                ShowCredentialFeedback();
            }


            SetCurrentUser();
        }
        public static void ShowCredentialFeedback()
        {
            switch (_validationResult)
            {
                default:
                case "Invalid":
                    Console.WriteLine("Invalid credentials. Try again.");

                    break;
                case "Valid username":
                    Console.WriteLine("Invalid password. Try again.");

                    break;
                case "Valid":
                    Console.WriteLine("Welcome.");

                    break;
            }
        }
        public void SignOut()
        {
            CurrentUser = null;
            _validationResult = "Invalid";
            _credentials = new();
        }
        public static string ShowAuthOptions()
        {
            var authOptions = new SelectionPrompt<string>()
                .Title("Membership")
                    .AddChoices(new[] { "Sign up", "Sign in", "Exit" });

            return AnsiConsole.Prompt(authOptions);
        }
        public static Dictionary<string, string> GetUserCredentials()
        {
            Dictionary<string, string> credentials = new();


            string username = AnsiConsole.Prompt(new TextPrompt<string>("Username: "));
            string password = AnsiConsole.Prompt(new TextPrompt<string>("Password: ").Secret());


            credentials.Add("username", username);
            credentials.Add("password", password);

            return credentials;
        }

        // Kan vara en custom setter men det blir mer kod
        private void SetCurrentUser(bool persist = false)
        {
            string username = _credentials["username"];
            string password = _credentials["password"];

            // Quick-fix syntaxen är coolare och mer koncist, men nytt för mig
            switch ((username, password))
            {
                case ("Robin", "Kamo"):
                    CurrentUser = new GoldUser(username, password);

                    break;
                case ("Sam", "Ba"):
                    CurrentUser = new SilverUser(username, password);

                    break;
                case ("Yandere", "Dev"):
                    CurrentUser = new BronzeUser(username, password);

                    break;
                default:
                    CurrentUser = new User(username, password);

                    break;
            }

            // persist = true om man signar up
            if (persist) CurrentUser.Persist();
        }
    }
}
