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

            try
            {

                if (!Service.ValidatePhoneNumber(TextBoxPhoneNumber.Text))
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
                selectedUser.Gender = ComboBoxGender.Text;
                if (!Service.ValidatePESEL(TextBoxPESEL.Text, birthDate, selectedUser.Gender))
                {
                    throw new FormatException("The PESEL number is incorrect.");
                }

                enabled = false;
                EnableFieldsOperation();

                ButtonApplyChanges.Visibility = Visibility.Hidden;
                ButtonEnableFields.Visibility = Visibility.Visible;

                Service.ApplyChanges(selectedUser);
                LoadUsers();
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
               
                if (!Service.ValidatePhoneNumber(TextBoxPhoneNumber.Text))
                {
                    throw new FormatException("The phone number is invalid. Enter 9 digits.");
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
                if (!Service.ValidatePESEL(TextBoxPESEL.Text, birthDate, newUser.Gender))
                {
                    throw new FormatException("The PESEL number is incorrect.");
                }
                Service.AddUser(newUser);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearFields();

            MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}