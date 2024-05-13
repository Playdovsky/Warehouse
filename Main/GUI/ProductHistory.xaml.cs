using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System.Windows.Data;
using System.ComponentModel;

namespace Main.GUI
{
    public partial class ProductHistory : UserControl
    {
        private ICollectionView _filteredProductHistoryView;
        private WarehouseDatabaseEntities _context;
        private List<ProductsHistoryExtended> _allProductHistory;
        private bool isFilterApplied = false;

        public ProductHistory()
        {
            InitializeComponent();
            _context = new WarehouseDatabaseEntities();
            LoadData();
            LoadProductTypes();
            LoadEmployees();
            SetupFilters();

            // Dodaj obsługę zdarzenia Loaded dla UserControl
            Loaded += (sender, e) => ApplyFilters();
        }

        private void LoadData()
        {
            _allProductHistory = _context.ProductsHistoryExtended.ToList();
            _filteredProductHistoryView = CollectionViewSource.GetDefaultView(_allProductHistory);
            HistoryDataGrid.ItemsSource = _filteredProductHistoryView;
        }

        private void SetupFilters()
        {
            // Ustawienie filtrów dla każdej kontrolki
            ProductTypeComboBox.SelectionChanged += (s, e) => ApplyFilters();
            EmployeeComboBox.SelectionChanged += (s, e) => ApplyFilters();
            FilterTextBox.TextChanged += (s, e) => ApplyFilters();
            NameCheckBox.Checked += (s, e) => ApplyFilters();
            NameCheckBox.Unchecked += (s, e) => ApplyFilters();
            ProductTypeCheckBox.Checked += (s, e) => ApplyFilters();
            ProductTypeCheckBox.Unchecked += (s, e) => ApplyFilters();
            UserNameCheckBox.Checked += (s, e) => ApplyFilters();
            UserNameCheckBox.Unchecked += (s, e) => ApplyFilters();
        }

        private void LoadProductTypes()
        {
            var productTypes = _context.ProductType.ToList();
            ProductTypeComboBox.ItemsSource = productTypes;
            ProductTypeComboBox.DisplayMemberPath = "TypeName";
            ProductTypeComboBox.SelectedValuePath = "Id";
        }

        private void LoadEmployees()
        {
            var employees = _context.User.ToList();
            EmployeeComboBox.ItemsSource = employees.Select(user => new {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName
            }).ToList();
            EmployeeComboBox.DisplayMemberPath = "FullName";
            EmployeeComboBox.SelectedValuePath = "Id";
        }

        private void ApplyFilters()
        {
            _filteredProductHistoryView.Filter = obj =>
            {
                var history = obj as ProductsHistoryExtended;
                if (history == null) return true;

                bool filterByName = NameCheckBox.IsChecked == true && !string.IsNullOrEmpty(FilterTextBox.Text);
                bool filterByProductType = ProductTypeCheckBox.IsChecked == true && ProductTypeComboBox.SelectedValue != null;
                bool filterByUserName = UserNameCheckBox.IsChecked == true && EmployeeComboBox.SelectedValue != null;

                bool nameMatches = filterByName ? (history.ProductName != null && history.ProductName.Contains(FilterTextBox.Text)) : true;
                bool productTypeMatches = filterByProductType ? (history.ProductType != null && history.ProductType == (ProductTypeComboBox.SelectedItem as ProductType)?.TypeName) : true;
                bool userNameMatches = filterByUserName ? (history.UserName != null && history.UserName == (EmployeeComboBox.SelectedItem as dynamic)?.FullName) : true;

                return nameMatches && productTypeMatches && userNameMatches;
            };

            // Odśwież widok po zastosowaniu filtrów
            _filteredProductHistoryView.Refresh();
        }
       
    }
}
