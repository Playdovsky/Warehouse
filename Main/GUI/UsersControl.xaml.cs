using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
            using (var context = new WarehouseEntities())
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

    }
}
