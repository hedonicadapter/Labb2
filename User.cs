using System.Text.Json;
using System.Text.Json.Serialization;

namespace Labb2Clean
{
    [JsonDerivedType(typeof(GoldUser), "Gold")]
    [JsonDerivedType(typeof(SilverUser), "Silver")]
    [JsonDerivedType(typeof(BronzeUser), "Bronze")]
    public class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; } // Yes it's public. Mad?
        public Cart Cart { get; set; }

        public User(string username, string password, int discount = 0)
        {
            Username = username;
            Password = password;

            Cart = Cart.GetCart(username) ?? new Cart(username, null, discount);
        }

        override public string ToString()
        {
            Cart cart = Cart.GetCart(this.Username) ?? new Cart(this.Username);
            string snyggString = $"{this.Username} {this.Password} {cart}";

            Console.WriteLine(snyggString);
            return snyggString;
        }

        // Exposed credentials aja baja moment
        public static User? GetUser(string username, string password)
        {
            List<User>? users = GetUsers();
            if (users == null) return null;

            return users.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        public static bool FindUser(string username)
        {
            List<User>? users = GetUsers();
            if (users == null) return false;

            return users.FirstOrDefault(user => user.Username == username) != null ? true : false;
        }

        static List<User>? GetUsers()
        {
            string persistedUsers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");
            string JSON = File.ReadAllText(persistedUsers).Trim();

            if (string.IsNullOrWhiteSpace(JSON.Trim())) return null;

            List<User> users = JsonSerializer.Deserialize<List<User>>(JSON);
            foreach (User user in users)
            {
                Console.WriteLine(user.Username);
            }
            return users;
        }

        public void Persist()
        {
            List<User>? users = GetUsers();

            if (users == null)
            {
                users = new List<User>();
                users.Add(this);
            }
            else
            {
                var thisUserExists = users.FindIndex(user => user.Username == this.Username);

                // Upsert
                if (thisUserExists == -1)
                {
                    users.Add(this);
                }
                else
                {
                    users[thisUserExists] = this;
                }
            }


            string json = JsonSerializer.Serialize(users);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");

            File.WriteAllText(path, json);
        }
    }

    public class GoldUser : User
    {
        static int Discount = 15;
        public GoldUser(string username, string password) : base(username, password, Discount)
        {
        }
    }

    public class SilverUser : User
    {
        static int Discount = 10;
        public SilverUser(string username, string password) : base(username, password, Discount)
        {
        }
    }

    public class BronzeUser : User
    {
        static int Discount = 5;
        public BronzeUser(string username, string password) : base(username, password, Discount)
        {
        }
    }
}