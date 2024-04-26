using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Main.GUI
{
    public partial class AttributesControl : UserControl
    {
        private List<SystemAttributes> attributes;
        
        /// <summary>
        /// Initializes basic system attributes control components.
        /// </summary>
        public AttributesControl()
        {
            InitializeComponent();
            LoadAttributes();
        }

        /// <summary>
        /// Loads attributes into the datagrid.
        /// </summary>
        private void LoadAttributes()
        {
            using (WarehouseDatabaseEntities context = new WarehouseDatabaseEntities())
            {
                attributes = context.SystemAttributes.ToList();
                AttributesDataGrid.ItemsSource = attributes;

                var lockTimeAttribute = attributes.FirstOrDefault(x => x.Name == "Lock time");
                TextboxLock.Text = lockTimeAttribute.Attribute.ToString();

                var attemptsAttribute = attributes.FirstOrDefault(x => x.Name == "Login attempts");
                TextboxAttempts.Text = attemptsAttribute.Attribute.ToString();
            }
        }

        /// <summary>
        /// Saves system attribute changes to database.
        /// </summary>
        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextboxLock.Text) || string.IsNullOrEmpty(TextboxAttempts.Text))
                {
                    throw new FormatException("Textbox is empty");
                }

                int lockTimeValue, attemptsValue;
                if (!int.TryParse(TextboxLock.Text, out lockTimeValue) || !int.TryParse(TextboxAttempts.Text, out attemptsValue))
                {
                    throw new FormatException("Incorrect format. Please insert numbers");
                }

                if (lockTimeValue <= 0 || attemptsValue <= 0)
                {
                    throw new FormatException("Values cannot be equal 0 or less");
                }

                Service.SaveSqlAttributeChanges(lockTimeValue, attemptsValue);
                
                LoadAttributes();
                MessageBox.Show("System attributes successfully changed.");
                ButtonEnableFields.Visibility = Visibility.Visible;
                ButtonSaveChanges.Visibility = Visibility.Hidden;
                TextboxLock.IsEnabled = false;
                TextboxAttempts.IsEnabled = false;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// On button click enables fields.
        /// </summary>
        private void ButtonEnableFields_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextboxLock.IsEnabled = true;
            TextboxAttempts.IsEnabled = true;
            ButtonEnableFields.Visibility = Visibility.Hidden;
            ButtonSaveChanges.Visibility = Visibility.Visible;
        }
    }
}
