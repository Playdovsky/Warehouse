using System;
using System.Windows;

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
        /// Initiates exit procedure.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Service.Exit();
        }
    }
}
