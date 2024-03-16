using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Data.Entity;

namespace Main
{
    /// <summary>
    /// Users control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    public partial class UsersControl : UserControl
    {
        private bool dataGridReduced = false;
        private double originalDataGridWidth;
        private double reducedDataGridWidth;
        private bool enabled = false;

        public List<User> Users { get; set; }

        public UsersControl()
        {
            InitializeComponent();
            GridUserInfo.Visibility = Visibility.Hidden;
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
            User selectedUser = (User)DataGridListOfUsers.SelectedItem;

            if (selectedUser != null)
            {

                if (selectedUser.Gender == "Male")
                    ComboBoxGender.SelectedIndex = 1;
                else if (selectedUser.Gender == "Female")
                    ComboBoxGender.SelectedIndex = 0;

                TextBoxDateOfBirth.Text = selectedUser.BirthDate?.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                TextBoxFirstName.Text = selectedUser.FirstName;
                TextBoxLastName.Text = selectedUser.LastName;
                TextBoxLogin.Text = selectedUser.Login;
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

                ButtonAddUser.Visibility = Visibility.Hidden;
                ButtonEnableFields.Visibility = Visibility.Visible;
                ButtonDeleteUser.Visibility = Visibility.Visible;

                ButtonApplyChanges.Visibility = Visibility.Hidden;

                enabled = false;
                EnableFieldsOperation();

                if (!dataGridReduced)
                {
                    originalDataGridWidth = DataGridListOfUsers.ActualWidth;
                    reducedDataGridWidth = originalDataGridWidth - (originalDataGridWidth / 3.0);
                }

                if (DataGridListOfUsers.ActualWidth <= reducedDataGridWidth)
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 550,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    GridUserInfo.BeginAnimation(WidthProperty, slideInAnimation);
                    GridUserInfo.Visibility = Visibility.Visible;
                }
                else if (!dataGridReduced && DataGridListOfUsers.ActualWidth > reducedDataGridWidth)
                {
                    DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                    {
                        From = DataGridListOfUsers.ActualWidth,
                        To = reducedDataGridWidth,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    reduceWidthAnimation.Completed += (s, args) =>
                    {
                        DoubleAnimation slideInAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = 550,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };
                        GridUserInfo.Visibility = Visibility.Visible;

                        GridUserInfo.BeginAnimation(WidthProperty, slideInAnimation);
                    };

                    DataGridListOfUsers.BeginAnimation(WidthProperty, reduceWidthAnimation);
                    dataGridReduced = true;
                }
            }
        }

        /// <summary>
        /// The user can filter the list by entering first name.
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
        /// Button that allows to modify the User Info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableFields_Click(object sender, RoutedEventArgs e)
        {
            enabled = true;
            EnableFieldsOperation();
            ButtonApplyChanges.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Real Time user list update.
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
        /// Button which saves changes to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (User)DataGridListOfUsers.SelectedItem;

            try
            {
                if (!ValidatePESEL(TextBoxPESEL.Text))
                {
                    throw new FormatException("The PESEL number is incorrect.");
                }

                if (!ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                {
                    throw new FormatException("The phone number is invalid. Enter 9 digits.");
                }

                if (DateTime.TryParseExact(TextBoxDateOfBirth.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
                {
                    selectedUser.BirthDate = birthDate;
                }
                else
                {
                    MessageBox.Show("The entered date is not in the correct format (dd.MM.yyyy). Please make sure to enter your birth date in the format day.month.year (e.g., 15.03.1990).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                selectedUser.Password = TextBoxPassword.Text;
                selectedUser.Role = ComboBoxRole.SelectedItem.ToString();

                enabled = false;
                EnableFieldsOperation();

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
        
        /// <summary>
        /// Button which opens clear GridUserInfo (contains 'Add User' button).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            EnableFields_Click(sender, e);
            
            ButtonAddUser.Visibility = Visibility.Visible;
            ButtonEnableFields.Visibility = Visibility.Hidden;
            ButtonDeleteUser.Visibility = Visibility.Hidden;

            ButtonApplyChanges.Visibility = Visibility.Hidden;

            if (!dataGridReduced)
            {
                originalDataGridWidth = DataGridListOfUsers.ActualWidth;
                reducedDataGridWidth = originalDataGridWidth - (originalDataGridWidth / 3.0);
            }

            if (DataGridListOfUsers.ActualWidth <= reducedDataGridWidth)
            {
                DoubleAnimation slideInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 550,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                GridUserInfo.BeginAnimation(WidthProperty, slideInAnimation);
                GridUserInfo.Visibility = Visibility.Visible;
            }
            else if (!dataGridReduced && DataGridListOfUsers.ActualWidth > reducedDataGridWidth)
            {
                DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                {
                    From = DataGridListOfUsers.ActualWidth,
                    To = reducedDataGridWidth,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                reduceWidthAnimation.Completed += (s, args) =>
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 550,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    GridUserInfo.Visibility = Visibility.Visible;

                    GridUserInfo.BeginAnimation(WidthProperty, slideInAnimation);
                };

                DataGridListOfUsers.BeginAnimation(WidthProperty, reduceWidthAnimation);
                dataGridReduced = true;
            }
        }

        /// <summary>
        /// Adds user to the database.
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

                if (DateTime.TryParseExact(TextBoxDateOfBirth.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
                {
                    newUser.BirthDate = birthDate;
                }
                else
                {
                    MessageBox.Show("The entered date is not in the correct format (dd.MM.yyyy). Please make sure to enter your birth date in the format day.month.year (e.g., 15.03.1990).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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

            ClearFields();

            MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Delete user from database.
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
            else
            {
                throw new ArgumentNullException("If you want to delete user you have to select him in the first place");
            }
        }

        /// <summary>
        /// Makes fields enabled / disabled for modification.
        /// </summary>
        private void EnableFieldsOperation()
        {
            TextBoxFirstName.IsEnabled = enabled;
            TextBoxLastName.IsEnabled = enabled;
            TextBoxLogin.IsEnabled = enabled;
            TextBoxEmail.IsEnabled = enabled;
            TextBoxCity.IsEnabled = enabled;
            TextBoxStreet.IsEnabled = enabled;
            TextBoxPostalCode.IsEnabled = enabled;
            TextBoxHouseNumber.IsEnabled = enabled;
            TextBoxApartmentNumber.IsEnabled = enabled;
            TextBoxPESEL.IsEnabled = enabled;
            TextBoxDateOfBirth.IsEnabled = enabled;
            TextBoxPhoneNumber.IsEnabled = enabled;
            ComboBoxGender.IsEnabled = enabled;
            TextBoxPassword.IsEnabled = enabled;
            ComboBoxRole.IsEnabled = enabled;
        }

        /// <summary>
        /// Clears textboxes and combobox fields.
        /// </summary>
        private void ClearFields()
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
            TextBoxDateOfBirth.Text = "";
            TextBoxLogin.Text = "";
            ComboBoxGender.SelectedIndex = -1;
            TextBoxPhoneNumber.Text = "";
            TextBoxPassword.Text = "";
        }

        /// <summary>
        /// Method that check whether the PESEL number is correct or not.
        /// </summary>
        /// <param name="pesel">pesel number to be checked by method</param>
        /// <returns>True if pesel format is correct or False if it is not correct</returns>
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
        /// Method that check whether the Phone Number is correct or not.
        /// </summary>
        /// <param name="phoneNumber">phone number to be checked by method</param>
        /// <returns>True if phone number format is correct or False if it is not correct</returns>
        private bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            return phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit);
        }
    }
}