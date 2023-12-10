using System.Text.Json;
using System.Text.Json.Serialization;
using Labb2Clean.DAL;
using Labb2Clean.db;

namespace Labb2Clean.Models
{
    [JsonDerivedType(typeof(GoldUser), "Gold")]
    [JsonDerivedType(typeof(SilverUser), "Silver")]
    [JsonDerivedType(typeof(BronzeUser), "Bronze")]
    public class User : IGenericMongoDoc
    {
        private string _username;
        private string _password;
        private Cart _cart;

        public Guid Id { get; set; }
        
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
        //  3. använd annan "databas"-form som .txt (måste show off allt JSON-grejsimojs :3)
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

        public User(string username, string password, Cart? cart)
        {
            Username = username;
            Password = password;
            Cart = cart ?? new Cart(username);
            Id = Guid.NewGuid();
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
        public GoldUser(string username, string password, Cart cart) : base(username, password, cart)
        {
            cart.ApplyDiscount(Discount);
        }
    }

    public class SilverUser : User
    {
        private static readonly int _discount = 10;
        public int Discount
        {
            get
            {
                return _discount;
            }
        }
        public SilverUser(string username, string password, Cart cart) : base(username, password, cart)
        {
            cart.ApplyDiscount(Discount);
        }
    }

    public class BronzeUser : User
    {
        private static readonly int _discount = 5;
        public int Discount
        {
            get
            {
                return _discount;
            }
        }
        public BronzeUser(string username, string password, Cart cart) : base(username, password, cart)
        {
            cart.ApplyDiscount(Discount);
        }
    }
}