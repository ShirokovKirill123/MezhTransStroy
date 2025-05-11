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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для ObjectInfoPage.xaml
    /// </summary>
    public partial class ObjectInfoPage : Page
    {
        private СтроительствоEntities context = new СтроительствоEntities();

        public ObjectInfoPage()
        {
            InitializeComponent();
            cmbObjects.ItemsSource = context.Строительные_Объекты.ToList();
        }

        private void ButtonCosts_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            var window = (MainWindow)mainWindow;
            window.MainFrame.Navigate(new CostsPage());
        }

        private void cmbObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbObjects.SelectedItem is Строительные_Объекты selectedObject)
            {
                txtStartDate.Text = selectedObject.Дата_Начала?.ToShortDateString() ?? "Не указана";
                txtEndDate.Text = selectedObject.Дата_Окончания?.ToShortDateString() ?? "Не указана";

                var status = context.Работа_На_Объекте
                    .Where(w => w.id_Объекта == selectedObject.id)
                    .Select(w => w.Статус)
                    .FirstOrDefault();

                txtStatus.Text = status ?? "Не указан";

                // Материалы
                var materials = context.Распределение_Материалов_На_Объект
                    .Where(m => m.id_Объекта == selectedObject.id)
                    .Select(m => new
                    {
                        Название = m.Материалы.Название,
                        m.Количество,
                        m.Стоимость_Материалов
                    })
                    .ToList();

                MaterialsList.Items.Clear();
                foreach (var mat in materials)
                {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) }); // Материал
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Название материала
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) }); // Кол-во
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // число
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Стоимость
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // сумма

                    grid.Children.Add(new TextBlock
                    {
                        Text = "Материал:",
                        FontWeight = FontWeights.Bold,
                        FontSize = 15
                    });
                    Grid.SetColumn(grid.Children[0], 0);

                    var nameScrollViewer = new ScrollViewer
                    {
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        Content = new TextBlock
                        {
                            Text = mat.Название,
                            FontSize = 14
                        }
                    };
                    Grid.SetColumn(nameScrollViewer, 1);
                    grid.Children.Add(nameScrollViewer);


                    grid.Children.Add(new TextBlock
                    {
                        Text = "Кол-во:",
                        FontWeight = FontWeights.Bold,
                        FontSize = 15
                    });
                    Grid.SetColumn(grid.Children[2], 2);

                    grid.Children.Add(new TextBlock
                    {
                        Text = mat.Количество.ToString(),
                        FontSize = 14
                    });
                    Grid.SetColumn(grid.Children[3], 3);

                    grid.Children.Add(new TextBlock
                    {
                        Text = "Стоимость:",
                        FontWeight = FontWeights.Bold,
                        FontSize = 15
                    });
                    Grid.SetColumn(grid.Children[4], 4);

                    grid.Children.Add(new TextBlock
                    {
                        Text = $"{mat.Стоимость_Материалов:C}",
                        FontSize = 14
                    });
                    Grid.SetColumn(grid.Children[5], 5);

                    MaterialsList.Items.Add(grid);
                }

                // Затраты на оборудование
                var equipmentCosts = context.Затраты_На_Оборудование
                    .Where(eqc => eqc.id_Объекта == selectedObject.id)
                    .Select(eqc => new
                    {
                        Название = eqc.Оборудование.Название,
                        eqc.Затраты
                    })
                    .ToList();

                EquipmentCostsList.Items.Clear();
                foreach (var eq in equipmentCosts)
                {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Оборудование
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // Название
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) }); // Затраты
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Сумма затрат

                    grid.Children.Add(new TextBlock
                    {
                        Text = "Оборудование:",
                        FontWeight = FontWeights.Bold,
                        FontSize = 15
                    });
                    Grid.SetColumn(grid.Children[0], 0);

                    grid.Children.Add(new TextBlock
                    {
                        Text = eq.Название,
                        FontSize = 14
                    });
                    Grid.SetColumn(grid.Children[1], 1);

                    grid.Children.Add(new TextBlock
                    {
                        Text = "Затраты:",
                        FontWeight = FontWeights.Bold,
                        FontSize = 15
                    });
                    Grid.SetColumn(grid.Children[2], 2);

                    grid.Children.Add(new TextBlock
                    {
                        Text = $"{eq.Затраты:C}",
                        FontSize = 14
                    });
                    Grid.SetColumn(grid.Children[3], 3);

                    EquipmentCostsList.Items.Add(grid);
                }

                // Затраты на сотрудников
                var employeeCosts = context.Работа_На_Объекте
                    .Where(w => w.id_Объекта == selectedObject.id)
                    .Join(context.Заработная_Плата_Сотрудников,
                        w => w.id_Сотрудника,
                        z => z.id_Сотрудника,
                        (w, z) => new
                        {
                            ФИО = w.Сотрудники.ФИО,
                            z.Затраты
                        })
                    .ToList();

                EmployeeCostsList.Items.Clear();
                if (employeeCosts.Any())
                {
                    foreach (var cost in employeeCosts)
                    {
                        var grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Сотрудник
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) }); // ФИО
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Затраты
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Сумма затрат

                        grid.Children.Add(new TextBlock
                        {
                            Text = "Сотрудник:",
                            FontWeight = FontWeights.Bold,
                            FontSize = 15
                        });
                        Grid.SetColumn(grid.Children[0], 0);

                        grid.Children.Add(new TextBlock
                        {
                            Text = cost.ФИО,
                            FontSize = 14
                        });
                        Grid.SetColumn(grid.Children[1], 1);

                        grid.Children.Add(new TextBlock
                        {
                            Text = "Затраты:",
                            FontWeight = FontWeights.Bold,
                            FontSize = 15
                        });
                        Grid.SetColumn(grid.Children[2], 2);

                        grid.Children.Add(new TextBlock
                        {
                            Text = $"{cost.Затраты:C}",
                            FontSize = 14
                        });
                        Grid.SetColumn(grid.Children[3], 3);

                        EmployeeCostsList.Items.Add(grid);
                    }
                }
                else
                {
                    EmployeeCostsList.Items.Add(new TextBlock
                    {
                        Text = "Нет данных о затратах на сотрудников",
                        FontSize = 14
                    });
                }

                // Расчёт прибыли
                var profitData = context.Строительные_Объекты
                    .Where(o => o.id == selectedObject.id)
                    .Select(d => new
                    {
                        d.Выделенный_Бюджет,

                        ЗатратыНаОборудование = context.Затраты_На_Оборудование
                            .Where(z => z.id_Объекта == d.id)
                            .Sum(z => (decimal?)z.Затраты) ?? 0,

                        ЗатратыНаЗарплату = context.Работа_На_Объекте
                            .Where(r => r.id_Объекта == d.id)
                            .Select(r => context.Заработная_Плата_Сотрудников
                                .Where(z => z.id_Сотрудника == r.id_Сотрудника)
                                .Select(z => (decimal?)z.Затраты)
                                .FirstOrDefault() ?? 0)
                            .Sum(),

                        ЗатратыНаМатериалы = context.Распределение_Материалов_На_Объект
                            .Where(m => m.id_Объекта == d.id)
                            .Sum(m => (decimal?)m.Стоимость_Материалов) ?? 0
                    })
                    .FirstOrDefault();

                if (profitData != null)
                {
                    var profit = profitData.Выделенный_Бюджет - (profitData.ЗатратыНаОборудование + profitData.ЗатратыНаЗарплату + profitData.ЗатратыНаМатериалы);
                    txtProfit.Text = $"Прибыль: {profit:C}";
                    txtProfit.Visibility = Visibility.Visible;

                    if (profit >= 0)
                    {
                        txtProfit.Text = $"Прибыль: {profit:C}";
                        txtProfit.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        txtProfit.Text = $"Убыток: {profit:C}";
                        txtProfit.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    txtProfit.Visibility = Visibility.Collapsed;
                }

            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var culture = new CultureInfo("ru-RU");

            if (!(cmbObjects.SelectedItem is Строительные_Объекты selectedObject))
            {
                MessageBox.Show("Выберите строительный объект перед экспортом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Сохранить отчет",
                FileName = $"{selectedObject.Название}_отчет.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            string filePath = saveFileDialog.FileName;

            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Add();
            Excel.Worksheet sheet = (Excel.Worksheet)workbook.Sheets[1];

            int row = 1;

            // Основная информация об объекте
            sheet.Cells[row, 1] = "Название объекта";
            sheet.Cells[row, 2] = selectedObject.Название;
            row++;
            sheet.Cells[row, 1] = "Дата начала";
            sheet.Cells[row, 2] = selectedObject.Дата_Начала?.ToShortDateString() ?? "Не указана";
            row++;
            sheet.Cells[row, 1] = "Дата окончания";
            sheet.Cells[row, 2] = selectedObject.Дата_Окончания?.ToShortDateString() ?? "Не указана";
            row++;

            // Статус объекта
            var status = context.Работа_На_Объекте
                .Where(w => w.id_Объекта == selectedObject.id)
                .Select(w => w.Статус)
                .FirstOrDefault();

            sheet.Cells[row, 1] = "Статус объекта";
            sheet.Cells[row, 2] = status ?? "Не указан";
            row += 2;

            // Материалы
            sheet.Cells[row, 1] = "Материалы на объекте";
            row++;
            sheet.Cells[row, 1] = "Материал";
            sheet.Cells[row, 2] = "Количество";
            sheet.Cells[row, 3] = "Стоимость материалов";
            row++;

            var materials = context.Распределение_Материалов_На_Объект
                .Where(m => m.id_Объекта == selectedObject.id)
                .Select(m => new
                {
                    Название = m.Материалы.Название,
                    m.Количество,
                    m.Стоимость_Материалов
                })
                .ToList();

            if (materials.Any())
            {
                foreach (var mat in materials)
                {
                    sheet.Cells[row, 1] = mat.Название;
                    sheet.Cells[row, 2] = mat.Количество;
                    sheet.Cells[row, 3] = mat.Стоимость_Материалов;
                    sheet.Cells[row, 3] = string.Format(culture, "{0:N2} ₽", mat.Стоимость_Материалов);
                    row++;
                }
            }
            else
            {
                sheet.Cells[row, 1] = "Нет материалов";
                row++;
            }

            row += 2;

            // Затраты на оборудование
            sheet.Cells[row, 1] = "Затраты на оборудование";
            row++;
            sheet.Cells[row, 1] = "Оборудование";
            sheet.Cells[row, 2] = "Затраты";
            row++;

            var equipmentCosts = context.Затраты_На_Оборудование
                .Where(eq => eq.id_Объекта == selectedObject.id)
                .Select(eq => new
                {
                    Название = eq.Оборудование.Название,
                    eq.Затраты
                })
                .ToList();

            if (equipmentCosts.Any())
            {
                foreach (var eq in equipmentCosts)
                {
                    sheet.Cells[row, 1] = eq.Название;
                    sheet.Cells[row, 2] = eq.Затраты;
                    sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", eq.Затраты);
                    row++;
                }
            }
            else
            {
                sheet.Cells[row, 1] = "Нет затрат на оборудование";
                row++;
            }

            row += 2;

            // Затраты на сотрудников
            sheet.Cells[row, 1] = "Затраты на сотрудников";
            row++;
            sheet.Cells[row, 1] = "Сотрудник";
            sheet.Cells[row, 2] = "Затраты";
            row++;

            var employeeCosts = context.Работа_На_Объекте
                .Where(w => w.id_Объекта == selectedObject.id)
                .Join(context.Заработная_Плата_Сотрудников,
                    w => w.id_Сотрудника,
                    z => z.id_Сотрудника,
                    (w, z) => new
                    {
                        ФИО = w.Сотрудники.ФИО,
                        z.Затраты
                    })
                .ToList();

            if (employeeCosts.Any())
            {
                foreach (var cost in employeeCosts)
                {
                    sheet.Cells[row, 1] = cost.ФИО;
                    sheet.Cells[row, 2] = cost.Затраты;
                    sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", cost.Затраты);
                    row++;
                }
            }
            else
            {
                sheet.Cells[row, 1] = "Нет данных о затратах на сотрудников";
                row++;
            }

            row += 2;

            // Расчёт прибыли
            sheet.Cells[row, 1] = "Финансовый итог";
            row++;
            sheet.Cells[row, 1] = "Выделенный бюджет";
            sheet.Cells[row, 2] = selectedObject.Выделенный_Бюджет;
            sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", selectedObject.Выделенный_Бюджет);
            row++;

            var profitData = context.Строительные_Объекты
                .Where(o => o.id == selectedObject.id)
                .Select(d => new
                {
                    d.Выделенный_Бюджет,

                    ЗатратыНаОборудование = context.Затраты_На_Оборудование
                        .Where(z => z.id_Объекта == d.id)
                        .Sum(z => (decimal?)z.Затраты) ?? 0,

                    ЗатратыНаЗарплату = context.Работа_На_Объекте
                        .Where(r => r.id_Объекта == d.id)
                        .Select(r => context.Заработная_Плата_Сотрудников
                            .Where(z => z.id_Сотрудника == r.id_Сотрудника)
                            .Select(z => (decimal?)z.Затраты)
                            .FirstOrDefault() ?? 0)
                        .Sum(),

                    ЗатратыНаМатериалы = context.Распределение_Материалов_На_Объект
                        .Where(m => m.id_Объекта == d.id)
                        .Sum(m => (decimal?)m.Стоимость_Материалов) ?? 0
                })
                .FirstOrDefault();

            if (profitData != null)
            {
                sheet.Cells[row, 1] = "Затраты на оборудование";
                sheet.Cells[row, 2] = profitData.ЗатратыНаОборудование;
                sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", profitData.ЗатратыНаОборудование);
                row++;

                sheet.Cells[row, 1] = "Затраты на зарплату";
                sheet.Cells[row, 2] = profitData.ЗатратыНаЗарплату;
                sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", profitData.ЗатратыНаЗарплату);
                row++;

                sheet.Cells[row, 1] = "Затраты на материалы";
                sheet.Cells[row, 2] = profitData.ЗатратыНаМатериалы;
                sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", profitData.ЗатратыНаМатериалы);
                row++;

                var profit = profitData.Выделенный_Бюджет - (profitData.ЗатратыНаОборудование + profitData.ЗатратыНаЗарплату + profitData.ЗатратыНаМатериалы);

                sheet.Cells[row, 1] = profit >= 0 ? "Прибыль" : "Убыток";
                sheet.Cells[row, 2] = string.Format(culture, "{0:N2} ₽", profit);
                row++;
            }
            else
            {
                sheet.Cells[row, 1] = "Нет данных для расчета прибыли";
                row++;
            }

            // Авто-ширина столбцов
            sheet.Columns.AutoFit();

            try
            {
                workbook.SaveAs(filePath);
                MessageBox.Show($"Файл успешно сохранен:\n{filePath}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                workbook.Close(false);
                excel.Quit();
            }
        }
    }
}
