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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.Entity;
using ClosedXML.Excel;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для CostsPage.xaml
    /// </summary>
    public partial class CostsPage : Page
    {
        public CostsPage()
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
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости заявок" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов на объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заработной плате сотрудников" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
            }            
            else if (userRole == "админ")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости заявок" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о стоимости материалов на объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заработной плате сотрудников" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
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
                    case "Отчёт о строительных объектах":
                        OnConstructionProjectsReport(context);
                        break;
                    case "Отчёт о стоимости материалов":
                        MaterialsCostReport(context);
                        break;
                    case "Отчёт о стоимости материалов на складах":
                        OnCostOfMaterialsInWarehousesReport(context);
                        break;
                    case "Отчёт о стоимости заявок":
                        OnTheCostOfApplicationsReport(context);
                        break;
                    case "Отчёт о стоимости материалов на объектах":
                        OnTheCostOfMaterialsOnObjectsReport(context);
                        break;
                    case "Отчёт о заработной плате сотрудников":
                        EmployeeSalaryReport(context);
                        break;                   
                    case "Отчёт о затратах на оборудование":
                        EquipmentCostReport(context);
                        break;                 
                    default:
                        MessageBox.Show("Неизвестный отчёт");
                        break;
                }
            }
        }

        private void OnConstructionProjectsReport(СтроительствоEntities context)
        {
            var reportData = context.Строительные_Объекты
                .Select(obj => new
                {
                    obj.id,
                    obj.Название,
                    obj.Выделенный_Бюджет,

                    ЗатратыНаОборудование = context.Затраты_На_Оборудование
                        .Where(z => z.id_Объекта == obj.id)
                        .Sum(z => (decimal?)z.Затраты) ?? 0,

                    ЗатратыНаЗарплату = context.Работа_На_Объекте
                        .Where(r => r.id_Объекта == obj.id)
                        .Select(r => context.Заработная_Плата_Сотрудников
                            .Where(z => z.id_Сотрудника == r.id_Сотрудника)
                            .Select(z => (decimal?)z.Затраты)
                            .FirstOrDefault() ?? 0)
                        .Sum(),

                    ЗатратыНаМатериалы = context.Распределение_Материалов_На_Объект
                        .Where(m => m.id_Объекта == obj.id)
                        .Sum(m => (decimal?)m.Стоимость_Материалов) ?? 0
                })
                .ToList()
                .Select(d => new
                {
                    d.id,
                    d.Название,
                    d.Выделенный_Бюджет,
                    d.ЗатратыНаОборудование,
                    d.ЗатратыНаЗарплату,
                    d.ЗатратыНаМатериалы,
                    Всего_Затрат = d.ЗатратыНаОборудование + d.ЗатратыНаЗарплату + d.ЗатратыНаМатериалы,
                    Прибыль = d.Выделенный_Бюджет - (d.ЗатратыНаОборудование + d.ЗатратыНаЗарплату + d.ЗатратыНаМатериалы)
                })
                .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Выделенный бюджет", Binding = new Binding("Выделенный_Бюджет") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты на оборудование", Binding = new Binding("ЗатратыНаОборудование") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты на оплату труда", Binding = new Binding("ЗатратыНаЗарплату") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты на материалы", Binding = new Binding("ЗатратыНаМатериалы") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Всего затрат", Binding = new Binding("Всего_Затрат") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Прибыль", Binding = new Binding("Прибыль") });
        }

        private void MaterialsCostReport(СтроительствоEntities context)
        {
            var reportData = context.Материалы
           .Select(g => new
           {
               g.Название,
               g.Единица_Измерения,
               g.Стоимость
           })
           .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Единица измерения", Binding = new Binding("Единица_Измерения") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость") });
        }

        private void OnCostOfMaterialsInWarehousesReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" } });
        }

        private void OnTheCostOfApplicationsReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата заявки", Binding = new Binding("Дата_Заявки") { StringFormat = "dd.MM.yyyy" } });
        }

        private void OnTheCostOfMaterialsOnObjectsReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Binding = new Binding("Стоимость_Материалов") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата передачи", Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" } });
        }

        private void EmployeeSalaryReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Ставка в день", Binding = new Binding("Ставка_в_День") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отработано дней", Binding = new Binding("Отработано_Дней") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты", Binding = new Binding("Затраты") });
        }

        private void EquipmentCostReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость в час", Binding = new Binding("Стоимость_в_Час") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты", Binding = new Binding("Затраты") });
        }        
    }
}
