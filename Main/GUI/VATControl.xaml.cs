using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Main.GUI
{
    /// <summary>
    /// Logika interakcji dla klasy VATControl.xaml
    /// </summary>
    public partial class VATControl : UserControl
    {
        private WarehouseView _selectedProduct;
        public VATControl(WarehouseView selectedProduct)
        {
            InitializeComponent();
            InitializeContent(selectedProduct);
            _selectedProduct = selectedProduct;
        }

        private void InitializeContent(WarehouseView selectedProduct)
        {
            LabelVAT.Content = $"Product {selectedProduct.Name.ToString()} has VAT rate of {selectedProduct.Rate}%\nYou can change selected product VAT rate below";
            ComboBoxVAT.ItemsSource = Service.ProductVAT;
            ComboBoxVAT.DisplayMemberPath = "Rate";
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int IdVAT = ComboBoxVAT.SelectedIndex + 1;
            var productToUpdate = Service.Products.FirstOrDefault(p => p.Name == _selectedProduct.Name && p.Description == _selectedProduct.Description);
            productToUpdate.IdVAT = IdVAT;
            
            if(IdVAT != 5)
            {
                MessageBox.Show($"VAT rate for product {_selectedProduct.Name} has been updated to {ComboBoxVAT.Text}%");
            }
            else
            {
                MessageBox.Show($"VAT rate for product {_selectedProduct.Name} has been updated to exempt from VAT");
            }

            using (var context = new WarehouseDatabaseEntities())
            {
                context.Products.Attach(productToUpdate);
                context.Entry(productToUpdate).State = EntityState.Modified;
                context.SaveChanges();
            }

            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }
    }
}
