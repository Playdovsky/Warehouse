using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Users control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public List<User> Users { get; set; }
        public UsersControl()
        {
            InitializeComponent();
            Users = new List<User>
            {
                new User { Id = 1, Name = "John", Surname = "Doe", Login = "john_doe", Password = "password123", Role = "user", Email = "john_doe@example.com", Address = "123 Main St", Phone = "555-1234" },
                new User { Id = 2, Name = "Alice", Surname = "Smith", Login = "alice_smith", Password = "password456", Role = "admin", Email = "alice@example.com", Address = "456 Oak Ave", Phone = "555-5678" },
                new User { Id = 3, Name = "Bob", Surname = "Johnson", Login = "bob_johnson", Password = "password789", Role = "user", Email = "bob@example.com", Address = "789 Elm Blvd", Phone = "555-91011" },
                new User { Id = 4, Name = "Emily", Surname = "Brown", Login = "emily_brown", Password = "password321", Role = "user", Email = "emily@example.com", Address = "321 Pine St", Phone = "555-2468" },
                new User { Id = 5, Name = "Michael", Surname = "Wilson", Login = "michael_wilson", Password = "password654", Role = "admin", Email = "michael@example.com", Address = "654 Maple Ave", Phone = "555-1357" },
                new User { Id = 6, Name = "Sophia", Surname = "Martinez", Login = "sophia_martinez", Password = "password987", Role = "user", Email = "sophia@example.com", Address = "987 Cedar Blvd", Phone = "555-3691" },
                new User { Id = 7, Name = "William", Surname = "Taylor", Login = "william_taylor", Password = "password246", Role = "user", Email = "william@example.com", Address = "246 Birch St", Phone = "555-8024" },
                new User { Id = 8, Name = "Olivia", Surname = "Thomas", Login = "olivia_thomas", Password = "password135", Role = "admin", Email = "olivia@example.com", Address = "135 Elm Ave", Phone = "555-5793" },
                new User { Id = 9, Name = "James", Surname = "Hernandez", Login = "james_hernandez", Password = "password468", Role = "user", Email = "james@example.com", Address = "468 Oak St", Phone = "555-2468" },
                new User { Id = 10, Name = "Isabella", Surname = "Young", Login = "isabella_young", Password = "password579", Role = "user", Email = "isabella@example.com", Address = "579 Pine Ave", Phone = "555-1357" },
                new User { Id = 11, Name = "Alexander", Surname = "Garcia", Login = "alexander_garcia", Password = "password789", Role = "user", Email = "alexander@example.com", Address = "789 Cedar St", Phone = "555-2468" },
                new User { Id = 12, Name = "Mia", Surname = "Martinez", Login = "mia_martinez", Password = "password111", Role = "admin", Email = "mia@example.com", Address = "111 Maple Ave", Phone = "555-1357" },
                new User { Id = 13, Name = "Ethan", Surname = "Robinson", Login = "ethan_robinson", Password = "password222", Role = "user", Email = "ethan@example.com", Address = "222 Elm Blvd", Phone = "555-3691" },
                new User { Id = 14, Name = "Ava", Surname = "Perez", Login = "ava_perez", Password = "password333", Role = "user", Email = "ava@example.com", Address = "333 Birch St", Phone = "555-8024" },
                new User { Id = 15, Name = "Benjamin", Surname = "Turner", Login = "benjamin_turner", Password = "password444", Role = "user", Email = "benjamin@example.com", Address = "444 Oak Ave", Phone = "555-5793" }
            };
                DataContext = this;
            }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
