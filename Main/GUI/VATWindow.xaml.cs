using System.Windows;


namespace Main.GUI
{
    /// <summary>
    /// Logika interakcji dla klasy VATWindow.xaml
    /// </summary>
    public partial class VATWindow : Window
    {
        public VATWindow(bool singleVAT, WarehouseView selectedProduct)
        {
            InitializeComponent();
            InitializeVATControls(singleVAT, selectedProduct);
        }

        private void InitializeVATControls(bool singleVAT, WarehouseView selectedProduct)
        {
            if(singleVAT)
            {
                VATControl vatControl = new VATControl(selectedProduct);
                ContentControlVAT.Content = vatControl;
            }
            else
            {
                VATTypeControl vatTypeControl = new VATTypeControl(selectedProduct);
                ContentControlVAT.Content = vatTypeControl;
            }
        }
    }
}
