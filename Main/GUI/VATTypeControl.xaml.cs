using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Main.GUI
{
    /// <summary>
    /// Logika interakcji dla klasy VATTypeControl.xaml
    /// </summary>
    public partial class VATTypeControl : UserControl
    {
        WarehouseView _selectedProduct;
        public VATTypeControl(WarehouseView selectedProduct)
        {
            InitializeComponent();
            InitializeContent(selectedProduct);
            _selectedProduct = selectedProduct;
        }

        private void InitializeContent(WarehouseView selectedProduct)
        {
            LabelVAT.Content = $"Product {selectedProduct.Name} of type {selectedProduct.TypeName} has VAT rate of {selectedProduct.Rate}%\nYou can change VAT rate for every product of {selectedProduct.TypeName} type below";
            ComboBoxVAT.ItemsSource = Service.ProductVAT;
            ComboBoxVAT.DisplayMemberPath = "Rate";
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int IdVAT = ComboBoxVAT.SelectedIndex + 1;
            int IdType = Service.ProductType.FirstOrDefault(pt => pt.TypeName == _selectedProduct.TypeName)?.Id ?? -1;

            if (IdType != -1)
            {
                var productsToUpdate = Service.Products.Where(p => p.IdType == IdType).ToList();

                foreach (var productToUpdate in productsToUpdate)
                {
                    productToUpdate.IdVAT = IdVAT;
                }

                using (var context = new WarehouseDatabaseEntities())
                {
                    foreach (var productToUpdate in productsToUpdate)
                    {
                        context.Products.Attach(productToUpdate);
                        context.Entry(productToUpdate).State = EntityState.Modified;
                    }

                    context.SaveChanges();
                }

                if (IdVAT != 5)
                {
                    MessageBox.Show($"VAT rate for type {_selectedProduct.TypeName} has been updated to {ComboBoxVAT.Text}%");
                }
                else
                {
                    MessageBox.Show($"VAT rate for type {_selectedProduct.TypeName} has been updated to exempt from VAT");
                }

                Window parentWindow = Window.GetWindow(this);
                parentWindow.Close();
            }
            else
            {
                MessageBox.Show($"Type {_selectedProduct.TypeName} not found in the ProductType table.");
            }
        }
    }
}