using Main.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;


namespace Main.GUI
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        private Guid userId;
        public NewPasswordWindow( Guid userId)
        {
            InitializeComponent();
            this.userId=userId;
        }

        private void ButtonConfirmNewPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = TextBoxNewPassword.Text;
            string confirmedPassword = TextBoxConfirmNewPassword.Text;

            if (!Service.ValidatePassword(newPassword))
            {
                return;
            }

            if (newPassword != confirmedPassword)
            {
                MessageBox.Show("Passwords do not match.", "New Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = Service.GetUserById(userId);
            if (user != null)
            {
                user.Password = newPassword;
                user.PasswordRecoveryStatus = false;

                using (var context = new WarehouseDatabaseEntities())
                {
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                }

                MessageBox.Show("Password change was successful.", "New Password", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("User does not exist.", "New Password Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
