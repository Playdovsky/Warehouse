using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;

namespace Main
{
    /// <summary>
    /// Main window of all operations.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Main method creates log in control as the first thing in the workspace.
        /// Full log in functionality will be added during further developement.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ContentControlWorkspace.Content = new LogInControl();
        }

        /// <summary>
        /// On button click creates and opens users control workspace.
        /// </summary>
        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            ContentControlWorkspace.Content = new UsersControl();
        }

        /// <summary>
        /// On button click creates and opens warehouse control workspace.
        /// </summary>
        private void ButtonWarehouse_Click(object sender, RoutedEventArgs e)
        {
            ContentControlWorkspace.Content = new WarehouseControl();
        }

        /// <summary>
        /// On button click creates and opens sales control workspace.
        /// </summary>
        private void ButtonSales_Click(object sender, RoutedEventArgs e)
        {
            ContentControlWorkspace.Content = new SalesControl();
        }

        /// <summary>
        /// Initiates exit procedure.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Service.Exit();
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Log out confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ButtonUsers.Visibility = Visibility.Collapsed;
                ButtonWarehouse.Visibility = Visibility.Collapsed;
                ButtonSales.Visibility = Visibility.Collapsed;
                ButtonLogout.Visibility = Visibility.Collapsed;

                ContentControlWorkspace.Content = new LogInControl();
            }
        }

        private void ButtonPermissions_Click(object sender, RoutedEventArgs e)
        {
            ContentControlWorkspace.Content = new PermissionsPage();
        }
    }
}
