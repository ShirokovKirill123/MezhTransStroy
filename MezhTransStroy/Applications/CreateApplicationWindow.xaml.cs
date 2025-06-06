using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Globalization;

using Newtonsoft.Json;
using System.Data.Entity; // Для Entity Framework 
using System.Data.Entity.Infrastructure;
using System.Windows.Forms.DataVisualization.Charting;
using MezhTransStroy.Database;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для CreateApplicationWindow.xaml
    /// </summary>
    public partial class CreateApplicationWindow : Window
    {
        private СтроительствоEntities context;

        public string MaterialName { get; private set; }
        public int Quantity { get; private set; }
        public int ObjectId { get; private set; }

        public CreateApplicationWindow()
        {
            InitializeComponent();
            context = new СтроительствоEntities();
            LoadMaterials();
            LoadObjects();
        }

        private void LoadMaterials()
        {
            var materials = context.Материалы.ToList();
            MaterialComboBox.ItemsSource = materials;
        }

        private void LoadObjects()
        {
            var objects = context.Строительные_Объекты.ToList();
            ObjectComboBox.ItemsSource = objects;
        }

        private void MaterialComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MaterialComboBox.SelectedItem is Материалы selectedMaterial)
            {
                UnitTextBlock.Text = selectedMaterial.Единица_Измерения;
            }
            else
            {
                UnitTextBlock.Text = "Ед. изм.";
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое положительное число)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ObjectComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите строительный объект", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedMaterial = (Материалы)MaterialComboBox.SelectedItem;
            var selectedObject = (Строительные_Объекты)ObjectComboBox.SelectedItem;

            MaterialName = selectedMaterial.Название;
            Quantity = quantity;
            ObjectId = (int)ObjectComboBox.SelectedValue;

            var newApplication = new
            {
                Material = MaterialName,
                Quantity = Quantity,
                Unit = selectedMaterial.Единица_Измерения,
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                EmployeeId = Manager.User.Employee,
                ObjectName = selectedObject.Название,
                ObjectId = selectedObject.id
            };

            var fileName = "Заявки.json";
            List<dynamic> applications;

            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                applications = JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new List<dynamic>();
            }
            else
            {
                applications = new List<dynamic>();
            }

            applications.Add(newApplication);

            var updatedJson = JsonConvert.SerializeObject(applications, Formatting.Indented);
            File.WriteAllText(fileName, updatedJson);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
