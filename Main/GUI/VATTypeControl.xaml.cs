using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System.Data.Entity.Migrations;


namespace Main.GUI
{
    public partial class VATTypeControl : UserControl
    {
        WarehouseView _selectedProduct;

        public VATTypeControl(WarehouseView selectedProduct)
        {
            InitializeComponent();
            InitializeContent(selectedProduct);
            _selectedProduct = selectedProduct;

            CheckBoxImmediateChange.Checked += CheckBoxImmediateChange_CheckedChanged;
            CheckBoxImmediateChange.Unchecked += CheckBoxImmediateChange_CheckedChanged;
        }
        private void CheckBoxImmediateChange_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (CheckBoxImmediateChange != null)
            {
                DatePickerEffectiveDate.IsEnabled = !(CheckBoxImmediateChange.IsChecked ?? false);
            }
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
            bool applyImmediately = CheckBoxImmediateChange.IsChecked ?? false;
            DateTime effectiveDate = applyImmediately ? DateTime.Now.Date : (DatePickerEffectiveDate.SelectedDate ?? DateTime.Now.Date);

            string message;
            if (ComboBoxVAT.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a VAT rate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            using (var context = new WarehouseDatabaseEntities())
            {
                var productsToUpdate = context.Products.Include(p => p.ProductType)
                                            .Where(p => p.ProductType.TypeName == _selectedProduct.TypeName).ToList();

                if (IdVAT != 5)
                {
                    message = $"VAT rate for all products of type {_selectedProduct.TypeName} has been updated to {ComboBoxVAT.Text}%";
                }
                else
                {
                    message = $"VAT rate for all products of type {_selectedProduct.TypeName} has been updated to exempt from VAT";
                }

                if (!applyImmediately)
                {
                    if (effectiveDate <= DateTime.Now.Date)
                    {
                        MessageBox.Show("Effective date for VAT change cannot be in the past.");
                        return;
                    }

                    var typeVatChange = new ProductTypeVATChange
                    {
                        ProductTypeId = productsToUpdate.First().IdType,
                        VatId = IdVAT,
                        EffectiveDate = effectiveDate
                    };
                    context.ProductTypeVATChange.AddOrUpdate(typeVatChange);

                    message += $" effective from {effectiveDate.ToShortDateString()}";
                }
                else
                {
                    foreach (var product in productsToUpdate)
                    {
                        product.IdVAT = IdVAT;
                        context.Products.Attach(product);
                        context.Entry(product).State = EntityState.Modified;
                    }
                }

                context.SaveChanges();
            }

            MessageBox.Show(message);
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

    }
}
