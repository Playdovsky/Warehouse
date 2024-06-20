﻿using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Main.GUI
{
    public partial class VATControl : UserControl
    {
        private WarehouseView _selectedProduct;

        public VATControl(WarehouseView selectedProduct)
        {
            InitializeComponent();
            InitializeContent(selectedProduct);
            _selectedProduct = selectedProduct;

            Service.ApplyScheduledVatChanges();

            CheckBoxImmediateChange.Checked += CheckBoxImmediateChange_CheckedChanged;
            CheckBoxImmediateChange.Unchecked += CheckBoxImmediateChange_CheckedChanged;
        }

        private void InitializeContent(WarehouseView selectedProduct)
        {
            LabelVAT.Content = $"Product {selectedProduct.Name} has VAT rate of {selectedProduct.Rate}%\nYou can change selected product VAT rate below";
            LabelVAT.Content = $"Product {selectedProduct.Name} of type {selectedProduct.TypeName} has VAT rate of {selectedProduct.Rate}%\nYou can change VAT rate for every product of {selectedProduct.TypeName} type below";
            var productVATs = new ObservableCollection<ProductVAT>(Service.ProductVAT);
            ComboBoxVAT.ItemsSource = productVATs;
        }

        private void CheckBoxImmediateChange_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (CheckBoxImmediateChange != null)
            {
                DatePickerEffectiveDate.IsEnabled = !(CheckBoxImmediateChange.IsChecked ?? false);
            }
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            int IdVAT = ComboBoxVAT.SelectedIndex + 1;
            bool applyImmediately = CheckBoxImmediateChange.IsChecked ?? false;
            DateTime effectiveDate = applyImmediately ? DateTime.Now.Date : (DatePickerEffectiveDate.SelectedDate ?? DateTime.Now.Date);

            ProductVAT selectedProductVAT = ComboBoxVAT.SelectedItem as ProductVAT;
            Products productToUpdate = Service.Products.FirstOrDefault(p => p.Name == _selectedProduct.Name && p.Description == _selectedProduct.Description);
            if (ComboBoxVAT.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a VAT rate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string vatRate = selectedProductVAT != null && selectedProductVAT.Rate != null ? selectedProductVAT.Rate.ToString() : "exempt";

            string message;
            if (IdVAT != 5)
            {
                message = $"VAT rate for product {_selectedProduct.Name} has been updated to {vatRate}%";
            }
            else
            {
                message = $"VAT rate for product {_selectedProduct.Name} has been updated to exempt from VAT";
            }

            using (var context = new WarehouseDatabaseEntities())
            {
                if (!applyImmediately)
                {
                    if (effectiveDate <= DateTime.Now.Date)
                    {
                        MessageBox.Show("Effective date for VAT change cannot be in the past.");
                        return;
                    }

                    var vatChange = new ProductVATChange
                    {
                        ProductId = productToUpdate.Id,
                        VatId = IdVAT,
                        EffectiveDate = effectiveDate
                    };
                    context.ProductVATChange.AddOrUpdate(vatChange);
                    message += $" effective from {effectiveDate.ToShortDateString()}";
                }
                else
                {
                    productToUpdate.IdVAT = IdVAT;
                    context.Products.Attach(productToUpdate);
                    context.Entry(productToUpdate).State = EntityState.Modified;
                }

                context.SaveChanges();
            }

            MessageBox.Show(message);
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        
    }
}