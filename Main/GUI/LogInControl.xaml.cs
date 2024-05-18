using Main.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Main
{
    public partial class LogInControl : UserControl
    {
        private int loginAttempts = 0;
        public static string CurrentLogin { get; set; }
        public LogInControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main function which controls whole login operation (including validation).
        /// </summary>
        private async void ButtonLogIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string login = TextBoxLoginLoginForm.Text;
            string password = PasswordBoxLoginForm.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both login and password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Service.ValidateLogin(login))
            {
                MessageBox.Show("Invalid login. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TextBoxLoginLoginForm.Clear();
                return;
            }

            using (WarehouseDatabaseEntities context = new WarehouseDatabaseEntities())
            {
                var loginAttemptsDB = context.SystemAttributes.FirstOrDefault(x => x.Name == "Login attempts");
                int maxLoginAttempts = loginAttemptsDB.Attribute;

                if (!Service.ValidatePasswordLoginMatch(login, password))
                {
                    loginAttempts++;
                    if (loginAttempts >= maxLoginAttempts)
                    {
                        await HandleLoginAttemptsExceeded();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Invalid password. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        PasswordBoxLoginForm.Clear();
                        return;
                    }
                }
            }

            CurrentLogin = login;
            Guid userId = Service.GetUserId(login);

            SetProductHistoryVisibility(userId);


            if (Service.IsPasswordRecoveryRequested(userId))
            {
                NewPasswordWindow newPasswordWindow = new NewPasswordWindow(userId);
                newPasswordWindow.ShowDialog();

                if (newPasswordWindow.DialogResult.HasValue)
                {
                    List<int> userPermissions = Service.GetUserPermissions(userId);
                    if (userPermissions.Count > 0)
                    {
                        SwitchToWelcomeControl(userPermissions);
                        

                    }
                    else
                    {
                        MessageBox.Show("User has no permissions.", "Permissions", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                List<int> userPermissions = Service.GetUserPermissions(userId);
                if (userPermissions.Count > 0)
                {
                    string permissionsText = string.Join(", ", userPermissions);
                    SwitchToWelcomeControl(userPermissions);
                    
                }
                else
                {
                    MessageBox.Show("User has no permissions.", "Permissions", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Controls what is visible for users with certain permissions.
        /// </summary>
        /// <param name="userPermissions">List of all permissions from database</param>
        private void SwitchToWelcomeControl(List<int> userPermissions)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {

                mainWindow.ContentControlWorkspace.Content = new WelcomeControl();

                if (userPermissions.Contains(1))
                {
                    mainWindow.ButtonProductHistory.Visibility = Visibility.Visible;
                    mainWindow.ButtonUsers.Visibility = Visibility.Visible;
                    mainWindow.ButtonWarehouse.Visibility = Visibility.Visible;
                    mainWindow.ButtonSales.Visibility = Visibility.Visible;
                    mainWindow.ButtonPermissions.Visibility = Visibility.Visible;
                    mainWindow.ButtonAttributes.Visibility = Visibility.Visible;
                }
                else
                {
                    mainWindow.ButtonWarehouse.Visibility = userPermissions.Contains(2) ? Visibility.Visible : Visibility.Collapsed;
                    mainWindow.ButtonSales.Visibility = userPermissions.Contains(3) ? Visibility.Visible : Visibility.Collapsed;
                }

                mainWindow.ButtonLogout.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Entry point of recovery password procedure.
        /// </summary>
        private void TextBlockForgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }

        /// <summary>
        /// Blocks the ability to logging in for the time specified in database seconds and then re-enables the login fields.
        /// </summary>
        private async Task HandleLoginAttemptsExceeded()
        {
            TextBoxLoginLoginForm.IsEnabled = false;
            PasswordBoxLoginForm.IsEnabled = false;

            TextBoxLoginLoginForm.Clear();
            PasswordBoxLoginForm.Clear();

            TextBoxLoginLoginForm.Background = Brushes.LightGray;
            PasswordBoxLoginForm.Background = Brushes.LightGray;

            using (WarehouseDatabaseEntities context = new WarehouseDatabaseEntities())
            {
                var lockTimeDB = context.SystemAttributes.FirstOrDefault(x => x.Name == "Lock time");
                int maxLockTime = lockTimeDB.Attribute;
                MessageBox.Show($"You have entered an incorrect password too many times. The ability to log in has been blocked for security reasons for {maxLockTime} seconds. For assistance, please contact support.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                await Task.Delay(maxLockTime * 1000);
            }

            TextBoxLoginLoginForm.IsEnabled = true;
            PasswordBoxLoginForm.IsEnabled = true;

            TextBoxLoginLoginForm.Background = Brushes.White;
            PasswordBoxLoginForm.Background = Brushes.White;

            loginAttempts = 0;
        }

        /// <summary>
        /// When mouse button is down show inserted password to the user.
        /// </summary>
        private void ButtonShowPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TextBoxLoginLoginForm.IsEnabled && PasswordBoxLoginForm.IsEnabled)
            {
                PasswordBoxLoginForm.Visibility = Visibility.Collapsed;
                TextBoxShowPassword.Text = PasswordBoxLoginForm.Password;
                TextBoxShowPassword.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// When mouse is up hide inserted password from the user.
        /// </summary>

        private void ButtonShowPassword_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxShowPassword.Visibility = Visibility.Hidden;
            PasswordBoxLoginForm.Visibility = Visibility.Visible;
        }

        private void SetProductHistoryVisibility(Guid userId)
        {
            string userRole = Service.GetUserRole(userId);

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ButtonProductHistory.Visibility = userRole == "Manager" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

    }


}





