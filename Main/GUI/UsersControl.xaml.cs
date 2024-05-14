using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private bool enabledPermissions = true;

        public UsersControl()
        {
            InitializeComponent();
            GridUserInfo.Visibility = Visibility.Hidden;
            Service.UserDataInitialization();

            ComboBoxRole.Items.Add("Employee");
            ComboBoxRole.Items.Add("Manager");
            ComboBoxRole.SelectedIndex = 0;

            DataGridListOfUsers.ItemsSource = Service.Users;
        }

        /// <summary>
        /// Resizes the DataGrid and animates the display of UserInfo_Grid.
        /// </summary>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CheckBoxNewPassword.IsChecked = false;
            CheckBoxNewPassword.Visibility = Visibility.Hidden;
            UserView selectedUser = (UserView)DataGridListOfUsers.SelectedItem;

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

                CheckBoxSalesman.IsChecked = false;
                CheckBoxWarehouseman.IsChecked = false;
                CheckBoxAdministrator.IsChecked = false;

                if (selectedUser.PermissionIds.Contains('1'))
                {
                    CheckBoxAdministrator.IsChecked = true;
                }

                if (selectedUser.PermissionIds.Contains('2'))
                {
                    CheckBoxWarehouseman.IsChecked = true;
                }

                if (selectedUser.PermissionIds.Contains('3'))
                {
                    CheckBoxSalesman.IsChecked = true;
                }


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
        /// Button that allows to modify the User Info.
        /// </summary>
        private void EnableFields_Click(object sender, RoutedEventArgs e)
        {
            enabled = true;
            EnableFieldsOperation();
            ButtonApplyChanges.Visibility = Visibility.Visible;
            CheckBoxNewPassword.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Button which saves changes to the database.
        /// </summary>
        private void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            UserView selectedUser = (UserView)DataGridListOfUsers.SelectedItem;
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

                List<int> permissionsValue = new List<int>();

                if (CheckBoxAdministrator.IsChecked.Value || CheckBoxWarehouseman.IsChecked.Value || CheckBoxSalesman.IsChecked.Value)
                {
                    if (CheckBoxAdministrator.IsChecked.Value)
                    {
                        permissionsValue.Add(1);
                    }

                    if (CheckBoxWarehouseman.IsChecked.Value)
                    {
                        permissionsValue.Add(2);
                    }

                    if (CheckBoxSalesman.IsChecked.Value)
                    {
                        permissionsValue.Add(3);
                    }
                }
                else
                {
                    throw new FormatException("Permissions are invalid, please select at least one permission for selected user.");
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

                Service.PermissionsRemoval(selectedUser);

                foreach (int permission in permissionsValue)
                {
                    tempUser.UserPermissions.Add(new UserPermissions() { UserId = tempUser.Id, PermissionsId = permission });
                }

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
                if (CheckBoxNewPassword.IsChecked==true && string.IsNullOrEmpty(PasswordBox1.Password))
                {
                    throw new FormatException("Please enter a password.");
                }
                if (CheckBoxNewPassword.IsChecked == true && !Service.ValidatePassword(PasswordBox1.Password))
                {
                    return;
                }
                if (ComboBoxRole.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a role for the new user");
                }

                try
                {
                    if (CheckBoxNewPassword.IsChecked == true && !Service.IsNewPasswordUnique(selectedUser.Id, tempUser.Password))
                    {
                        MessageBox.Show("New password must be different from the last three passwords. Please come up with another password", "New Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (CheckBoxNewPassword.IsChecked == true)
                    {
                        Service.ApplyChanges(selectedUser, tempUser,true);
                    }
                    else
                    {
                        Service.ApplyChanges(selectedUser, tempUser);
                    }

                    Service.UpdateUserPasswordHistory(selectedUser.Id, tempUser.Password);

                    ClearFields();
                    LoadUsers();

                    enabled = false;
                    EnableFieldsOperation();

                    ButtonApplyChanges.Visibility = Visibility.Hidden;
                    ButtonEnableFields.Visibility = Visibility.Visible;
                    CheckBoxNewPassword.IsChecked = false;
                    CheckBoxNewPassword.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            EnableFields_Click(sender, e);
            
            ButtonAddUser.Visibility = Visibility.Visible;
            ButtonEnableFields.Visibility = Visibility.Hidden;
            ButtonDeleteUser.Visibility = Visibility.Hidden;
            CheckBoxNewPassword.Visibility = Visibility.Hidden;

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
                    Id = Guid.NewGuid(),
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
                    Role = ComboBoxRole.Text,
                };

                if (CheckBoxAdministrator.IsChecked.Value || CheckBoxWarehouseman.IsChecked.Value || CheckBoxSalesman.IsChecked.Value)
                {
                    UserPermissions userPermissions = new UserPermissions() { UserId = newUser.Id};

                    if (CheckBoxAdministrator.IsChecked.Value)
                    {
                        userPermissions.PermissionsId = 1;
                        newUser.UserPermissions.Add(userPermissions);
                    }

                    if (CheckBoxWarehouseman.IsChecked.Value)
                    {
                        userPermissions.PermissionsId = 2;
                        newUser.UserPermissions.Add(userPermissions);
                    }

                    if (CheckBoxSalesman.IsChecked.Value)
                    {
                        userPermissions.PermissionsId = 3;
                        newUser.UserPermissions.Add(userPermissions);
                    }
                }
                else
                {
                    throw new ArgumentNullException("Permissions are invalid, please select at least one permission for selected user.");
                }

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
                if (!Service.ValidatePassword(PasswordBox1.Password))
                {
                    return;
                }
                if (ComboBoxRole.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a role for the new user");
                }

                Service.AddUser(newUser);

                Service.UpdateUserPasswordHistory(newUser.Id, newUser.Password);

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
        private void ButtonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            UserView selectedUser = (UserView)DataGridListOfUsers.SelectedItem;

            if (selectedUser != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Service.Removal(selectedUser);
                    Service.DeleteUserPasswordHistory(selectedUser.Id);
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
        /// On text change call search method.
        /// </summary>
        private void SearchingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchByPermissions();
        }

        /// <summary>
        /// The user can filter the list by entering first name, last name or E-mail address.
        /// User can also search by permissions which has greater priority than text.
        /// </summary>
        private void Search(List<string> permissionsValue)
        {
            string filter = SearchingTextBox.Text.ToLower();

            var filteredUsersByName = Service.Users
                .Where(x =>
                    x.FirstName.ToLower().Contains(filter) ||
                    x.LastName.ToLower().Contains(filter) ||
                    x.Email.ToLower().Contains(filter)
                )
                .ToList();

            if (CheckBoxExclusiveSearch.IsChecked.Value && permissionsValue.Count == 1)
            {
                var filteredUsersByExclusivePermissions = filteredUsersByName
                    .Where(user =>
                        permissionsValue.All(permission =>
                            user.PermissionIds.Split(',').Select(y => y.Trim()).Contains(permission) &&
                            user.PermissionIds.Split(',').All(p => permissionsValue.Contains(p.Trim()))
                        )
                    )
                    .ToList();
                DataGridListOfUsers.ItemsSource = filteredUsersByExclusivePermissions;
            }
            else if (!CheckBoxSearchAll.IsChecked.Value)
            {
                var filteredUsersByPermissions = filteredUsersByName
                    .Where(x =>
                        permissionsValue.Any(tubix => x.PermissionIds.Split(',').Select(y => y.Trim()).Contains(tubix))
                    )
                    .ToList();
                DataGridListOfUsers.ItemsSource = filteredUsersByPermissions;
            }
            else
            {
                DataGridListOfUsers.ItemsSource = filteredUsersByName;
            }
        }

        /// <summary>
        /// This function locks / unlocks other checkboxes to prevent misconception.
        /// </summary>
        private void CheckBoxSearchAll_Click(object sender, RoutedEventArgs e)
        {
            if (enabledPermissions)
            {
                CheckBoxSearchAdministrator.IsEnabled = true;
                CheckBoxSearchWarehouseman.IsEnabled = true;
                CheckBoxSearchSalesman.IsEnabled = true;
                enabledPermissions = false;
            }
            else
            {
                CheckBoxSearchAdministrator.IsEnabled = false;
                CheckBoxSearchWarehouseman.IsEnabled = false;
                CheckBoxSearchSalesman.IsEnabled = false;
                
                CheckBoxSearchAdministrator.IsChecked = false;
                CheckBoxSearchWarehouseman.IsChecked = false;
                CheckBoxSearchSalesman.IsChecked = false;

                enabledPermissions = true;
            }

            SearchByPermissions();
        }

        /// <summary>
        /// Calculates permissionsValue and serves as broker function between permissions and search.
        /// </summary>
        private void SearchByPermissions()
        {
            List<string> permissionsValue = new List<string>();

            if (CheckBoxSearchAdministrator.IsChecked.Value)
                permissionsValue.Add("1");

            if (CheckBoxSearchWarehouseman.IsChecked.Value)
                permissionsValue.Add("2");

            if (CheckBoxSearchSalesman.IsChecked.Value)
                permissionsValue.Add("3");

            Search(permissionsValue);
        }

        /// <summary>
        /// Simple event for further search purposes.
        /// </summary>
        private void CheckBoxSearchAdministrator_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxExclusiveSearch.IsChecked == true)
            {
                CheckBoxSearchWarehouseman.IsChecked = false;
                CheckBoxSearchSalesman.IsChecked = false;
            }
            SearchByPermissions();
        }

        /// <summary>
        /// Simple event for further search purposes.
        /// </summary>
        private void CheckBoxSearchWarehouseman_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxExclusiveSearch.IsChecked == true)
            {
                CheckBoxSearchAdministrator.IsChecked = false;
                CheckBoxSearchSalesman.IsChecked = false;
            }
            SearchByPermissions();
        }

        /// <summary>
        /// Simple event for further search purposes.
        /// </summary>
        private void CheckBoxSearchSalesman_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxExclusiveSearch.IsChecked == true)
            {
                CheckBoxSearchAdministrator.IsChecked = false;
                CheckBoxSearchWarehouseman.IsChecked = false;
            }
            SearchByPermissions();
        }

        /// <summary>
        /// Checking if SearchExclusive is checked or not
        /// </summary>
        private void CheckBoxSearch_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxExclusiveSearch.IsChecked == true)
            {
                SearchExclusive();
            }
            else
            {
                SearchByPermissions();
            }
        }

        /// <summary>
        /// Searching for users who only have the role we choose
        /// </summary>
        private void SearchExclusive()
        {
            List<string> permissionsValue = new List<string>();

            if (CheckBoxSearchAdministrator.IsChecked.Value)
                permissionsValue.Add("1");

            if (CheckBoxSearchWarehouseman.IsChecked.Value)
                permissionsValue.Add("2");

            if (CheckBoxSearchSalesman.IsChecked.Value)
                permissionsValue.Add("3");

            if (permissionsValue.Count == 1)
            {
                Search(permissionsValue);
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
            CheckBoxAdministrator.IsEnabled = enabled;
            CheckBoxWarehouseman.IsEnabled = enabled;
            CheckBoxSalesman.IsEnabled = enabled;
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