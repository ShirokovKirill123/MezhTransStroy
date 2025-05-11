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
using ClosedXML.Excel;


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
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заработной плате сотрудников" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
            }
            else if (userRole == "склад")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
            }
            else if (userRole == "строительство")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
            }
            else if (userRole == "админ")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заявках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заработной плате сотрудников" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
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

            //экспорт в формат
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = "Отчёт.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.AddWorksheet("Отчёт");

                    var properties = data.First().GetType().GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = properties[i].Name;
                    }

                    int row = 2; // Начинаем с 2-й строки т.к. первая строка — заголовки
                    foreach (var item in data)
                    {
                        for (int col = 0; col < properties.Length; col++)
                        {
                            var value = properties[col].GetValue(item)?.ToString() ?? "";
                            worksheet.Cell(row, col + 1).Value = value;
                        }
                        row++;
                    }

                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Отчёт успешно экспортирован в Excel");
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
                    case "Отчёт об оборудовании":
                        EquipmentReport(context);
                        break;
                    case "Отчёт о поставщиках":
                        SupplierReport(context);
                        break;
                    case "Отчёт о складах":
                        WarehouseReport(context);
                        break;
                    case "Отчёт о материалах на складах":
                        Materials_In_StockReport(context);
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
                    case "Отчёт о затратах на оборудование":
                        Equipment_СostsReport(context);
                        break;
                    case "Отчёт о заработной плате сотрудников":
                        Salary_Wage_EmployeesReport(context);
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
               n.Дата_Окончания,
               n.Выделенный_Бюджет
           })
           .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата начала", Binding = new Binding("Дата_Начала") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата окончания", Binding = new Binding("Дата_Окончания") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Выделенный бюджет", Binding = new Binding("Выделенный_Бюджет") { StringFormat = "{0:N2} ₽" } });
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость") { StringFormat = "{0:N2} ₽" } });
        }

        private void EquipmentReport(СтроительствоEntities context)
        {
            var reportData = context.Оборудование
            .Select(g => new
            {
                g.id,
                g.Название,
                g.Тип
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new Binding("Тип") });
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
            var reportData = context.Склады
             .Select(g => new
             {
                 g.id,
                 g.Номер_Склада
             })
             .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер склада", Binding = new Binding("Номер_Склада") });
        }
        
        private void Materials_In_StockReport(СтроительствоEntities context)
        {
            var reportData = context.Материалы_На_Складах
            .Include(g => g.Материалы)
            .Include(g => g.Поставщики)
            .Include(g => g.Склады)
             .Select(g => new
             {
                 g.id,
                 Склад = g.Склады.id,
                 Материал = g.Материалы.Название,
                 Поставщик = g.Поставщики.Название,
                 g.Количество,
                 g.Стоимость_Материалов,
                 g.Дата_Поступления
             })
             .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new Binding("Склад") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" } });
        }

        private void ApplicationReport(СтроительствоEntities context)
        {
            var reportData = context.Заявки
            .Include(m => m.Материалы)
            .Include(m => m.Строительные_Объекты)
            .Include(m => m.Склады)
            .Include(m => m.Поставщики)
            .Select(m => new
            {
                ID = m.id,
                Объект = m.Строительные_Объекты.Название,
                Склад = m.Склады.id,
                Поставщик = m.Поставщики.Название,
                Материал = m.Материалы.Название,
                Количество = m.Количество_Материала,
                Стоимость_Материалов = m.Стоимость_Материалов,
                Статус = m.Статус,
                Дата_Заявки = m.Дата_Заявки
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new Binding("Склад") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата заявки", Binding = new Binding("Дата_Заявки") { StringFormat = "dd.MM.yyyy" } });
        }

        private void OnMaterialDistributionToTheSiteReport(СтроительствоEntities context)
        {
            var reportData = context.Распределение_Материалов_На_Объект
              .Include(m => m.Материалы)
              .Include(m => m.Строительные_Объекты)
              .Include(m => m.Склады)
              .Select(m => new
              {
                  ID = m.id,
                  Склад = m.Склады.id,
                  Объект = m.Строительные_Объекты.Название,
                  Материал = m.Материалы.Название,
                  m.Количество,
                  Стоимость_Материалов = m.Стоимость_Материалов,
                  Дата_Передачи = m.Дата_Передачи
              })
              .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });           
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new Binding("Склад") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата передачи", Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" } });
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
                Дата_Назначения = b.Дата_Назначения,
                Статус = b.Статус
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник", Binding = new Binding("Сотрудник") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата назначения", Binding = new Binding("Дата_Назначения") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
        }

        private void Equipment_СostsReport(СтроительствоEntities context)
         {
            var reportData = context.Затраты_На_Оборудование
            .Include(m => m.Оборудование)
            .Include(m => m.Строительные_Объекты)
            .Select(b => new
            {
                ID = b.id,
                Объект = b.Строительные_Объекты.Название,
                Оборудование = b.Оборудование.Название,
                Часы_Работы = b.Часы_Работы,
                Стоимость_в_Час = b.Стоимость_в_Час,
                Затраты = b.Затраты
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оборудование", Binding = new Binding("Оборудование") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Часы работы", Binding = new Binding("Часы_Работы") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость в час", Binding = new Binding("Стоимость_в_Час") { StringFormat = "{0:N2} ₽" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты", Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" } });
        }

        private void Salary_Wage_EmployeesReport(СтроительствоEntities context)
        {
            var reportData = context.Заработная_Плата_Сотрудников
            .Include(m => m.Сотрудники)
            .Select(b => new
            {
                ID = b.id,
                Сотрудник = b.Сотрудники.ФИО,
                Ставка_в_День = b.Ставка_в_День,
                Отработано_Дней = b.Отработано_Дней,
                Затраты = b.Затраты
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник", Binding = new Binding("Сотрудник") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Ставка в день", Binding = new Binding("Ставка_в_День") { StringFormat = "{0:N2} ₽" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отработано дней", Binding = new Binding("Отработано_Дней") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты", Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" } });
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Уровень доступа", Binding = new Binding("Роль") });
        }
    }
}
