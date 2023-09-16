using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2Clean
{
    public class User
    {
        public string Username {get; private set;}
        public string Password {get; private set;} // Yes it's public. Mad?

        public User(string username, string password){
            Username = username;
            Password = password;
        }

        override public string ToString(){
            Cart cart = Cart.GetCart(this.Username) ?? new Cart(this.Username, this.Password);
            string snyggString = $"{this.Username} {this.Password} {cart}";

            Console.WriteLine(snyggString);
            return snyggString;
        }

        // Exposed credentials aja baja moment
        public static User? GetUser(string username, string password){
            List<User>? users = GetUsers();
            if (users == null) return null;

            return users.FirstOrDefault(user=>user.Username == username && user.Password == password);
        }

        public static bool FindUser(string username){
            List<User>? users = GetUsers();
            if (users == null) return false;

            return users.FirstOrDefault(user=>user.Username == username) != null ? true : false;
        }

        static List<User>? GetUsers()
        {
            string persistedUsers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");
            string JSON = File.ReadAllText(persistedUsers).Trim();

            if (string.IsNullOrWhiteSpace(JSON.Trim())) return null;

            List<User> users = JsonSerializer.Deserialize<List<User>>(JSON);

            return users;
        }

        public void Persist(){
            List<User>? users = GetUsers();

            if (users == null) {
                users = new List<User>();
                users.Add(this);
            } else
            {
                var thisUserExists = users.FindIndex(user => user.Username == this.Username);

                // Upsert
                if (thisUserExists == -1)
                {
                    Console.WriteLine("this user doesnt exist " + thisUserExists);
                    users.Add(this);
                } else
                {
                    Console.WriteLine("this user exists " + thisUserExists);
                    users[thisUserExists] = this;
                }
            }


            string json = JsonSerializer.Serialize(users);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");

            File.WriteAllText(path, json);
        }
    }
}