using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для RedirectQuantityWindow.xaml
    /// </summary>
    public partial class RedirectQuantityWindow : Window
    {
        public int Quantity { get; private set; }
        private int maxQuantity;

        public RedirectQuantityWindow(string materialName, int maxQuantity)
        {
            InitializeComponent();
            this.maxQuantity = maxQuantity;
            MaterialTextBlock.Text = $"Материал: {materialName}";
            MaxQuantityTextBlock.Text = $"Максимально доступно для перенаправления: {maxQuantity}";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0 || quantity > maxQuantity)
            {
                MessageBox.Show($"Введите корректное количество (1 - {maxQuantity})", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Quantity = quantity;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
