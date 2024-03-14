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

            ComboBoxRole.Items.Add("user");
            ComboBoxRole.Items.Add("admin");
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
                TextBoxPassword.Text = selectedUser.Password;
                ComboBoxRole.SelectedItem = selectedUser.Role;
                TextBoxPassword.Visibility = Visibility.Visible;
                ComboBoxRole.Visibility = Visibility.Visible;
                ButtonAddUser.Visibility = Visibility.Hidden;
                ButtonEnableFields.Visibility = Visibility.Visible;
                ButtonDeleteUser.Visibility = Visibility.Visible;
                ButtonApplyChanges.Visibility = Visibility.Hidden;

                TextBoxFirstName.IsEnabled = false;
                TextBoxLastName.IsEnabled = false;
                TextBoxLogin.IsEnabled = false;
                TextBoxEmail.IsEnabled = false;
                TextBoxCity.IsEnabled = false;
                TextBoxStreet.IsEnabled = false;
                TextBoxPostalCode.IsEnabled = false;
                TextBoxHouseNumber.IsEnabled = false;
                TextBoxApartmentNumber.IsEnabled = false;
                TextBoxPESEL.IsEnabled = false;
                TextBoxPhoneNumber.IsEnabled = false;
                ComboBoxGender.IsEnabled = false;
                TextBoxPassword.IsEnabled = false;
                ComboBoxRole.IsEnabled = false;

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
            TextBoxPassword.IsEnabled = true;
            ComboBoxRole.IsEnabled = true;
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
                try
                {
                    if (!ValidatePESEL(TextBoxPESEL.Text))
                    {
                        throw new Exception("The PESEL number is incorrect.");
                    }

                    if (!ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                    {
                        throw new Exception("The phone number is invalid. Enter 9 digits.");
                    }

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
                    selectedUser.Password = TextBoxPassword.Text;
                    selectedUser.Role = ComboBoxRole.SelectionBoxItem.ToString();

                    TextBoxFirstName.IsEnabled = false;
                    TextBoxLastName.IsEnabled = false;
                    TextBoxLogin.IsEnabled = false;
                    TextBoxEmail.IsEnabled = false;
                    TextBoxCity.IsEnabled = false;
                    TextBoxStreet.IsEnabled = false;
                    TextBoxPostalCode.IsEnabled = false;
                    TextBoxHouseNumber.IsEnabled = false;
                    TextBoxApartmentNumber.IsEnabled = false;
                    TextBoxPESEL.IsEnabled = false;
                    TextBoxPhoneNumber.IsEnabled = false;
                    ComboBoxGender.IsEnabled = false;
                    TextBoxPassword.IsEnabled = false;
                    ComboBoxRole.IsEnabled = false;

                    ButtonApplyChanges.Visibility = Visibility.Hidden;
                    ButtonEnableFields.Visibility = Visibility.Visible;

                    using (var context = new WarehouseDBEntities())
                    {
                        context.Entry(selectedUser).State = EntityState.Modified;
                        context.SaveChanges();

                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            TextBoxPassword.Text = "";
            EnableFields_Click(sender, e);
            ButtonEnableFields.Visibility = Visibility.Hidden;
            ButtonApplyChanges.Visibility = Visibility.Hidden;
            ButtonAddUser.Visibility = Visibility.Visible;
            ButtonDeleteUser.Visibility = Visibility.Hidden;
            TextBoxPassword.Visibility = Visibility.Visible;
            ComboBoxRole.Visibility = Visibility.Visible;


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
            try
            {
                if (!ValidatePESEL(TextBoxPESEL.Text))
                {
                    throw new Exception("The PESEL number is incorrect.");
                }

                if (!ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                {
                    throw new Exception("The phone number is invalid. Enter 9 digits.");
                }

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
                    Gender = ComboBoxGender.Text,
                    Password = TextBoxPassword.Text,
                    Role = ComboBoxRole.SelectionBoxItem.ToString()

                };
                newUser.Id = Guid.NewGuid();

                using (var context = new WarehouseDBEntities())
                {
                    context.User.Add(newUser);
                    context.SaveChanges();


                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        /// <summary>
        /// Delete user from DateBase. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (User)DataGridListOfUsers.SelectedItem;
            if (selectedUser != null)
            {
                using (var context = new WarehouseDBEntities())
                {
                    if (context.Entry(selectedUser).State == EntityState.Detached)
                    {
                        context.User.Attach(selectedUser);
                    }

                    context.User.Remove(selectedUser);
                    context.SaveChanges();

                    LoadUsers();
                }
            }
        }
        /// <summary>
        /// method that check whether the PESEL number is correct or not
        /// </summary>
        /// <param name="pesel"></param>
        /// <returns></returns>
        private bool ValidatePESEL(string pesel)
        {
            if (pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }

            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(pesel[i].ToString()) * weights[i];
            }

            int controlNumber = (10 - (sum % 10)) % 10;

            return controlNumber == int.Parse(pesel[10].ToString());
        }
        /// <summary>
        /// method that check whether the Phone Number is correct or not
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            return phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit);
        }

    }

}


