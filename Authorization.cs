using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    internal class Authorization
    {
        public static User? AuthFlow()
        {
            string authOptionPicked = AuthOptions();
            Dictionary<string, string> credentials = new();
            string? validationResult = null;

            if (authOptionPicked == "Sign up") {
                Console.WriteLine("Enter an ID.");
            
                while ((validationResult = ValidateCredentials(credentials = GetUserCredentials())) == null){
                    Console.WriteLine(validationResult);
                }

                User newUser = new User(credentials["username"], credentials["password"]);
                newUser.Persist();

                return newUser;
            } else if (authOptionPicked == "Sign in"){
                Console.WriteLine("Enter your ID.");

                while ((validationResult = ValidateCredentials(credentials = GetUserCredentials())) == null){
                    Console.WriteLine(validationResult);
                }

                return new User(credentials["username"], credentials["password"]);
            };


            return null;
        }

        public static string AuthOptions()
        {
            var authOptions = new SelectionPrompt<string>()
                .Title("Membership")
                    .AddChoices(new[] { "Sign up", "Sign in" });

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

        public static string? ValidateCredentials(Dictionary<string, string>credentials){
            User? user = User.GetUser(credentials["username"], credentials["password"]);
            
            return user != null ? credentials["username"] : "Invalid credentials. Try again.";
        }
    }
}
