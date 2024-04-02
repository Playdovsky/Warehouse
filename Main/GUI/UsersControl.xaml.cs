using System;
using System.Linq;
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
        private bool dataGridReduced = false;
        private double originalDataGridWidth;
        private double reducedDataGridWidth;
        private bool enabled = false;

        public UsersControl()
        {
            InitializeComponent();
            GridUserInfo.Visibility = Visibility.Hidden;
            Service.UserListInitialization();

            ComboBoxRole.Items.Add("user");
            ComboBoxRole.Items.Add("admin");

            DataGridListOfUsers.ItemsSource = Service.Users;
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
                PasswordBox1.Password = selectedUser.Password;
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
        /// The user can filter the list by entering first name, last name or E-mail address.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchingTextBox.Text.ToLower();
            var filteredUsers = Service.Users.Where(x =>
                x.FirstName.ToLower().Contains(filter) ||
                x.LastName.ToLower().Contains(filter) ||
                x.Email.ToLower().Contains(filter)
            ).ToList();
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
        /// Button which saves changes to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (User)DataGridListOfUsers.SelectedItem;
            User tempUser = new User();

            try
            {
                if (string.IsNullOrEmpty(TextBoxFirstName.Text) || string.IsNullOrEmpty(TextBoxLastName.Text))
                {
                    throw new FormatException("Please enter both first and last name.");
                }
                if (string.IsNullOrEmpty(TextBoxLogin.Text))
                {
                    throw new FormatException("Please enter a login.");
                }
                if (ComboBoxGender.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a gender for the new user.");
                }
                if (TextBoxEmail.Text != selectedUser.Email)
                {
                    if (!Service.ValidateEmail(TextBoxEmail.Text))
                    {
                        throw new FormatException("The email address is invalid. Example of a valid email address: example@example.com");
                    }
                }
                if (TextBoxPhoneNumber.Text != selectedUser.PhoneNumber)
                {
                    if (!Service.ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                    {
                        throw new FormatException("The phone number is invalid. Enter 9 digits.");
                    }
                }

                string phoneNumber = Service.ConvertPhoneNumber(TextBoxPhoneNumber.Text);

                tempUser.FirstName = TextBoxFirstName.Text;
                tempUser.LastName = TextBoxLastName.Text;
                tempUser.Login = TextBoxLogin.Text;
                tempUser.Email = TextBoxEmail.Text;
                tempUser.City = TextBoxCity.Text;
                tempUser.Street = TextBoxStreet.Text;
                tempUser.PostalCode = TextBoxPostalCode.Text;
                tempUser.HouseNumber = TextBoxHouseNumber.Text;
                tempUser.ApartmentNumber = TextBoxApartmentNumber.Text;
                tempUser.Pesel = TextBoxPESEL.Text;
                tempUser.PhoneNumber = phoneNumber;
                tempUser.Password = PasswordBox1.Password;
                tempUser.Role = ComboBoxRole.SelectedItem.ToString();
                tempUser.Gender = ComboBoxGender.Text;

                if (DateTime.TryParseExact(TextBoxDateOfBirth.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
                {
                    tempUser.BirthDate = birthDate;
                }
                else
                {
                    MessageBox.Show("The entered date is not in the correct format (dd.MM.yyyy). Please make sure to enter your birth date in the format day.month.year (e.g., 15.03.1990).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (TextBoxPESEL.Text != selectedUser.Pesel)
                {
                    if (!Service.ValidatePESEL(TextBoxPESEL.Text, birthDate, ComboBoxGender.Text))
                    {
                        throw new FormatException("The PESEL number is incorrect.");
                    }
                }
                if (string.IsNullOrEmpty(TextBoxCity.Text) || string.IsNullOrEmpty(TextBoxPostalCode.Text) || string.IsNullOrEmpty(TextBoxHouseNumber.Text))
                {
                    throw new FormatException("Please provide complete address details.");
                }
                if (string.IsNullOrEmpty(PasswordBox1.Password))
                {
                    throw new FormatException("Please enter a password.");
                }
                if (ComboBoxRole.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a role for the new user");
                }

                enabled = false;
                EnableFieldsOperation();

                ButtonApplyChanges.Visibility = Visibility.Hidden;
                ButtonEnableFields.Visibility = Visibility.Visible;

                Service.ApplyChanges(selectedUser, tempUser);
                ClearFields();
                LoadUsers();

                DoubleAnimation hideUserInfoAnimation = new DoubleAnimation
                {
                    From = 550,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                hideUserInfoAnimation.Completed += (s, args) =>
                {
                    GridUserInfo.Visibility = Visibility.Hidden;

                    if (dataGridReduced)
                    {
                        DoubleAnimation expandWidthAnimation = new DoubleAnimation
                        {
                            From = DataGridListOfUsers.ActualWidth,
                            To = originalDataGridWidth,
                            Duration = TimeSpan.FromSeconds(0.3)
                        };

                        DataGridListOfUsers.BeginAnimation(WidthProperty, expandWidthAnimation);
                        dataGridReduced = false;
                    }
                };

                GridUserInfo.BeginAnimation(WidthProperty, hideUserInfoAnimation);

                MessageBox.Show("Changes applied successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (string.IsNullOrEmpty(TextBoxFirstName.Text) || string.IsNullOrEmpty(TextBoxLastName.Text))
                {
                    throw new FormatException("Please enter both first and last name.");
                }
                if (string.IsNullOrEmpty(TextBoxLogin.Text))
                {
                    throw new FormatException("Please enter a login.");
                }
                if (ComboBoxGender.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a gender for the new user.");
                }
                if (!Service.ValidateEmail(TextBoxEmail.Text))
                {
                    throw new FormatException("The email address is invalid. Example of a valid email address: example@example.com");
                }
                if (!Service.ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                {
                    throw new FormatException("The phone number is invalid. Enter 9 digits.");
                }

                string phoneNumber = Service.ConvertPhoneNumber(TextBoxPhoneNumber.Text);

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
                    PhoneNumber = phoneNumber,
                    Gender = ComboBoxGender.Text,
                    Password = PasswordBox1.Password,
                    Role = ComboBoxRole.Text
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

                if (!Service.ValidatePESEL(TextBoxPESEL.Text, birthDate, newUser.Gender))
                {
                    throw new FormatException("The PESEL number is incorrect.");
                }
                if (string.IsNullOrEmpty(TextBoxCity.Text) || string.IsNullOrEmpty(TextBoxPostalCode.Text) || string.IsNullOrEmpty(TextBoxHouseNumber.Text))
                {
                    throw new FormatException("Please provide complete address details.");
                }
                if (string.IsNullOrEmpty(PasswordBox1.Password))
                {
                    throw new FormatException("Please enter a password.");
                }
                if (ComboBoxRole.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a role for the new user");
                }

                Service.AddUser(newUser);
                ClearFields();
                LoadUsers();

                MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Removes user from database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (User)DataGridListOfUsers.SelectedItem;

            if (selectedUser != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Service.Removal(selectedUser);
                    LoadUsers();
                    ClearFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

      

        /// <summary>
        /// Real Time user list update also known as 'refresh'.
        /// </summary>
        private void LoadUsers()
        {
            Service.LoadUsers();
            DataGridListOfUsers.ItemsSource = null;
            DataGridListOfUsers.ItemsSource = Service.Users;
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
            PasswordBox1.IsEnabled = enabled;
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
            PasswordBox1.Password = ""; 
        }
    }
}