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
    /// Логика взаимодействия для Application_Materials_At_ObjectPage.xaml
    /// </summary>
    public partial class Application_Materials_At_ObjectPage : Page
    {
        public Application_Materials_At_ObjectPage()
        {
            InitializeComponent();
            LoadUsedMaterials();
            LoadMyApplications();          
        }

        private void LoadMyApplications()
        {
            string fileName = "Заявки.json";

            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                var applications = JsonConvert.DeserializeObject<List<dynamic>>(json);

                var myApplications = applications
                    .Where(a => (int)a.EmployeeId == Manager.User.Employee)
                    .Select(a => new
                    {
                        Material = (string)a.Material,
                        Quantity = (int)a.Quantity,
                        Unit = (string)a.Unit,
                        Date = (string)a.Date,
                        ObjectName = (string)a.ObjectName 
                    })
                    .ToList();

                MyApplicationsListBox.ItemsSource = myApplications;

                NoApplicationsTextBlock.Visibility = myApplications.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            else
            {
                MyApplicationsListBox.ItemsSource = null;
                NoApplicationsTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void DeleteApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedApplication = (button.DataContext as dynamic);

            string fileName = "Заявки.json";
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                var applications = JsonConvert.DeserializeObject<List<dynamic>>(json);

                var appToRemove = applications.FirstOrDefault(a =>
                    (string)a.Material == (string)selectedApplication.Material &&
                    (string)a.Date == (string)selectedApplication.Date &&
                    (int)a.EmployeeId == Manager.User.Employee);

                if (appToRemove != null)
                {
                    applications.Remove(appToRemove);
                    string updatedJson = JsonConvert.SerializeObject(applications, Formatting.Indented);
                    File.WriteAllText(fileName, updatedJson);
                    LoadMyApplications();
                }
            }
        }

        private void LoadUsedMaterials()
        {
            using (var context = new СтроительствоEntities())
            {
                var материалы = context.Материалы.ToList();

                var materialsList = context.Распределение_Материалов_На_Объект
                    .Include(r => r.Строительные_Объекты)
                    .Include(r => r.Склады)
                    .Include(r => r.Материалы)
                    .ToList();

                foreach (var item in materialsList)
                {
                    var материалItem = материалы.FirstOrDefault(m => m.id == item.id_Материала);
                    if (материалItem != null)
                        item.Стоимость_Материалов = item.Количество * материалItem.Стоимость;
                }

                var usedMaterialsOnly = materialsList
                    .Where(m => m.Количество == m.Израсходовано)
                    .ToList();

                UsedMaterialsDataGrid.ItemsSource = usedMaterialsOnly;
                UsedMaterialsDataGrid.Columns.Clear();

                var textStyle = new Style(typeof(TextBlock));
                textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Binding = new Binding("id"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Объект",
                    Binding = new Binding("Строительные_Объекты.Название"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Склад",
                    Binding = new Binding("Склады.Номер_Склада"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Материал",
                    Binding = new Binding("Материалы.Название"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Количество",
                    Binding = new Binding("Количество"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Ед. изм.",
                    Binding = new Binding("Материалы.Единица_Измерения"),
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата передачи",
                    Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = textStyle
                });
                UsedMaterialsDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Израсходовано",
                    Binding = new Binding("Израсходовано"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    ElementStyle = textStyle
                });
                foreach (var column in UsedMaterialsDataGrid.Columns)
                {
                    column.Width = DataGridLength.Auto;
                }
                if (UsedMaterialsDataGrid.Columns.Count > 0)
                    UsedMaterialsDataGrid.Columns.Last().Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private void CreateApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateApplicationWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadMyApplications();
            }
        }      
    }
}
