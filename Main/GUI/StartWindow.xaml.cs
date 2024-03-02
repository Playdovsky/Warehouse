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
using System.Windows.Threading;

namespace Main
{
    /// <summary>
    /// Startup window which is executed for first 2 seconds.
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            Startup();
        }

        /// <summary>
        /// Startup window which lasts 2 seconds.
        /// In the background initialization of main window.
        /// </summary>
        private async void Startup()
        {
            var mainWindow = new MainWindow();
            mainWindow.Hide();
            await Task.Delay(2000);
            mainWindow.Show();
            this.Close();
        }
    }
}
