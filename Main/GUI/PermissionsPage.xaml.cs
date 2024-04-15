using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Main
{
    using System.Windows.Controls;
    using System.Linq;

    public partial class PermissionsPage : UserControl
    {
        public PermissionsPage()
        {
            InitializeComponent();
            LoadPermissions();
        }

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
