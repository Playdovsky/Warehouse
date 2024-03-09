using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        /// <summary>
        /// Main method creates log in control as the first thing in the workspace.
        /// Full log in functionality will be added during further developement.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ContentControlWorkspace.Content = new LogInControl();
        }

        /*
        public static void Connection()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "magazyn-serwer.database.windows.net";
                builder.UserID = "magazynek";
                builder.Password = "Testowanie123!";
                builder.InitialCatalog = "test";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    String sql = "SELECT TOP(3) * FROM [dbo].[User]";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        List<string> dbStrings = new List<string>();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dbStrings.Add($"{reader.GetString(0)} {reader.GetString(1)}");
                            }

                            foreach(string dbString in dbStrings)
                            {
                                MessageBox.Show(dbString);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        */

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
