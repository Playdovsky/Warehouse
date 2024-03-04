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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Users control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    /// Komentarz testowy
    public partial class UsersControl : UserControl
    {
        public List<User> Users { get; set; }
        public UsersControl()
        {
            InitializeComponent();
            Users = new List<User>
            {
                new User { UserId = "john_doe", FirstName = "John", LastName = "Doe",Gender = "Male", Password = "password123", Role = "user", Email = "john_doe@example.com", City = "New York", Street = "Main St", PostalCode = "10-001", HouseNumber = "12", ApartmentNumber = "12" ,Pesel = "12345678901", PhoneNumber = "555-1234" },
                new User { UserId = "alice_smith", FirstName = "Alice", LastName = "Smith",Gender = "Female", Password = "password456", Role = "admin", Email = "alice@example.com", City = "Los Angeles", Street = "Oak Ave", PostalCode = "90-001", HouseNumber = "45", ApartmentNumber = "" ,Pesel = "23456789012", PhoneNumber = "555-5678" },
                new User { UserId = "bob_johnson", FirstName = "Bob", LastName = "Johnson",Gender = "Male", Password = "password789", Role = "user", Email = "bob@example.com", City = "Chicago", Street = "Elm Blvd", PostalCode = "60-601", HouseNumber = "78", ApartmentNumber = "" ,Pesel = "34567890123", PhoneNumber = "555-91011" },
                new User { UserId = "emily_brown", FirstName = "Emily", LastName = "Brown",Gender = "Female", Password = "password321", Role = "user", Email = "emily@example.com", City = "Houston", Street = "Pine St", PostalCode = "77-001", HouseNumber = "32", ApartmentNumber = "4" ,Pesel = "45678901234", PhoneNumber = "555-2468" },
                new User { UserId = "michael_wilson", FirstName = "Michael", LastName = "Wilson",Gender = "Male", Password = "password654", Role = "admin", Email = "michael@example.com", City = "Phoenix", Street = "Maple Ave", PostalCode = "85-001", HouseNumber = "65", ApartmentNumber = "" ,Pesel = "56789012345", PhoneNumber = "555-1357" },
                new User { UserId = "sophia_martinez", FirstName = "Sophia", LastName = "Martinez",Gender = "Female", Password = "password987", Role = "user", Email = "sophia@example.com", City = "Philadelphia", Street = "Cedar Blvd", PostalCode = "19-101", HouseNumber = "98", ApartmentNumber = "" ,Pesel = "67890123456", PhoneNumber = "555-3691" },
                new User { UserId = "william_taylor", FirstName = "William", LastName = "Taylor",Gender = "Male", Password = "password246", Role = "user", Email = "william@example.com", City = "San Antonio", Street = "Birch St", PostalCode = "78-201", HouseNumber = "24", ApartmentNumber = "" ,Pesel = "78901234567", PhoneNumber = "555-8024" },
                new User { UserId = "olivia_thomas", FirstName = "Olivia", LastName = "Thomas",Gender = "Female", Password = "password135", Role = "admin", Email = "olivia@example.com", City = "San Diego", Street = "Elm Ave", PostalCode = "92-101", HouseNumber = "13", ApartmentNumber = "" ,Pesel = "89012345678", PhoneNumber = "555-5793" },
                new User { UserId = "james_hernandez", FirstName = "James", LastName = "Hernandez",Gender = "Male", Password = "password468", Role = "user", Email = "james@example.com", City = "Dallas", Street = "Oak St", PostalCode = "75-201", HouseNumber = "46", ApartmentNumber = "34" ,Pesel = "90123456789", PhoneNumber = "555-2468" },
                new User { UserId = "isabella_young", FirstName = "Isabella", LastName = "Young",Gender = "Female", Password = "password579", Role = "user", Email = "isabella@example.com", City = "San Jose", Street = "Pine Ave", PostalCode = "95-101", HouseNumber = "57", ApartmentNumber = "" ,Pesel = "01234567890", PhoneNumber = "555-1357" },
                new User { UserId = "alexander_garcia", FirstName = "Alexander", LastName = "Garcia",Gender = "Male", Password = "password789", Role = "user", Email = "alexander@example.com", City = "Austin", Street = "Cedar St", PostalCode = "78-701", HouseNumber = "78", ApartmentNumber = "" ,Pesel = "09876543210", PhoneNumber = "555-2468" },
                new User { UserId = "mia_martinez", FirstName = "Mia", LastName = "Martinez",Gender = "Female", Password = "password111", Role = "admin", Email = "mia@example.com", City = "Indianapolis", Street = "Maple Ave", PostalCode = "46-201", HouseNumber = "11", ApartmentNumber = "" ,Pesel = "98765432101", PhoneNumber = "555-1357" },
                new User { UserId = "ethan_robinson", FirstName = "Ethan", LastName = "Robinson",Gender = "Male", Password = "password222", Role = "user", Email = "ethan@example.com", City = "Jacksonville", Street = "Elm Blvd", PostalCode = "32-201", HouseNumber = "22", ApartmentNumber = "" ,Pesel = "87654321012", PhoneNumber = "555-3691" },
                new User { UserId = "ava_perez", FirstName = "Ava", LastName = "Perez",Gender = "Female", Password = "password333", Role = "user", Email = "ava@example.com", City = "San Francisco", Street = "Birch St", PostalCode = "94-101", HouseNumber = "33", ApartmentNumber = "" ,Pesel = "76543210123", PhoneNumber = "555-8024" },
                new User { UserId = "benjamin_turner", FirstName = "Benjamin", LastName = "Turner",Gender = "Male", Password = "password444", Role = "user", Email = "benjamin@example.com", City = "Columbus", Street = "Oak Ave", PostalCode = "43-201", HouseNumber = "44", ApartmentNumber = "" ,Pesel = "65432101234", PhoneNumber = "555-5793" }
            };

            DataContext = this;
            }
        /// <summary>
        /// Resizes the DataGrid and animates the display of UserInfo_Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedUser = (User)DataGridListOfUsers.SelectedItem;
            if (selectedUser != null)
            {
                TextBoxFirstName.Text = selectedUser.FirstName;
                TextBoxLastName.Text = selectedUser.LastName;
                TextBoxLogin.Text = selectedUser.UserId;
                if (selectedUser.Gender == "Male")
                    ComboBoxGender.SelectedIndex = 1;
                else if (selectedUser.Gender == "Female")
                    ComboBoxGender.SelectedIndex = 0;
                TextBoxEmail.Text = selectedUser.Email;
                TextBoxCity.Text = selectedUser.City;
                TextBoxStreet.Text = selectedUser.Street;
                TextBoxPostalCode.Text = selectedUser.PostalCode;
                TextBoxHouseNumber.Text = selectedUser.HouseNumber;
                TextBoxApartmentNumber.Text = selectedUser.ApartmentNumber;
                TextBoxPESEL.Text = selectedUser.Pesel;
                TextBoxPhoneNumber.Text = selectedUser.PhoneNumber;

                if (DataGridListOfUsers.ActualWidth <= 280)
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 350,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    GridUserInfo.BeginAnimation(Grid.WidthProperty, slideInAnimation);
                    GridUserInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                    {
                        From = DataGridListOfUsers.ActualWidth,
                        To = 280,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    reduceWidthAnimation.Completed += (s, args) =>
                    {
                        DoubleAnimation slideInAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = 350,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };

                        GridUserInfo.BeginAnimation(Grid.WidthProperty, slideInAnimation);
                        GridUserInfo.Visibility = Visibility.Visible;
                    };

                    LabelSearchParametrs.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
                    DataGridListOfUsers.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
                }
            }
        }
    }
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string Pesel { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
    }

}
