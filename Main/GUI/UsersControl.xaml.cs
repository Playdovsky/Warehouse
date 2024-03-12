using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace Main
{
    /// <summary>
    /// Users control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public List<User> Users { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsersControl()
        {
            InitializeComponent();
            Users = new List<User>();

            //Retrieves data from "User" table and binds it with users list and sets the data context.
            using (var context = new WarehouseDBEntities())
            {
                var users = from u in context.User select u;
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }

            DataContext = this;
        }

        /// <summary>
        /// Resizes the DataGrid and animates the display of UserInfo_Grid.
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
                TextBoxLogin.Text = selectedUser.Login;

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


                    DataGridListOfUsers.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
                }
            }
        }

        private void DataGridListOfUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// The user can filter the list by entering first name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchingTextBox.Text.ToLower();
            var filteredUsers = Users.Where(x => x.FirstName.ToLower().Contains(filter)).ToList();
            DataGridListOfUsers.ItemsSource = filteredUsers;
        }
        /// <summary>
        /// Button that allows to modify the User Info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableFields_Click(object sender, RoutedEventArgs e)
        { 
            TextBoxFirstName.IsEnabled = true;
            TextBoxLastName.IsEnabled = true;
            TextBoxLogin.IsEnabled = true;
            TextBoxEmail.IsEnabled = true;
            TextBoxCity.IsEnabled = true;
            TextBoxStreet.IsEnabled = true;
            TextBoxPostalCode.IsEnabled = true;
            TextBoxHouseNumber.IsEnabled = true;
            TextBoxApartmentNumber.IsEnabled = true;
            TextBoxPESEL.IsEnabled = true;
            TextBoxPhoneNumber.IsEnabled = true;
            ComboBoxGender.IsEnabled = true;
            ButtonApplyChanges.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Real Time user list update
        /// </summary>
        private void LoadUsers()
        {
            Users.Clear();

            using (var context = new WarehouseDBEntities())
            {
                var users = context.User.ToList();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }

            DataGridListOfUsers.ItemsSource = null;
            DataGridListOfUsers.ItemsSource = Users;
        }

        /// <summary>
        /// Button that save changes to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (User)DataGridListOfUsers.SelectedItem;
            if (selectedUser != null)
            {
                
                selectedUser.FirstName = TextBoxFirstName.Text;
                selectedUser.LastName = TextBoxLastName.Text;
                selectedUser.Login = TextBoxLogin.Text;
                selectedUser.Email = TextBoxEmail.Text;
                selectedUser.City = TextBoxCity.Text;
                selectedUser.Street = TextBoxStreet.Text;
                selectedUser.PostalCode = TextBoxPostalCode.Text;
                selectedUser.HouseNumber = TextBoxHouseNumber.Text;
                selectedUser.ApartmentNumber = TextBoxApartmentNumber.Text;
                selectedUser.Pesel = TextBoxPESEL.Text;
                selectedUser.PhoneNumber = TextBoxPhoneNumber.Text;
                selectedUser.Gender = ComboBoxGender.Text;

                using (var context = new WarehouseDBEntities())
                {
                    
                    context.Entry(selectedUser).State = EntityState.Modified;
                    context.SaveChanges();

                    LoadUsers();
                }
            }
        }
        /// <summary>
        /// button that open clear GridUserInfo (contains button Add User)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            TextBoxFirstName.Text = "";
            TextBoxLastName.Text = "";
            TextBoxEmail.Text = "";
            TextBoxCity.Text = "";
            TextBoxStreet.Text = "";
            TextBoxApartmentNumber.Text = "";
            TextBoxPostalCode.Text = "";
            TextBoxHouseNumber.Text = "";
            TextBoxPESEL.Text = "";
            TextBoxLogin.Text = "";
            ComboBoxGender.SelectedIndex = -1; // Usuwa zaznaczenie
            TextBoxPhoneNumber.Text = "";
            EnableFields_Click(sender, e);
            ButtonEnableFields.Visibility = Visibility.Hidden;
            ButtonApplyChanges.Visibility = Visibility.Hidden;
            ButtonAddUser.Visibility = Visibility.Visible;


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


                DataGridListOfUsers.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
            }

        }
        /// <summary>
        /// adding user to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {

            User newUser = new User
            {
                FirstName = TextBoxFirstName.Text,
                LastName = TextBoxLastName.Text,
                Login = TextBoxLogin.Text,
                Email = TextBoxEmail.Text,
                City = TextBoxCity.Text,
                Street = TextBoxStreet.Text,
                PostalCode = TextBoxPostalCode.Text,
                HouseNumber = TextBoxHouseNumber.Text,
                ApartmentNumber = TextBoxApartmentNumber.Text,
                Pesel = TextBoxPESEL.Text,
                PhoneNumber = TextBoxPhoneNumber.Text,
                Gender = ComboBoxGender.Text

            };
            newUser.Id = Guid.NewGuid();

            using (var context = new WarehouseDBEntities())
            {
                context.User.Add(newUser);
                context.SaveChanges();

               
                LoadUsers();
            }

            TextBoxFirstName.Text = "";
            TextBoxLastName.Text = "";
            TextBoxEmail.Text = "";
            TextBoxCity.Text = "";
            TextBoxStreet.Text = "";
            TextBoxApartmentNumber.Text = "";
            TextBoxPostalCode.Text = "";
            TextBoxHouseNumber.Text = "";
            TextBoxPESEL.Text = "";
            TextBoxLogin.Text = "";
            ComboBoxGender.SelectedIndex = -1; 
            TextBoxPhoneNumber.Text = "";

            MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            
            LoadUsers();

        }
    }

}


