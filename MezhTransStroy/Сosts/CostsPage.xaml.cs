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
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.DataVisualization.Charting;
using MezhTransStroy.Database;

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

            if (userRole == "админ" || userRole == "бухгалтерия")
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

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = "Отчёт_по_затратам.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.AddWorksheet("Отчёт");

                    var properties = data.First().GetType().GetProperties();
                    int totalColumns = properties.Length;

                    string reportTitle = (ReportsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Отчёт";

                    worksheet.Range(2, 1, 2, totalColumns).Merge();
                    var titleCell = worksheet.Cell(2, 1);
                    titleCell.Value = reportTitle;
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.FontSize = 16;
                    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < totalColumns; i++)
                    {
                        worksheet.Cell(3, i + 1).Value = properties[i].Name;
                        worksheet.Cell(3, i + 1).Style.Font.Bold = true;
                    }

                    int row = 4;
                    foreach (var item in data)
                    {
                        for (int col = 0; col < totalColumns; col++)
                        {
                            var value = properties[col].GetValue(item);
                            var cell = worksheet.Cell(row, col + 1);

                            var culture = new CultureInfo("ru-RU");
                            if (value is decimal || value is double || value is float)
                            {
                                double number = Convert.ToDouble(value);
                                string formattedValue = number.ToString("N2", culture) + " ₽";
                                cell.Value = formattedValue;
                            }
                            else
                            {
                                cell.Value = value?.ToString() ?? "";
                            }
                        }
                        row++;
                    }

                    // Пользователь и дата 
                    using (var context = new СтроительствоEntities())
                    {
                        int employeeId = Manager.User.Employee;

                        var user = context.Сотрудники
                            .Where(emp => emp.id == employeeId)
                            .Select(emp => emp.ФИО)
                            .FirstOrDefault() ?? "Неизвестный пользователь";

                        worksheet.Cell(row + 2, 1).Value = "Пользователь:";
                        worksheet.Cell(row + 2, 2).Value = user;

                        worksheet.Cell(row + 2, totalColumns - 1).Value = "Дата:";
                        worksheet.Cell(row + 2, totalColumns).Value = DateTime.Now.ToString("dd.MM.yyyy");
                    }

                    worksheet.Columns().AdjustToContents();
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

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Бюджет", Binding = new Binding("Выделенный_Бюджет") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оборудование", Binding = new Binding("ЗатратыНаОборудование") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Зарплата", Binding = new Binding("ЗатратыНаЗарплату") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материалы", Binding = new Binding("ЗатратыНаМатериалы") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Всего", Binding = new Binding("Всего_Затрат") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Прибыль", Binding = new Binding("Прибыль") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            for (int i = 2; i <= 7; i++)
                ReportsDataGrid.Columns[i].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
        }

        private void MaterialsCostReport(СтроительствоEntities context)
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

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Название"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Единица измерения", Binding = new Binding("Единица_Измерения"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
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
                    Единица_Измерения = g.Материалы.Единица_Измерения,
                    g.Стоимость_Материалов,
                    g.Дата_Поступления
                })
                .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            var textStyle = new Style(typeof(TextBlock));
            textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new Binding("Склад"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Ед. изм.",
                Binding = new Binding("Единица_Измерения"),
                IsReadOnly = true,
                ElementStyle = textStyle,
                Width = DataGridLength.Auto
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" }, ElementStyle = centerStyle });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[5].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[6].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
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
                    m.id,
                    Объект = m.Строительные_Объекты.Название,
                    Склад = m.Склады.id,
                    Поставщик = m.Поставщики.Название,
                    Материал = m.Материалы.Название,
                    Количество = m.Количество_Материала,
                    Единица_Измерения = m.Материалы.Единица_Измерения,
                    Стоимость_Материалов = m.Стоимость_Материалов,
                    Статус = m.Статус,
                    Дата_Заявки = m.Дата_Заявки
                })
                .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            var textStyle = new Style(typeof(TextBlock));
            textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new Binding("Склад"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество"), ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Ед. изм.",
                Binding = new Binding("Единица_Измерения"),
                IsReadOnly = true,
                ElementStyle = textStyle,
                Width = DataGridLength.Auto
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" }, ElementStyle = centerStyle });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата заявки", Binding = new Binding("Дата_Заявки") { StringFormat = "dd.MM.yyyy" }, ElementStyle = centerStyle });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(0.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[5].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[6].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[7].Width = new DataGridLength(0.7, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[8].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
        }

        private void OnTheCostOfMaterialsOnObjectsReport(СтроительствоEntities context)
        {
            var reportData = context.Распределение_Материалов_На_Объект
               .Include(m => m.Материалы)
               .Include(m => m.Строительные_Объекты)
               .Include(m => m.Склады)
               .Select(m => new
               {
                   m.id,
                   Склад = m.Склады.id,
                   Объект = m.Строительные_Объекты.Название,
                   Материал = m.Материалы.Название,
                   m.Количество,
                   Единица_Измерения = m.Материалы.Единица_Измерения,
                   Стоимость_Материалов = m.Стоимость_Материалов,
                   Дата_Передачи = m.Дата_Передачи
               })
               .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            var textStyle = new Style(typeof(TextBlock));
            textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Склад",
                Binding = new Binding("Склад"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Материал",
                Binding = new Binding("Материал"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Количество",
                Binding = new Binding("Количество"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Ед. изм.",
                Binding = new Binding("Единица_Измерения"),
                IsReadOnly = true,
                ElementStyle = textStyle,
                Width = DataGridLength.Auto
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Стоимость материалов",
                Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата передачи",
                Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[5].Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[6].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
        }

        private void EmployeeSalaryReport(СтроительствоEntities context)
        {
            var reportData = context.Заработная_Плата_Сотрудников
            .Include(m => m.Сотрудники)
            .Select(b => new
            {
                b.id,
                Сотрудник = b.Сотрудники.ФИО,
                Ставка_в_День = b.Ставка_в_День,
                Отработано_Дней = b.Отработано_Дней,
                Затраты = b.Затраты
            })
            .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник", Binding = new Binding("Сотрудник") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Ставка в день",
                Binding = new Binding("Ставка_в_День") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Отработано дней",
                Binding = new Binding("Отработано_Дней"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Затраты",
                Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
        }

        private void EquipmentCostReport(СтроительствоEntities context)
        {
            var reportData = context.Затраты_На_Оборудование
                .Include(m => m.Оборудование)
                .Include(m => m.Строительные_Объекты)
                .Select(b => new
                {
                    b.id,
                    Объект = b.Строительные_Объекты.Название,
                    Оборудование = b.Оборудование.Название,
                    Стоимость_в_Час = b.Оборудование.Стоимость_в_Час,
                    Часы_Работы = b.Часы_Работы,
                    Затраты = b.Затраты
                })
                .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оборудование", Binding = new Binding("Оборудование") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Часы работы",
                Binding = new Binding("Часы_Работы"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Стоимость в час",
                Binding = new Binding("Стоимость_в_Час") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Затраты",
                Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[5].Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            var itemsSource = ReportsDataGrid.ItemsSource as IEnumerable<object>;
            if (itemsSource == null || !itemsSource.Any())
            {
                MessageBox.Show("Данные таблицы пусты, невозможно выполнить фильтрацию",
                                "Фильтрация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string idFilter = TextBoxID.Text.Trim();
            string nameFilter = TextBoxName.Text.Trim();

            if (string.IsNullOrWhiteSpace(idFilter) && string.IsNullOrWhiteSpace(nameFilter))
            {
                MessageBox.Show("Невозможно выполнить фильтрацию: фильтр не задан");
                return;
            }

            int idValue = 0;
            bool hasIdFilter = false;

            if (!string.IsNullOrWhiteSpace(idFilter))
            {
                if (!int.TryParse(idFilter, out idValue))
                {
                    MessageBox.Show("Некорректное значение в поле ID. Введите целое число",
                                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                hasIdFilter = true;
            }

            try
            {
                var filteredList = itemsSource.Where(item =>
                {
                    bool matchesId = true;
                    if (hasIdFilter)
                    {
                        var idProp = item.GetType().GetProperty("id");
                        if (idProp != null)
                        {
                            var value = idProp.GetValue(item);
                            if (value is int intValue)
                            {
                                matchesId = intValue == idValue;
                            }
                            else
                            {
                                matchesId = false;
                            }
                        }
                        else
                        {
                            matchesId = false;
                        }
                    }

                    bool matchesName = true;
                    if (!string.IsNullOrWhiteSpace(nameFilter))
                    {
                        matchesName = item.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(item) as string)
                            .Any(val => !string.IsNullOrEmpty(val) &&
                                        val.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                    }

                    return matchesId && matchesName;

                }).ToList();

                if (filteredList.Count == 0)
                {
                    MessageBox.Show("Фильтрация не дала результатов. Попробуйте изменить фильтр");
                    return;
                }

                ReportsDataGrid.ItemsSource = filteredList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка фильтрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string reportName = selectedItem.Content.ToString();

                using (var context = new СтроительствоEntities())
                {
                    switch (reportName)
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
            else
            {
                MessageBox.Show("Не выбран отчёт для обновления");
            }
        }
    }
}
