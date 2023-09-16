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

        // Exposed credentials aja baja moment
        public static User? GetUser(string username, string password){
            List<User>? users = GetUsers();
            if (users == null) return null;

            return users.FirstOrDefault(user=>user.Username == username && user.Password == password);
        }

        static List<User>? GetUsers()
        {
            string persistedUsers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");
            string JSON = File.ReadAllText(persistedUsers).Trim();

            if (string.IsNullOrEmpty(JSON.Trim())) return null;

            List<User> users = JsonSerializer.Deserialize<List<User>>(JSON);

            return users;
        }

        public void Persist(){
            List<User>? users = GetUsers();
            
            if (users == null) {
                users = new List<User>{ this };
            } else
            {
                var thisUserExists = users.FindIndex(user => user.Username == this.Username);

                // Upsert
                if (thisUserExists == -1)
                {
                    users.Add(this);
                } else
                {
                    users[thisUserExists] = this;
                }
            }


            string json = JsonSerializer.Serialize(users);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");

            File.WriteAllText(path, json);
        }
    }
}