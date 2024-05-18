using Main.GUI;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Main
{
    /// <summary>
    /// Warehouse control workspace.
    /// This is workspace which is available to use inside MainWindow.
    /// </summary>
    public partial class WarehouseControl : UserControl
    {
        private bool dataGridReduced = false;
        private double originalDataGridWidth;
        private double reducedDataGridWidth;
        public WarehouseControl()
        {
            InitializeComponent();
            HideAllGrids();

            Service.WarehouseDataInitialization();
            Service.ProductDataInitialization();
            Service.ProductHistoryInitialization();
            LoadProducts();

            ComboBoxProductType.ItemsSource = Service.ProductType;
            ComboBoxProductType.DisplayMemberPath = "TypeName";

            ComboBoxProductVat.ItemsSource = Service.ProductVAT;
            ComboBoxProductVat.DisplayMemberPath = "Rate";

            DataGridWarehouse.ItemsSource = Service.Warehouse;
        }


        private void HideAllGrids()
        {
            GridProductInfo.Visibility = Visibility.Hidden;
            GridNewDelivery.Visibility = Visibility.Hidden;
        }
        private void DataGridWarehouse_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridWarehouse.SelectedItem != null)
            {
                WarehouseView selectedRow = (WarehouseView)DataGridWarehouse.SelectedItem;

                TextBoxProductHistoryName.Text = selectedRow.Name;
                TextBoxProductHistoryType.Text = selectedRow.TypeName;
                TextBoxProductHistoryMeasure.Text = selectedRow.Measure;
                TextBoxProductHistoryPrice.Text = selectedRow.PricePerUnit.ToString() + " USD";
                TextBoxProductHistoryRate.Text = selectedRow.Rate.ToString() + " %";
                TextBoxProductHistoryTotalQuantity.Text = selectedRow.TotalQuantity.ToString();
                TextBoxProductHistoryDeliveryQuantity.Text = selectedRow.Quantity.ToString();
                TextBoxProductHistoryRegisteringPerson.Text = selectedRow.RegisteringPerson;
                TextBoxProductHistorySupplier.Text = selectedRow.Supplier;

                HideAllGrids();
                if (!dataGridReduced)
                {
                    originalDataGridWidth = DataGridWarehouse.ActualWidth;
                    reducedDataGridWidth = originalDataGridWidth - (originalDataGridWidth / 3.0);
                }

                if (DataGridWarehouse.ActualWidth <= reducedDataGridWidth)
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 550,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    GridProductInfo.BeginAnimation(WidthProperty, slideInAnimation);
                    GridProductInfo.Visibility = Visibility.Visible;
                }
                else if (!dataGridReduced && DataGridWarehouse.ActualWidth > reducedDataGridWidth)
                {
                    DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                    {
                        From = DataGridWarehouse.ActualWidth,
                        To = reducedDataGridWidth,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };

                    reduceWidthAnimation.Completed += (s, args) =>
                    {
                        DoubleAnimation slideInAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = 550,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };
                        GridProductInfo.Visibility = Visibility.Visible;

                        GridProductInfo.BeginAnimation(WidthProperty, slideInAnimation);
                    };

                    DataGridWarehouse.BeginAnimation(WidthProperty, reduceWidthAnimation);
                    dataGridReduced = true;
                }
            }
        }

        private void ButtonNewDelivery_Click(object sender, RoutedEventArgs e)
        {
            HideAllGrids();
            if (!dataGridReduced)
            {
                originalDataGridWidth = DataGridWarehouse.ActualWidth;
                reducedDataGridWidth = originalDataGridWidth - (originalDataGridWidth / 3.0);
            }

            if (DataGridWarehouse.ActualWidth <= reducedDataGridWidth)
            {
                DoubleAnimation slideInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 550,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                GridNewDelivery.BeginAnimation(WidthProperty, slideInAnimation);
                GridNewDelivery.Visibility = Visibility.Visible;
            }
            else if (!dataGridReduced && DataGridWarehouse.ActualWidth > reducedDataGridWidth)
            {
                DoubleAnimation reduceWidthAnimation = new DoubleAnimation
                {
                    From = DataGridWarehouse.ActualWidth,
                    To = reducedDataGridWidth,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                reduceWidthAnimation.Completed += (s, args) =>
                {
                    DoubleAnimation slideInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 550,
                        Duration = TimeSpan.FromSeconds(0.5)
                    };
                    GridNewDelivery.Visibility = Visibility.Visible;

                    GridNewDelivery.BeginAnimation(WidthProperty, slideInAnimation);
                };

                DataGridWarehouse.BeginAnimation(WidthProperty, reduceWidthAnimation);
                dataGridReduced = true;
            }
        }

        private void ButtonAddNewProduct_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(TextBoxProductName.Text))
                {
                    throw new FormatException("Please enter product name.");
                }
                if (ComboBoxProductMeasure.SelectedIndex == -1)
                {
                    throw new FormatException("Please select product measure.");
                }

                ComboBoxItem selectedItem = (ComboBoxItem)ComboBoxProductMeasure.SelectedItem;
                string measureTag = (string)selectedItem.Tag;

                if (ComboBoxProductVat.SelectedIndex == -1)
                {
                    throw new FormatException("Please select VAT rate.");
                }

                string priceInput = TextBoxPricePerUnit.Text;
                bool isPriceValid=Service.ValidatePrice(priceInput);

                if(isPriceValid) {
                    decimal pricePerUnit;
                    if (decimal.TryParse(priceInput, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out pricePerUnit))
                    {
                        decimal roundedPrice = Math.Round(pricePerUnit, 2);

                        if (ComboBoxProductType.SelectedIndex == -1)
                        {
                            throw new FormatException("Please select product type.");
                        }

                        Products newProduct = new Products
                        {
                            IdType = ComboBoxProductType.SelectedIndex + 1,
                            Measure = measureTag,
                            Name = TextBoxProductName.Text,
                            PricePerUnit = roundedPrice,
                            IdVAT = ComboBoxProductVat.SelectedIndex + 1,
                            Description = TextBoxProductDescription.Text,
                        };
                        Service.AddProduct(newProduct);
                        ClearFields();
                        LoadProducts();

                        MessageBox.Show("Product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadProducts()
        {
            Service.LoadProducts();
            ComboBoxProducts.ItemsSource = null;
            ComboBoxProducts.ItemsSource = Service.Products;
            ComboBoxProducts.DisplayMemberPath = "Name";
        }

        private void ClearFields()
        {
            TextBoxProductName.Text = string.Empty;
            ComboBoxProductType.SelectedIndex = -1;
            ComboBoxProductMeasure.SelectedIndex = -1;
            ComboBoxProductVat.SelectedIndex = -1;
            TextBoxPricePerUnit.Text = string.Empty;
            TextBoxProductDescription.Text = string.Empty;
            ComboBoxProducts.SelectedIndex = -1;
            TextBoxQuantity.Text = string.Empty;
            TextBoxSupplierCompany.Text = string.Empty;
            TextBoxDeliveryDate.Text = string.Empty;
        }


        private void ButtonRegisterDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxProducts.SelectedIndex == -1)
                {
                    throw new FormatException("Please select a product from the list.");
                }
                if (string.IsNullOrEmpty(TextBoxQuantity.Text))
                {
                    throw new FormatException("Please enter quantity.");
                }
                if (!int.TryParse(TextBoxQuantity.Text, out int quantity))
                {
                    throw new FormatException("Incorrect format for the quantity of the ordered product. Quantity must be an integer");
                }
                if (string.IsNullOrEmpty(TextBoxSupplierCompany.Text))
                {
                    throw new FormatException("Please enter the supplier's company name.");
                }

                string currentUserLogin = LogInControl.CurrentLogin;
                Guid userId = Service.GetUserId(currentUserLogin);

                ProductsHistory newDelivery = new ProductsHistory
                {
                    IdProduct = ComboBoxProducts.SelectedIndex + 1,
                    Quantity= quantity,
                    Supplier= TextBoxSupplierCompany.Text,
                    DateOfRegistration= DateTime.Today,
                    IdUser = userId,
                };

                if (DateTime.TryParseExact(TextBoxDeliveryDate.Text, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime deliveryDate))
                {
                    newDelivery.DateOfDelivery = deliveryDate;
                }
                else
                {
                    MessageBox.Show("The entered date is not in the correct format (dd.MM.yyyy). Please make sure to enter your birth date in the format day.month.year (e.g., 15.03.1990).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Service.AddProductHistory(newDelivery);
                ClearFields();
                LoadWarehouse();

                MessageBox.Show("Delivery registration was successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadWarehouse()
        {
            Service.LoadWarehouse();
            DataGridWarehouse.ItemsSource = null;
            DataGridWarehouse.ItemsSource = Service.Warehouse;
        }

        private void TextBoxFilterProducts_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = TextBoxFilterProducts.Text.ToLower();
            var filterProducts = Service.Warehouse.Where(x => x.Name.ToLower().Contains(filter) || x.TypeName.ToLower().Contains(filter) || x.RegisteringPerson.ToLower().Contains(filter)).ToList();
            DataGridWarehouse.ItemsSource = filterProducts;
        }

        private void DataGridWarehouse_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string currentUserLogin = LogInControl.CurrentLogin;
            Guid userId = Service.GetUserId(currentUserLogin);
            string userRole = Service.GetUserRole(userId);

            if (userRole == "Manager" || Service.GetUserPermissions(userId).Contains(1))
            {
                WarehouseView selectedProduct = (WarehouseView)DataGridWarehouse.SelectedItem;
                if(selectedProduct != null) 
                {
                    ShowContextMenu(selectedProduct);
                }
            }
        }

        private void ShowContextMenu(WarehouseView selectedProduct)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem menuItemSingle = new MenuItem { Header = "Change VAT rate for selected Product" };
            menuItemSingle.Click += (s, e) => MenuItemSingle_Click(s, e, selectedProduct);
            contextMenu.Items.Add(menuItemSingle);

            MenuItem menuItemAll = new MenuItem { Header = "Change VAT rate for every product of this type" };
            menuItemAll.Click += (s, e) => MenuItemAll_Click(s, e, selectedProduct);
            contextMenu.Items.Add(menuItemAll);

            DataGridWarehouse.ContextMenu = contextMenu;
            contextMenu.IsOpen = true;
        }

        private void MenuItemSingle_Click(object sender, RoutedEventArgs e, WarehouseView selectedProduct)
        {
            bool singleVAT = true;
            if (selectedProduct != null)
            {
                VATWindow vatWindow = new VATWindow(singleVAT, selectedProduct);
                vatWindow.ShowDialog();
                LoadWarehouse();
            }
        }

        private void MenuItemAll_Click(object sender, RoutedEventArgs e, WarehouseView selectedProduct)
        {
            bool singleVAT = false;
            if (selectedProduct != null)
            {
                VATWindow vatWindow = new VATWindow(singleVAT, selectedProduct);
                vatWindow.ShowDialog();
                LoadWarehouse();
            }
        }

    }
}
