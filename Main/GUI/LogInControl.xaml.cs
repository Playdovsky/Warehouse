using Main.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Main
{
    /// <summary>
    /// Log in control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// It should be noted, that log in functionality shall be developed in the future.
    /// </summary>
    public partial class LogInControl : UserControl
    {
        private int loginAttempts = 0;
        public LogInControl()
        {
            InitializeComponent();
        }

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

            if (!Service.ValidatePassword(login, password))
            {
                loginAttempts++;
                if (loginAttempts >= 3)
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

            Guid userId = Service.GetUserId(login);

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
        private void SwitchToWelcomeControl(List<int> userPermissions)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ContentControlWorkspace.Content = new WelcomeControl();

                if (userPermissions.Contains(1))
                {
                    mainWindow.ButtonUsers.Visibility = Visibility.Visible;
                    mainWindow.ButtonWarehouse.Visibility = Visibility.Visible;
                    mainWindow.ButtonSales.Visibility = Visibility.Visible;
                }
                else
                {
                    mainWindow.ButtonWarehouse.Visibility = userPermissions.Contains(2) ? Visibility.Visible : Visibility.Collapsed;
                    mainWindow.ButtonSales.Visibility = userPermissions.Contains(3) ? Visibility.Visible : Visibility.Collapsed;
                }

                mainWindow.ButtonLogout.Visibility = Visibility.Visible;
            }
        }
        private void TextBlockForgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }

        /// <summary>
        /// Blocks the ability to log in for 10 seconds and then re-enables the login fields.
        /// </summary>
        private async Task HandleLoginAttemptsExceeded()
        {
            MessageBox.Show("You have entered an incorrect password three times. The ability to log in has been blocked for security reasons for 10 seconds. For assistance, please contact support.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);

            TextBoxLoginLoginForm.IsEnabled = false;
            PasswordBoxLoginForm.IsEnabled = false;

            TextBoxLoginLoginForm.Clear();
            PasswordBoxLoginForm.Clear();

            TextBoxLoginLoginForm.Background = Brushes.LightGray;
            PasswordBoxLoginForm.Background = Brushes.LightGray;

            await Task.Delay(10000);

            TextBoxLoginLoginForm.IsEnabled = true;
            PasswordBoxLoginForm.IsEnabled = true;

            TextBoxLoginLoginForm.Background = Brushes.White;
            PasswordBoxLoginForm.Background = Brushes.White;

            loginAttempts = 0;
        }
        private void ButtonShowPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TextBoxLoginLoginForm.IsEnabled && PasswordBoxLoginForm.IsEnabled)
            {
                PasswordBoxLoginForm.Visibility = Visibility.Collapsed;
                TextBoxShowPassword.Text = PasswordBoxLoginForm.Password;
                TextBoxShowPassword.Visibility = Visibility.Visible;
            }
        }

        private void ButtonShowPassword_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxShowPassword.Visibility = Visibility.Hidden;
            PasswordBoxLoginForm.Visibility = Visibility.Visible;
        }

    }
}
