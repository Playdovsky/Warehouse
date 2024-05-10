using System.Windows.Controls;

namespace Main
{
    /// <summary>
    /// Warehouse control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    public partial class WarehouseControl : UserControl
    {
        public WarehouseControl()
        {
            InitializeComponent();
            Service.WarehouseDataInitialization();
            DataGridWarehouse.ItemsSource = Service.Warehouse;
        }
    }
}
