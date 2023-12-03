using Labb2Clean.DAL;
using Labb2Clean.db;
using Labb2Clean.Models;
using Spectre.Console;

namespace Labb2Clean
{
    // Enligt min kundundersökning är dessa funktioner
    // bättre som egen klass ist för metoder på User :3
    internal sealed class Authentication
    {
        private static readonly Mongo _db = new();
        
        // Singleton-syntaxen är plagiat, tar ingen cred, kan bara singletons i JS utan thread safety
        // -> https://csharpindepth.com/articles/singleton
        private static readonly Lazy<Authentication> _lazy = new(() => new Authentication());
        private static string _validationResult = "Invalid"; // default validation result
        private static Dictionary<string, string> _credentials = new();
        public static Authentication Instance { get { return _lazy.Value; } }

        private UDAL _UDAL
        {
            get;
        } = new (_db);
        
        private CDAL _CDAL
        {
            get;
        } = new (_db);
        
        public User? CurrentUser { get; private set; }

        private async Task<string> ValidateSignIn(Dictionary<string, string> credentials)
        {
            string username = credentials["username"];
            string password = credentials["password"];

            User? user = CurrentUser == null ? await _UDAL.GetUserByCredentials(username, password) : await _UDAL.GetUser(username);

            if (user != null)
            {
                if (user.Password == password) return "Valid";
                if (user.Password != password) return "Valid username";
            }
            else return "Invalid";

            return _validationResult;
        }
        private async Task<string> ValidateSignUp(Dictionary<string, string> credentials)
        {
            string username = credentials["username"];

            User? user = CurrentUser == null ? await _UDAL.GetUserByCredentials(username, null) : await _UDAL.GetUser(username);

            if (user != null)
            {
                return "Invalid";
            }
            
            return "Valid";
        }

        public async Task SignUpFlow()
        {
            while (_validationResult == "Invalid")
            {
                _credentials = GetUserCredentials();
                _validationResult = await ValidateSignUp(_credentials);

                ShowCredentialFeedback();
            }

            await SetCurrentUser(true);
        }
        public async Task SignInFlow()
        {
            while (_validationResult == "Invalid" || _validationResult == "Valid username")
            {
                _credentials = GetUserCredentials();
                _validationResult = await ValidateSignIn(_credentials);

                if (_validationResult == "Invalid")
                {
                    var signUpPrompt = new SelectionPrompt<string>().Title("Do you want to sign up instead?")
                    .AddChoices(new[] { "Yes", "No" });

                    string reply = AnsiConsole.Prompt(signUpPrompt);

                    if (reply == "Yes")
                    {
                        await SignUpFlow();
                        return;
                    }
                }

                ShowCredentialFeedback();
            }

            await SetCurrentUser();
        }
        private static void ShowCredentialFeedback()
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
        private static Dictionary<string, string> GetUserCredentials()
        {
            Dictionary<string, string> credentials = new();


            string username = AnsiConsole.Prompt(new TextPrompt<string>("Username: "));
            string password = AnsiConsole.Prompt(new TextPrompt<string>("Password: ").Secret());


            credentials.Add("username", username);
            credentials.Add("password", password);

            return credentials;
        }

        // Kan vara en custom setter men det blir mer kod
        private async Task SetCurrentUser(bool persist = false)
        {
            string username = _credentials["username"];
            string password = _credentials["password"];
            var cart = await _CDAL.GetCartByOwner(username);

            // Quick-fix syntaxen är coolare och mer koncist, men nytt för mig
            switch ((username, password))
            {
                case ("Robin", "Kamo"):
                    CurrentUser = new GoldUser(username, password, cart);

                    break;
                case ("Sam", "Ba"):
                    CurrentUser = new SilverUser(username, password, cart);

                    break;
                case ("Yandere", "Dev"):
                    CurrentUser = new BronzeUser(username, password, cart);

                    break;
                default:
                    CurrentUser = new User(username, password, cart);

                    break;
            }

            // persist = true om man signar up
            if (persist) _UDAL.Upsert(CurrentUser);
        }
    }
}
