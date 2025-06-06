using MezhTransStroy.Database;
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
    /// Логика взаимодействия для SelectObjectWindow.xaml
    /// </summary>
    public partial class SelectObjectWindow : Window
    {
        public int SelectedObjectId { get; private set; }

        public SelectObjectWindow()
        {
            InitializeComponent();
            LoadObjects();
        }

        private void LoadObjects()
        {
            using (var context = new СтроительствоEntities())
            {
                var objects = context.Строительные_Объекты
                    .Select(o => new { o.id, o.Название })
                    .ToList();

                ObjectComboBox.ItemsSource = objects;
                ObjectComboBox.DisplayMemberPath = "Название";
                ObjectComboBox.SelectedValuePath = "id";
            }
        }

        public void SetEquipmentName(string equipmentName)
        {
            EquipmentNameTextBlock.Text = $"Оборудование: {equipmentName}";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ObjectComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите объект", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedObjectId = (int)ObjectComboBox.SelectedValue;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
