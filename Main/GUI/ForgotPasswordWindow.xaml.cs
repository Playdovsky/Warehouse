using Main.Logic;
using System;
using System.Windows;
using System.Data.Entity;


namespace Main.GUI
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void ButtonRecoverPassword_Click(object sender, RoutedEventArgs e)
        {
            string idString = TextBoxIdRecoverForm.Text;
            string email = TextBoxEmailRecoverForm.Text;

            if (string.IsNullOrWhiteSpace(idString) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter both user ID and email.", "Recover Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Guid.TryParse(idString, out Guid id))
            {
                User user = Service.GetUserById(id);
                if (user != null && Service.ValidateRecoveryEmail(id, email))
                {
                    string newPassword = PasswordGenerator.GeneratePassword();
                    user.Password = newPassword;
                    user.PasswordRecoveryStatus = true;


                    using (var context = new WarehouseDatabaseEntities())
                    {
                        context.Entry(user).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    MessageBox.Show("Password recovery initiated. Your new password will be sent to the provided email address.", "Recover Password", MessageBoxButton.OK, MessageBoxImage.Information);
                    MessageBox.Show("New password : " + newPassword + ".", "Recover Password", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
                else
                {
                    MessageBox.Show("Invalid user ID or email.", "Recover Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid user ID or email.", "Recover Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
