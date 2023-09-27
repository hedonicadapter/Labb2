using System.Text.Json;
using System.Text.Json.Serialization;

namespace Labb2Clean
{
    [JsonDerivedType(typeof(GoldUser), "Gold")]
    [JsonDerivedType(typeof(SilverUser), "Silver")]
    [JsonDerivedType(typeof(BronzeUser), "Bronze")]
    public class User
    {
        private string _username;
        private string _password;
        private Cart _cart;

        public string Username
        {
            get
            {
                return _username;
            }
            private set
            {
                _username = value;
            }
        }
        // Förstår att denna ska vara private, och att det räcker med en field,
        // men JSON serializear inte private members. Det finns tre lösningar som 
        // jag tror hamnar utanför the scope av uppgiften:
        //  1. declare custom converter för serializern (mycket kod som är nytt för mig)
        //  2. använd Newtonsoft.Json (har inte kollat hur man gör)
        //  3. använd annan "databas"-form som .txt (orkar inte skriva om allt JSON-grejsimojs :3)
        public string Password
        {
            get
            {
                return _password;
            }
            private set
            {
                _password = value;
            }
        }
        public Cart Cart
        {
            get
            {
                return _cart;
            }
            set
            {
                _cart = value;
            }
        }

        public User(string username, string password)
        {
            Username = username;
            Password = password;

            Cart = Cart.GetCart(username) ?? new Cart(username, null);
        }

        override public string ToString()
        {
            Cart cart = Cart.GetCart(this.Username) ?? new Cart(this.Username);
            string snyggString = $"{this.Username} {this.Password} {cart}";

            Console.WriteLine(snyggString);
            return snyggString;
        }

        public static User? GetUser(string username)
        {
            List<User>? users = GetUsers();
            if (users == null) return null;

            return users.FirstOrDefault(user => user.Username == username);
        }
        public static User? GetUser(string username, string password)
        {
            List<User>? users = GetUsers();
            if (users == null) return null;

            return users.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        static List<User>? GetUsers()
        {
            string persistedUsers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\db\\users.json");
            string JSON = File.ReadAllText(persistedUsers);

            if (string.IsNullOrWhiteSpace(JSON)) return null;

            List<User> users = JsonSerializer.Deserialize<List<User>>(JSON);

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
        private static readonly int _discount = 15;
        public int Discount
        {
            get
            {
                return _discount;
            }
        }
        public GoldUser(string username, string password) : base(username, password)
        {
            Cart = Cart.GetCart(username) ?? new Cart(username, null, Discount);
        }
    }

    public class SilverUser : User
    {
        private static int _discount = 10;
        public int Discount
        {
            get
            {
                return _discount;
            }
        }
        public SilverUser(string username, string password) : base(username, password)
        {
            Cart = Cart.GetCart(username) ?? new Cart(username, null, Discount);
        }
    }

    public class BronzeUser : User
    {
        private static int _discount = 5;
        public int Discount
        {
            get
            {
                return _discount;
            }
        }
        public BronzeUser(string username, string password) : base(username, password)
        {
            Cart = Cart.GetCart(username) ?? new Cart(username, null, Discount);
        }
    }
}