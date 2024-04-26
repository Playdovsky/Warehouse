using System.Windows.Controls;
using System.Linq;

namespace Main
{
    public partial class PermissionsPage : UserControl
    {
        /// <summary>
        /// Initializes basic permissions control components.
        /// </summary>
        public PermissionsPage()
        {
            InitializeComponent();
            LoadPermissions();
        }

        /// <summary>
        /// Loads permissions into the datagrid.
        /// </summary>
        private void LoadPermissions()
        {
            using (var context = new WarehouseDatabaseEntities())
            {
                var permissions = context.Permissions.ToList();
                PermissionsDataGrid.ItemsSource = permissions;
            }
        }

    }
}
