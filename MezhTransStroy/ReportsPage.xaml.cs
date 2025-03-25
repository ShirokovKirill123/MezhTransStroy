using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.Entity;   // Для Entity Framework 

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReportsForRole();
        }

        private void LoadReportsForRole()
        {
            ReportsComboBox.Items.Clear();

            string userRole = Manager.User.Role;

            if (userRole == "планирование")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
            }
            else if (userRole == "склад")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
            }
            else if (userRole == "строительство")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
            }
            else if (userRole == "админ")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о пользователях" });
            }
            else
            {
                MessageBox.Show("Неизвестная роль пользователя");
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var data = ReportsDataGrid.ItemsSource as IEnumerable<object>;
            if (data == null || !data.Any())
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            // Реализация экспорта в CSV
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                FileName = "Отчёт.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    var properties = data.First().GetType().GetProperties();
                    writer.WriteLine(string.Join(";", properties.Select(p => p.Name)));

                    foreach (var item in data)
                    {
                        var values = properties.Select(p => p.GetValue(item)?.ToString());
                        writer.WriteLine(string.Join(";", values));
                    }
                }

                MessageBox.Show("Отчёт успешно экспортирован");
            }
        }

        private void ReportsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedReport = (ReportsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(selectedReport))
                return;

            using (var context = new СтроительствоEntities())
            {
                switch (selectedReport)
                {
                    case "Отчёт об отделах":
                        DepartmentalReport(context);
                        break;
                    case "Отчёт о сотрудниках":
                        DisplayEmployeesReport(context);
                        break;
                    case "Отчёт о строительных объектах":
                        OnConstructionProjectsReport(context);
                        break;
                    case "Отчёт о материалах":
                        MaterialsReport(context);
                        break;
                    case "Отчёт о поставщиках":
                        SupplierReport(context);
                        break;
                    case "Отчёт о складах":
                        WarehouseReport(context);
                        break;
                    case "Отчёт о заявках":
                        ApplicationReport(context);
                        break;
                    case "Отчёт о распределении материалов на объект":
                        OnMaterialDistributionToTheSiteReport(context);
                        break;
                    case "Отчёт о работе на объекте":
                        OnSiteWorkReport(context);
                        break;
                    case "Отчёт о пользователях":
                        DisplayUsersReport(context);
                        break;
                    default:
                        MessageBox.Show("Неизвестный отчёт");
                        break;

                }
            }
        }

        private void DepartmentalReport(СтроительствоEntities context)
        {
            var reportData = context.Отделы
            .Select(d => new
            {
                d.id,
                d.Название              
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
        }

        private void DisplayEmployeesReport(СтроительствоEntities context)
        {
            var reportData = context.Сотрудники
            .Include(e => e.Отделы)
            .Select(e => new
            {
                e.id,
                e.ФИО,
                e.Должность,
                e.Квалификация,
                e.Дата_Приёма,
                e.Контакты,
                Отдел = e.Отделы.Название
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ФИО", Binding = new Binding("ФИО") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Должность", Binding = new Binding("Должность") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Квалификация", Binding = new Binding("Квалификация") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата приёма", Binding = new Binding("Дата_Приёма") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контакты", Binding = new Binding("Контакты") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отдел", Binding = new Binding("Отдел") });
        }

        private void OnConstructionProjectsReport(СтроительствоEntities context)
        {
            var reportData = context.Строительные_Объекты
           .Select(n => new
           {
               n.id,
               n.Название,
               n.Адрес,
               n.Дата_Начала,
               n.Дата_Окончания
           })
           .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата начала", Binding = new Binding("Дата_Начала") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата окончания", Binding = new Binding("Дата_Окончания") { StringFormat = "dd.MM.yyyy" } });
        }

        private void MaterialsReport(СтроительствоEntities context)
        {
            var reportData = context.Материалы
            .Select(g => new
            {
                g.id,
                g.Название,
                g.Единица_Измерения,
                g.Стоимость
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Единица измерения", Binding = new Binding("Единица_Измерения") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость") });
        }


        private void SupplierReport(СтроительствоEntities context)
        {
            var reportData = context.Поставщики
            .Select(emp => new
            {
                emp.id,
                emp.Название,
                emp.Контактное_Лицо,
                emp.Телефон,
                emp.Адрес
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контактное лицо", Binding = new Binding("Контактное_Лицо") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Binding = new Binding("Телефон") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });
        }

        private void WarehouseReport(СтроительствоEntities context)
        {
            var reportData = context.Склад
            .Include(g => g.Материалы)
            .Include(g => g.Поставщики)
             .Select(g => new
             {
                 g.id,
                 Материал = g.Материалы.Название,
                 g.Количество,
                 Поставщик = g.Поставщики.Название,
                 g.Дата_Поступления
             })
             .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" } });
        }

        private void ApplicationReport(СтроительствоEntities context)
        {
            var reportData = context.Заявки
            .Include(m => m.Материалы)
            .Include(m => m.Строительные_Объекты)
            .Select(m => new
            {
                ID = m.id,
                Объект = m.Строительные_Объекты.Название,
                Материал = m.Материалы.Название,
                Количество = m.Количество,
                Статус = m.Статус
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
        }

        private void OnMaterialDistributionToTheSiteReport(СтроительствоEntities context)
        {
            var reportData = context.Распределение_Материалов_На_Объект
              .Include(m => m.Материалы)
              .Include(m => m.Строительные_Объекты)
              .Select(m => new
              {
                  ID = m.id,
                  Объект = m.Строительные_Объекты.Название,
                  Материал = m.Материалы.Название,
                  m.Количество,
                  Дата_Передачи = m.Дата_Передачи
              })
              .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата_Передачи", Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" } });
        }

        private void OnSiteWorkReport(СтроительствоEntities context)
        {
            var reportData = context.Работа_На_Объекте
            .Include(m => m.Сотрудники)
            .Include(m => m.Строительные_Объекты)
            .Select(b => new
            {
                ID = b.id,
                Сотрудник = b.Сотрудники.ФИО,
                Объект = b.Строительные_Объекты.Название,
                Дата_Назначения = b.Дата_Назначения
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник", Binding = new Binding("Сотрудник") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата назначения", Binding = new Binding("Дата_Назначения") { StringFormat = "dd.MM.yyyy" } });
        }


        private void DisplayUsersReport(СтроительствоEntities context)
        {
            var reportData = context.Пользователи
           .Select(u => new
           {
               u.id,
               u.Логин,
               u.Пароль,
               Роль = u.Уровень_Доступа
           })
           .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Логин", Binding = new Binding("Логин") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Пароль", Binding = new Binding("Пароль") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Уровень Доступа", Binding = new Binding("Роль") });
        }
    }
}
