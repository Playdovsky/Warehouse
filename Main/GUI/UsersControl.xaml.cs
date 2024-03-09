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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Users control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    /// Komentarz testowy
    public partial class UsersControl : UserControl
    {


        public List<User> Users { get; set; }
        public UsersControl()
        {
            InitializeComponent();
            Users = new List<User>();
            using (var context = new WarehouseEntities())
            {
                var users = from u in context.User select u;
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }

            DataContext = this;
        }

            /// <summary>
            /// Resizes the DataGrid and animates the display of UserInfo_Grid
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedUser = (User)DataGridListOfUsers.SelectedItem;
            if (selectedUser != null)
            {

                TextBoxFirstName.Text = selectedUser.FirstName;
                TextBoxLastName.Text = selectedUser.LastName;
                TextBoxLogin.Text = selectedUser.Login;
                if (selectedUser.Gender == "Male")
                    ComboBoxGender.SelectedIndex = 1;
                else if (selectedUser.Gender == "Female")
                    ComboBoxGender.SelectedIndex = 0;
                TextBoxEmail.Text = selectedUser.Email;
                TextBoxCity.Text = selectedUser.City;
                TextBoxStreet.Text = selectedUser.Street;
                TextBoxPostalCode.Text = selectedUser.PostalCode;
                TextBoxHouseNumber.Text = selectedUser.HouseNumber;
                TextBoxApartmentNumber.Text = selectedUser.ApartmentNumber;
                TextBoxPESEL.Text = selectedUser.Pesel;
                TextBoxPhoneNumber.Text = selectedUser.PhoneNumber;

                if (DataGridListOfUsers.ActualWidth <= 280)
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 350,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    GridUserInfo.BeginAnimation(Grid.WidthProperty, slideInAnimation);
                    GridUserInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                    {
                        From = DataGridListOfUsers.ActualWidth,
                        To = 280,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    reduceWidthAnimation.Completed += (s, args) =>
                    {
                        DoubleAnimation slideInAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = 350,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };

                        GridUserInfo.BeginAnimation(Grid.WidthProperty, slideInAnimation);
                        GridUserInfo.Visibility = Visibility.Visible;
                    };

                    LabelSearchParametrs.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
                    DataGridListOfUsers.BeginAnimation(DataGrid.WidthProperty, reduceWidthAnimation);
                }
            }
        }
    }
}
