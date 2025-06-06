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

using System.Data.Entity;// Для Entity Framework 
using System.Globalization;
using System.Windows.Controls.Primitives;
using MezhTransStroy.Database;

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
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на складах" });             
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
            }
            else if (userRole == "склад")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о поставщиках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
            }
            else if (userRole == "строительство")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о работе на объекте" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
            }
            else if (userRole == "бухгалтерия")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о распределении материалов на объект" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах на складах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о затратах на оборудование" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о заработной плате сотрудников" });
            }
            else if (userRole == "админ")
            {
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об отделах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о сотрудниках" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о строительных объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт о материалах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на объектах" });
                ReportsComboBox.Items.Add(new ComboBoxItem { Content = "Отчёт об оборудовании на складах" });
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
                    case "Отчёт об оборудовании на складах":
                        Equipment_In_Storage_Click(context);
                        break;
                    case "Отчёт об оборудовании на объектах":
                        Distribution_Of_Equipment_To_The_Site_Click(context);
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
                d.Название,
                d.Телефон
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Название отдела",
                Binding = new Binding("Название"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Телефон",
                Binding = new Binding("Телефон"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата приёма",
                Binding = new Binding("Дата_Приёма") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Контакты",
                Binding = new Binding("Контакты"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Отдел", Binding = new Binding("Отдел") });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(2.7, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(3.1, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.9, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1.4, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[5].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[6].Width = new DataGridLength(1.8, DataGridLengthUnitType.Star);
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

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата начала",
                Binding = new Binding("Дата_Начала") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата окончания",
                Binding = new Binding("Дата_Окончания") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Выделенный бюджет",
                Binding = new Binding("Выделенный_Бюджет") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
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
                Header = "Материал",
                Binding = new Binding("Название"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Единица измерения",
                Binding = new Binding("Единица_Измерения"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Стоимость",
                Binding = new Binding("Стоимость") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
        }

        private void EquipmentReport(СтроительствоEntities context)
        {
            var reportData = context.Оборудование
            .Select(g => new
            {
                g.id,
                g.Название,
                g.Тип,
                g.Год_Выпуска,
                g.Производитель,
                g.Стоимость_в_Час
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Оборудование", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new Binding("Тип") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Год выпуска",
                Binding = new Binding("Год_Выпуска"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Производитель", Binding = new Binding("Производитель") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Стоимость в час",
                Binding = new Binding("Стоимость_в_Час") { StringFormat = "{0:N2} ₽" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
        }

        private void Equipment_In_Storage_Click(СтроительствоEntities context)
        {
            var reportData = context.Оборудование_На_Складах
                .Include(o => o.Оборудование)
                .Include(o => o.Склады)
                .Select(o => new
                {
                    o.id,
                    Склад = o.Склады.id,
                    Оборудование = o.Оборудование.Название,
                    o.Статус
                })
                .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            centerStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = centerStyle
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Склад",
                Binding = new Binding("Склад"),
                ElementStyle = centerStyle
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Оборудование",
                Binding = new Binding("Оборудование"),
                ElementStyle = centerStyle
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Статус",
                Binding = new Binding("Статус"),
                ElementStyle = centerStyle
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void Distribution_Of_Equipment_To_The_Site_Click(СтроительствоEntities context)
        {
            var reportData = context.Распределение_Оборудования_На_Объект
                 .Include(r => r.Оборудование)
                 .Include(r => r.Склады)
                 .Include(r => r.Строительные_Объекты)
                 .Select(r => new
                 {
                     r.id,
                     Склад = r.Склады.id,
                     Объект = r.Строительные_Объекты.Название,
                     Оборудование = r.Оборудование.Название,
                     r.Дата_Передачи
                 })
                 .ToList();

            ReportsDataGrid.ItemsSource = reportData;
            ReportsDataGrid.Columns.Clear();

            var centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            centerStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = centerStyle
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Склад",
                Binding = new Binding("Склад"),
                ElementStyle = centerStyle
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Объект",
                Binding = new Binding("Объект")
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Оборудование",
                Binding = new Binding("Оборудование")
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата передачи",
                Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = centerStyle
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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

            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("id"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Название") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Контактное лицо", Binding = new Binding("Контактное_Лицо") { StringFormat = "dd.MM.yyyy" } });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Телефон",
                Binding = new Binding("Телефон"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
        }

        private void WarehouseReport(СтроительствоEntities context)
        {
            var reportData = context.Склады
             .Select(g => new
             {
                 g.id,
                 g.Номер_Склада,
                 g.Адрес,
                 g.Телефон
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Номер склада",
                Binding = new Binding("Номер_Склада"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Binding = new Binding("Адрес") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Телефон",
                Binding = new Binding("Телефон"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
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
                Header = "Дата поступления",
                Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Склад",
                Binding = new Binding("Склад"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new Binding("Поставщик") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
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
                Header = "Статус",
                Binding = new Binding("Статус"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата заявки",
                Binding = new Binding("Дата_Заявки") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(1.7, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(1.6, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[6].Width = new DataGridLength(1.1, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[7].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[8].Width = new DataGridLength(1.2, DataGridLengthUnitType.Star);
        }

        private void OnMaterialDistributionToTheSiteReport(СтроительствоEntities context)
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new Binding("Материал") });
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
            ReportsDataGrid.Columns[1].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(0.5, DataGridLengthUnitType.Star);
        }

        private void OnSiteWorkReport(СтроительствоEntities context)
        {
            var reportData = context.Работа_На_Объекте
            .Include(m => m.Сотрудники)
            .Include(m => m.Строительные_Объекты)
            .Select(b => new
            {
                b.id,
                Сотрудник = b.Сотрудники.ФИО,
                Объект = b.Строительные_Объекты.Название,
                Дата_Назначения = b.Дата_Назначения,
                Статус = b.Статус
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Binding = new Binding("Объект") });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата назначения",
                Binding = new Binding("Дата_Назначения") { StringFormat = "dd.MM.yyyy" },
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Статус",
                Binding = new Binding("Статус"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[1].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[2].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[3].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            ReportsDataGrid.Columns[4].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
        }

        private void Equipment_СostsReport(СтроительствоEntities context)
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
                Затраты = b.Оборудование.Стоимость_в_Час * b.Часы_Работы
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

        private void Salary_Wage_EmployeesReport(СтроительствоEntities context)
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
        }

        private void DisplayUsersReport(СтроительствоEntities context)
        {
            var reportData = context.Пользователи
           .Include(m => m.Сотрудники)
           .Select(u => new
           {
               u.id,
               u.Логин,
               u.Пароль,
               Роль = u.Уровень_Доступа,
               Сотрудник = u.Сотрудники.ФИО
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
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Логин",
                Binding = new Binding("Логин"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Пароль",
                Binding = new Binding("Пароль"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Уровень доступа",
                Binding = new Binding("Роль"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                }
            });
            ReportsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Сотрудник", Binding = new Binding("Сотрудник") });

            ReportsDataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
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
                            MessageBox.Show("Выберите отчёт из списка");
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
