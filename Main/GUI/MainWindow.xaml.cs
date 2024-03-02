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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Main window of all operations
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
