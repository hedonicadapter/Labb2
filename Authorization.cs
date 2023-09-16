using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    // Enligt min kundundersökning sa kunderna att dessa funktioner
    // är bättre som egen klass ist för metoder på User :3
    internal static class Authorization
    {
        public static User? AuthFlow()
        {
            string authOptionPicked = AuthOptions();
            Dictionary<string, string> credentials = new();
            

            if (authOptionPicked == "Sign up") {
                string? validationResult = "User exists.";
            
                while (validationResult.StartsWith("User exists")){
                    credentials = GetUserCredentials();
                    validationResult = ValidateCredentials(credentials);

                    Console.WriteLine(validationResult);
                }
                
                User newUser = new User(credentials["username"], credentials["password"]);
                newUser.Persist();

                return newUser;
            } else if (authOptionPicked == "Sign in"){
                string? validationResult = "Invalid credentials. Try again.";


                while (!validationResult.StartsWith("User exists")){
                    credentials = GetUserCredentials();
                    validationResult = ValidateCredentials(credentials);
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

        public static string ValidateCredentials(Dictionary<string, string>credentials){
            string invalidMsg = "Invalid credentials. Try again.";

            User? user = User.GetUser(credentials["username"], credentials["password"]);
            if (user == null) {
                return User.FindUser(credentials["username"]) ? invalidMsg : "Do you want to sign up? Then restart cause I'm not about to rewrite my core loop to give you the option.";
            }
            
            return "User exists.";
        }
    }
}
