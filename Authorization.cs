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
        public static Authentication Instance { get { return _lazy.Value; } }
        public User? CurrentUser { get; private set; }


        public void SignUpFlow()
        {
            Dictionary<string, string> credentials = new();
            string? validationResult = "User exists."; // Default value

            while (validationResult.StartsWith("User exists"))
            {
                credentials = GetUserCredentials();
                validationResult = ValidateCredentials(credentials);

                Console.WriteLine(validationResult);
            }

            if (credentials["username"] == "Robin" && credentials["password"] == "Kamo")
            {
                CurrentUser = new GoldUser("Robin", "Kamo");
            }
            else CurrentUser = new User(credentials["username"], credentials["password"]);

            CurrentUser.Persist();
        }
        public void SignInFlow()
        {
            Dictionary<string, string> credentials = new();
            string? validationResult = "Invalid credentials. Try again."; // Default value


            while (!validationResult.StartsWith("User exists"))
            {
                credentials = GetUserCredentials();
                validationResult = ValidateCredentials(credentials);
                Console.WriteLine(validationResult);
            }

            if (credentials["username"] == "Robin" && credentials["password"] == "Kamo")
            {
                CurrentUser = new GoldUser("Robin", "Kamo");
            }
            else CurrentUser = new User(credentials["username"], credentials["password"]);
        }
        public void SignOut()
        {
            CurrentUser = null;
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

        public static string ValidateCredentials(Dictionary<string, string> credentials)
        {
            string invalidMsg = "Invalid credentials. Try again.";

            User? user = User.GetUser(credentials["username"], credentials["password"]);
            if (user == null)
            {
                return User.FindUser(credentials["username"]) ? invalidMsg : "Do you want to sign up? Then restart cause I'm not about to rewrite my core loop to give you the option.";
            }

            return "User exists.";
        }
    }
}
