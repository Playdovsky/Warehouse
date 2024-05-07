using Main.Logic;
using System;
using System.Windows;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;


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
                    try
                    {
                        SendRecoveryEmail(email, newPassword);
                        MessageBox.Show("Password recovery initiated. Your new password has been sent to the provided email address.", "Recover Password", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to send email. Error: " + ex.Message, "Email Send Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
        private void SendRecoveryEmail(string email, string newPassword)
        {
            using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("warehouse.recover@outlook.com", "Testowanie123!");

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("warehouse.recover@outlook.com");
                    message.To.Add(new MailAddress(email));
                    message.Subject = "Your New Password";
                    message.Body = $"Hello, your new password is: {newPassword}\nYou have to change it after your first login.";

                    client.Send(message);
                }
            }
        }
    }
}
