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

using System.Data.Entity; // Для Entity Framework 
using System.Data.Entity.Infrastructure;
using MezhTransStroy.Database;
using System.Windows.Media.Animation;

namespace MezhTransStroy.Roles
{
    /// <summary>
    /// Логика взаимодействия для BuildingPage.xaml
    /// </summary>
    public partial class BuildingPage : Page
    {
        private string currentTable;
        private DataGridLoader _loader;

        public BuildingPage()
        {
            InitializeComponent();
            this.DataContext = this;
            _loader = new DataGridLoader(DataGrid);          
        }

        private void SetTableName(string tableName)
        {
            TableNameTextBlock.Text = tableName;
            TableNameTextBlock.Visibility = Visibility.Visible;
        }

        private void SetTableNameFromButton(object sender)
        {
            if (sender is Button button)
            {
                if (button.Content is string contentText)
                {
                    SetTableName(contentText);
                }
                else
                {
                    TableNameTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideMenu.Visibility != Visibility.Visible)
            {
                SideMenu.Visibility = Visibility.Visible;
                var showStoryboard = (Storyboard)FindResource("ShowMenuStoryboard");
                showStoryboard.Begin();
            }
            else
            {
                var hideStoryboard = (Storyboard)FindResource("HideMenuStoryboard");
                hideStoryboard.Completed += (s, ev) =>
                {
                    SideMenu.Visibility = Visibility.Collapsed;
                };
                hideStoryboard.Begin();
            }
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SideMenu.Visibility == Visibility.Visible)
            {
                Point clickPosition = e.GetPosition(this);

                if (!IsPointInsideElement(SideMenu, clickPosition) && !IsPointInsideElement(MenuButton, clickPosition))
                {
                    SideMenu.Visibility = Visibility.Hidden;
                }
            }
        }

        private bool IsPointInsideElement(FrameworkElement element, Point point)
        {
            Rect bounds = new Rect(element.TranslatePoint(new Point(0, 0), this), element.RenderSize);

            return bounds.Contains(point);
        }

        private void StackPanelVisibility()
        {
            if (DataGrid.Items.Count == 0)
            {
                StackPanel2.Visibility = Visibility.Hidden;
            }
            else
            {
                StackPanel2.Visibility = Visibility.Visible;
            }
        }

        private void Button_Sections_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadSections();
            currentTable = "Отделы";
            StackPanelVisibility();
        }

        private void Button_Construction_Objects_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadConstructionObjects();
            currentTable = "Строительные_Объекты";
            StackPanelVisibility();
        }

        private void Button_Materials_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadMaterials();
            currentTable = "Материалы";
            StackPanelVisibility();
        }

        private void Button_Equipment_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadEquipment();
            currentTable = "Оборудование";
            StackPanelVisibility();
        }

        private void Distribution_Of_Equipment_To_The_Site_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);

            using (var context = new СтроительствоEntities())
            {
                var склады = context.Склады.ToList();
                var объекты = context.Строительные_Объекты.ToList();
                var оборудование = context.Оборудование.ToList();

                var distributionList = context.Распределение_Оборудования_На_Объект
                    .Include(r => r.Склады)
                    .Include(r => r.Строительные_Объекты)
                    .Include(r => r.Оборудование)
                    .ToList();

                DataGrid.ItemsSource = distributionList;
                currentTable = "Распределение_Оборудования_На_Объект";

                DataGrid.Columns.Clear();

                var textStyle = new Style(typeof(TextBlock));
                textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Binding = new Binding("id"),
                    ElementStyle = textStyle,
                    Width = new DataGridLength(0.4, DataGridLengthUnitType.Star),
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Склад",
                    ItemsSource = склады,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "id",
                    SelectedValueBinding = new Binding("id_Склада"),
                    ElementStyle = new Style(typeof(ComboBox))
                    {
                        Setters = { new Setter(ComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    },
                    Width = new DataGridLength(0.4, DataGridLengthUnitType.Star),
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Объект",
                    ItemsSource = объекты,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Объекта"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Оборудование",
                    ItemsSource = оборудование,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Оборудования"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });

                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата передачи",
                    Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = textStyle,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });

                // Создание стиля кнопки
                var buttonStyle = new Style(typeof(Button));
                buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CD853F"))));
                buttonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
                buttonStyle.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.SemiBold));
                buttonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 6, 10, 6)));
                buttonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(4, 2, 4, 2)));
                buttonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
                buttonStyle.Setters.Add(new Setter(Button.CursorProperty, Cursors.Hand));
                buttonStyle.Setters.Add(new Setter(Button.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
                buttonStyle.Setters.Add(new Setter(Button.VerticalAlignmentProperty, VerticalAlignment.Stretch));

                var template = new ControlTemplate(typeof(Button));
                var borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
                borderFactory.SetBinding(Border.BackgroundProperty, new Binding("Background")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
                });

                var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

                borderFactory.AppendChild(contentFactory);
                template.VisualTree = borderFactory;
                buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

                var hoverTrigger = new Trigger
                {
                    Property = Button.IsMouseOverProperty,
                    Value = true
                };
                hoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B87333"))));
                buttonStyle.Triggers.Add(hoverTrigger);

                // Кнопка 
                var buttonTemplate = new DataTemplate();
                var buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonFactory.SetValue(Button.ContentProperty, "Отправить обратно на склад");
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(SendToStock_Click));
                buttonFactory.SetValue(Button.StyleProperty, buttonStyle);

                buttonTemplate.VisualTree = buttonFactory;

                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn
                {
                    Header = "",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    CellTemplate = buttonTemplate
                };

                DataGrid.Columns.Add(buttonColumn);

                StackPanelVisibility();
            }
        }

        private void SendToStock_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                int employeeId = Manager.User.Employee;
                var сотрудник = context.Сотрудники
                    .Where(emp => emp.id == employeeId)
                    .Select(emp => new { emp.ФИО, emp.id_Отдела })
                    .FirstOrDefault();

                string fio = сотрудник?.ФИО ?? "Неизвестный пользователь";
                string отдел = сотрудник != null
                    ? context.Отделы.Where(o => o.id == сотрудник.id_Отдела).Select(o => o.Название).FirstOrDefault() ?? "Неизвестный отдел"
                    : "Неизвестный отдел";

                var selectedDistribution = (Распределение_Оборудования_На_Объект)DataGrid.SelectedItem;
                if (selectedDistribution == null)
                {
                    MessageBox.Show("Выберите оборудование для перенаправления обратно на склад.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var оборудование = context.Оборудование.FirstOrDefault(o => o.id == selectedDistribution.id_Оборудования);
                if (оборудование == null)
                {
                    MessageBox.Show("Оборудование не найдено в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var склад = context.Склады.FirstOrDefault(s => s.id == selectedDistribution.id_Склада);
                string номерСклада = склад != null ? склад.Номер_Склада.ToString() : "неизвестен";

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите перенаправить оборудование \"{оборудование.Название}\" обратно на склад №{номерСклада}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                var equipmentInStorage = context.Оборудование_На_Складах
                    .FirstOrDefault(eq => eq.id_Оборудования == selectedDistribution.id_Оборудования
                                         && eq.id_Склада == selectedDistribution.id_Склада);
                var distributionToDelete = context.Распределение_Оборудования_На_Объект
                    .FirstOrDefault(d => d.id == selectedDistribution.id);
                var objectName = context.Строительные_Объекты
                    .Where(o => o.id == selectedDistribution.id_Объекта)
                    .Select(o => o.Название)
                    .FirstOrDefault();

                if (distributionToDelete != null)
                {
                    equipmentInStorage.Статус = "На складе";
                    context.Распределение_Оборудования_На_Объект.Remove(distributionToDelete);
                    context.SaveChanges();

                    var newHistory = new История_Перемещений_Оборудования
                    {
                        id_Оборудования = оборудование.id,
                        id_Склада = selectedDistribution.id_Склада,
                        id_Объекта = selectedDistribution.id_Объекта,
                        Дата_Перемещения_С_Объекта_На_Склад = distributionToDelete.Дата_Передачи,
                        Описание = $"Оборудование \"{оборудование.Название}\" возвращено с объекта \"{objectName}\" на склад №{номерСклада} ({distributionToDelete.Дата_Передачи:dd.MM.yyyy HH:mm}), пользователем: {fio} ({отдел})"
                    };
                    context.История_Перемещений_Оборудования.Add(newHistory);

                }
                else
                {
                    MessageBox.Show("Оборудование на складе не найдено.");
                }

                context.SaveChanges();

                MessageBox.Show($"Оборудование \"{оборудование.Название}\" успешно перенаправлено обратно на склад.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

                Distribution_Of_Equipment_To_The_Site_Click(null, null);
            }
        }

        private void ButtonMaterialMovements_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new MaterialMovementsPage());
        }

        private void Button_Application_Materials_At_Object_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Application_Materials_At_ObjectPage());
        }

        private void Button_Allocation_of_Materials_to_the_Object_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);

            using (var context = new СтроительствоEntities())
            {
                var склады = context.Склады.ToList();
                var объект = context.Строительные_Объекты.ToList();
                var материал = context.Материалы.ToList();

                var Allocation_of_Materials_to_the_ObjectList = context.Распределение_Материалов_На_Объект
                    .Include(emp => emp.Строительные_Объекты)
                    .Include(emp => emp.Склады)
                    .Include(emp => emp.Материалы)
                    .ToList();


                foreach (var item in Allocation_of_Materials_to_the_ObjectList)
                {
                    var материалItem = материал.FirstOrDefault(m => m.id == item.id_Материала);
                    if (материалItem != null)
                    {
                        item.Стоимость_Материалов = item.Количество * материалItem.Стоимость;
                    }
                }

                DataGrid.ItemsSource = Allocation_of_Materials_to_the_ObjectList;
                currentTable = "Распределение_Материалов_На_Объект";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Объект",
                    ItemsSource = объект,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Объекта")
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Склад",
                    ItemsSource = склады,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "id",
                    SelectedValueBinding = new Binding("id_Склада"),
                    ElementStyle = new Style(typeof(ComboBox))
                    {
                        Setters = { new Setter(ComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    }
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Материал",
                    ItemsSource = материал,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Материала")
                });

                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Количество",
                    Binding = new Binding("Количество"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Стоимость материалов",
                    Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" },
                    IsReadOnly = true,
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата передачи",
                    Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                DataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Израсходовано",
                    Binding = new Binding("Израсходовано"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                foreach (var column in DataGrid.Columns)
                {
                    column.Width = DataGridLength.Auto;
                }
                if (DataGrid.Columns.Count > 0)
                    DataGrid.Columns.Last().Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                StackPanelVisibility();
            }
        }

        private void Button_Work_On_Object_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadWork_On_Object();
            currentTable = "Работа_На_Объекте";
            StackPanelVisibility();
        }

        private void Button_Equipment_Сosts_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadEquipment_Сosts();
            currentTable = "Затраты_На_Оборудование";
            StackPanelVisibility();
        }


        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                switch (currentTable)
                {
                    case "Отделы":
                        MessageBox.Show("Невозможно добавить новый отдел", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;

                    case "Строительные_Объекты":
                        var newConstruction_Objects = new Строительные_Объекты
                        {
                            Название = "",
                            Адрес = "",
                            Дата_Начала = DateTime.Now.Date,
                            Дата_Окончания = DateTime.Now.Date,
                            Выделенный_Бюджет = 0m
                        };
                        context.Строительные_Объекты.Add(newConstruction_Objects);
                        break;

                    case "Материалы":
                        var newMaterials = new Материалы
                        {
                            Название = "",
                            Единица_Измерения = "",
                            Стоимость = 0m
                        };
                        context.Материалы.Add(newMaterials);
                        break;

                    case "Оборудование":
                        var newEquipment = new Оборудование
                        {
                            Название = "",
                            Тип = "",
                            Год_Выпуска = 0,
                            Производитель = "",
                            Стоимость_в_Час = 0m
                        };
                        context.Оборудование.Add(newEquipment);
                        break;

                    case "Распределение_Оборудования_На_Объект_Click":
                        var newDistribution_Of_Equipment_To_The_Site = new Распределение_Оборудования_На_Объект
                        {
                            id_Склада = 1,
                            id_Объекта = 1,
                            id_Оборудования = 1,
                            Дата_Передачи = DateTime.Now.Date
                        };
                        context.Распределение_Оборудования_На_Объект.Add(newDistribution_Of_Equipment_To_The_Site);
                        break;

                    case "Распределение_Материалов_На_Объект":
                        var newAllocation_of_Materials_to_the_Object = new Распределение_Материалов_На_Объект
                        {
                            id_Склада = 1,
                            id_Объекта = 1,
                            id_Материала = 1,
                            Количество = 0,
                            Стоимость_Материалов = 0,
                            Дата_Передачи = DateTime.Now.Date,
                            Израсходовано = 0
                        };
                        context.Распределение_Материалов_На_Объект.Add(newAllocation_of_Materials_to_the_Object);
                        break;

                    case "Работа_На_Объекте":
                        var newWork_On_Object = new Работа_На_Объекте
                        {
                            id_Сотрудника = 1,
                            id_Объекта = 1,
                            Дата_Назначения = DateTime.Now.Date,
                            Статус = "Не начат"
                        };
                        context.Работа_На_Объекте.Add(newWork_On_Object);
                        break;

                    case "Затраты_На_Оборудование":
                        var newEquipment_Сosts = new Затраты_На_Оборудование
                        {
                            id_Объекта = 1,
                            id_Оборудования = 1,
                            Часы_Работы = 0,
                            Затраты = 0m
                        };
                        context.Затраты_На_Оборудование.Add(newEquipment_Сosts);
                        break;

                    default:
                        MessageBox.Show("Неизвестная таблица для добавления");
                        return;
                }

                context.SaveChanges();
                RefreshDataGrid();
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                if (currentTable == "Отделы")
                {
                    var SectionsFromGrid = DataGrid.ItemsSource as List<Отделы>;

                    if (SectionsFromGrid != null)
                    {
                        foreach (var Sections in SectionsFromGrid)
                        {
                            if (Sections.id == 0)
                            {
                                context.Отделы.Add(Sections);
                            }
                            else
                            {
                                var existingSections = context.Отделы.Find(Sections.id);
                                if (existingSections != null)
                                {
                                    context.Entry(existingSections).CurrentValues.SetValues(Sections);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Строительные_Объекты")
                {
                    var Construction_ObjectsFromGrid = DataGrid.ItemsSource as List<Строительные_Объекты>;

                    if (Construction_ObjectsFromGrid != null)
                    {
                        foreach (var Construction_Objects in Construction_ObjectsFromGrid)
                        {
                            if (Construction_Objects.id == 0)
                            {
                                context.Строительные_Объекты.Add(Construction_Objects);
                            }
                            else
                            {
                                var existingConstruction_Objects = context.Строительные_Объекты.Find(Construction_Objects.id);
                                if (existingConstruction_Objects != null)
                                {
                                    context.Entry(existingConstruction_Objects).CurrentValues.SetValues(Construction_Objects);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Материалы")
                {
                    var MaterialsFromGrid = DataGrid.ItemsSource as List<Материалы>;

                    if (MaterialsFromGrid != null)
                    {
                        foreach (var Materials in MaterialsFromGrid)
                        {
                            if (Materials.id == 0)
                            {
                                context.Материалы.Add(Materials);
                            }
                            else
                            {
                                var existingMaterials = context.Материалы.Find(Materials.id);
                                if (existingMaterials != null)
                                {
                                    context.Entry(existingMaterials).CurrentValues.SetValues(Materials);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Оборудование")
                {
                    var EquipmentFromGrid = DataGrid.ItemsSource as List<Оборудование>;

                    if (EquipmentFromGrid != null)
                    {
                        foreach (var Equipment in EquipmentFromGrid)
                        {
                            if (Equipment.Год_Выпуска < 1800)
                            {
                                MessageBox.Show($"Ошибка: у оборудования с ID {Equipment.id} указан недопустимый год выпуска ({Equipment.Год_Выпуска})!");
                                return;
                            }

                            if (Equipment.id == 0)
                            {
                                context.Оборудование.Add(Equipment);
                            }
                            else
                            {
                                var existingEquipment = context.Оборудование.Find(Equipment.id);
                                if (existingEquipment != null)
                                {
                                    context.Entry(existingEquipment).CurrentValues.SetValues(Equipment);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Распределение_Оборудования_На_Объект")
                {
                    var DistributionFromGrid = DataGrid.ItemsSource as List<Распределение_Оборудования_На_Объект>;

                    if (DistributionFromGrid != null)
                    {
                        foreach (var Distribution in DistributionFromGrid)
                        {
                            if (Distribution.id == 0)
                            {
                                context.Распределение_Оборудования_На_Объект.Add(Distribution);
                            }
                            else
                            {
                                var existingDistribution = context.Распределение_Оборудования_На_Объект.Find(Distribution.id);
                                if (existingDistribution != null)
                                {
                                    context.Entry(existingDistribution).CurrentValues.SetValues(Distribution);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Распределение_Материалов_На_Объект")
                {
                    var Allocation_of_Materials_to_the_ObjectFromGrid = DataGrid.ItemsSource as List<Распределение_Материалов_На_Объект>;

                    if (Allocation_of_Materials_to_the_ObjectFromGrid != null)
                    {
                        foreach (var Allocation_of_Materials_to_the_Object in Allocation_of_Materials_to_the_ObjectFromGrid)
                        {
                            if (Allocation_of_Materials_to_the_Object.id == 0)
                            {
                                context.Распределение_Материалов_На_Объект.Add(Allocation_of_Materials_to_the_Object);
                            }
                            else
                            {
                                var existingAllocation_of_Materials_to_the_Object = context.Распределение_Материалов_На_Объект.Find(Allocation_of_Materials_to_the_Object.id);
                                if (existingAllocation_of_Materials_to_the_Object != null)
                                {
                                    context.Entry(existingAllocation_of_Materials_to_the_Object).CurrentValues.SetValues(Allocation_of_Materials_to_the_Object);
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Работа_На_Объекте")
                {
                    var Work_On_ObjectFromGrid = DataGrid.ItemsSource as List<Работа_На_Объекте>;

                    if (Work_On_ObjectFromGrid != null)
                    {
                        foreach (var Work_On_Object in Work_On_ObjectFromGrid)
                        {
                            if (Work_On_Object.id == 0)
                            {
                                context.Работа_На_Объекте.Add(Work_On_Object);
                            }
                            else
                            {
                                var existingWork_On_Object = context.Работа_На_Объекте.Find(Work_On_Object.id);
                                if (existingWork_On_Object != null)
                                {
                                    context.Entry(existingWork_On_Object).CurrentValues.SetValues(Work_On_Object);
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }

                else if (currentTable == "Затраты_На_Оборудование")
                {
                    var Equipment_СostsFromGrid = DataGrid.ItemsSource as List<Затраты_На_Оборудование>;

                    if (Equipment_СostsFromGrid != null)
                    {
                        foreach (var Equipment_Сosts in Equipment_СostsFromGrid)
                        {
                            if (Equipment_Сosts.id == 0)
                            {
                                context.Затраты_На_Оборудование.Add(Equipment_Сosts);
                            }
                            else
                            {
                                var existingEquipment_Сosts = context.Затраты_На_Оборудование.Find(Equipment_Сosts.id);
                                if (existingEquipment_Сosts != null)
                                {
                                    context.Entry(existingEquipment_Сosts).CurrentValues.SetValues(Equipment_Сosts);
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            string inputID = TextBoxID.Text;

            if (string.IsNullOrEmpty(inputID))
            {
                MessageBox.Show("Введите ID удаляемого элемента в поле ID:");
                return;
            }

            int deleteID = int.Parse(inputID);

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить элемент с ID {deleteID}?", "Подтверждение удаления", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            using (var context = new СтроительствоEntities())
            {
                dynamic itemToDelete = null;

                switch (currentTable)
                {
                    case "Отделы":
                        MessageBox.Show("Невозможно удалить отдел", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;

                    case "Строительные_Объекты":
                        itemToDelete = context.Строительные_Объекты.FirstOrDefault(s => s.id == deleteID);
                        break;

                    case "Материалы":
                        itemToDelete = context.Материалы.FirstOrDefault(o => o.id == deleteID);
                        break;

                    case "Оборудование":
                        itemToDelete = context.Оборудование.FirstOrDefault(eq => eq.id == deleteID);
                        break;

                    case "Распределение_Оборудования_На_Объект":
                        itemToDelete = context.Распределение_Оборудования_На_Объект.FirstOrDefault(eq => eq.id == deleteID);
                        break;

                    case "Распределение_Материалов_На_Объект":
                        itemToDelete = context.Распределение_Материалов_На_Объект.FirstOrDefault(si => si.id == deleteID);
                        break;

                    case "Работа_На_Объекте":
                        itemToDelete = context.Работа_На_Объекте.FirstOrDefault(si => si.id == deleteID);
                        break;

                    case "Затраты_На_Оборудование":
                        itemToDelete = context.Затраты_На_Оборудование.FirstOrDefault(equip => equip.id == deleteID);
                        break;

                    default:
                        MessageBox.Show("Неизвестная таблица.");
                        return;
                }

                if (itemToDelete == null)
                {
                    MessageBox.Show($"Запись с ID {deleteID} не найдена.");
                    return;
                }

                context.Set(itemToDelete.GetType()).Remove(itemToDelete);

                try
                {
                    context.SaveChanges();
                    MessageBox.Show($"Запись с ID {deleteID} была успешно удалена.");
                    RefreshDataGrid();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                    {
                        MessageBox.Show("Невозможно удалить запись. Она используется в других таблицах!", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RefreshDataGrid()
        {
            switch (currentTable)
            {
                case "Отделы":
                    Button_Sections_Click(null, null);
                    break;
                case "Строительные_Объекты":
                    Button_Construction_Objects_Click(null, null);
                    break;
                case "Материалы":
                    Button_Materials_Click(null, null);
                    break;
                case "Оборудование":
                    Button_Equipment_Click(null, null);
                    break;
                case "Распределение_Оборудования_На_Объект":
                    Distribution_Of_Equipment_To_The_Site_Click(null, null);
                    break;
                case "Распределение_Материалов_На_Объект":
                    Button_Allocation_of_Materials_to_the_Object_Click(null, null);
                    break;
                case "Работа_На_Объекте":
                    Button_Work_On_Object_Click(null, null);
                    break;
                case "Затраты_На_Оборудование":
                    Button_Equipment_Сosts_Click(null, null);
                    break;
                default:
                    break;
            }
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            var itemsSource = DataGrid.ItemsSource as IEnumerable<object>;

            string idFilter = TextBoxID.Text;
            string nameFilter = TextBoxName.Text;

            if (string.IsNullOrWhiteSpace(idFilter) && string.IsNullOrWhiteSpace(nameFilter))
            {
                MessageBox.Show("Невозможно выполнить фильтрацию: фильтр не задан.");
            }

            try
            {
                var filteredList = itemsSource.Where(item =>
                {
                    bool matchesId = true;
                    if (!string.IsNullOrWhiteSpace(idFilter) && int.TryParse(idFilter, out int id))
                    {
                        if (item is Отделы отделы)
                        {
                            matchesId = отделы.id == id;
                        }
                        else if (item is Строительные_Объекты cтроительные_Объекты)
                        {
                            matchesId = cтроительные_Объекты.id == id;
                        }
                        else if (item is Материалы материалы)
                        {
                            matchesId = материалы.id == id;
                        }
                        else if (item is Оборудование оборудование)
                        {
                            matchesId = оборудование.id == id;
                        }
                        else if (item is Распределение_Оборудования_На_Объект распределение_Оборудования_На_Объект)
                        {
                            matchesId = распределение_Оборудования_На_Объект.id == id;
                        }
                        else if (item is Распределение_Материалов_На_Объект распределение_Материалов_На_Объект)
                        {
                            matchesId = распределение_Материалов_На_Объект.id_Объекта == id;
                        }
                        else if (item is Работа_На_Объекте работа_На_Объекте)
                        {
                            matchesId = работа_На_Объекте.id_Объекта == id;
                        }
                        else if (item is Затраты_На_Оборудование затраты_На_Оборудование)
                        {
                            matchesId = затраты_На_Оборудование.id_Объекта == id;
                        }
                    }

                    bool matchesName = true;
                    if (!string.IsNullOrWhiteSpace(nameFilter))
                    {
                        matchesName = false;

                        // Фильтрация по имени для связей в таблицах
                        if (item is Отделы отдел)
                        {
                            matchesName = отдел.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Строительные_Объекты строительные_Объекты)
                        {
                            matchesName = строительные_Объекты.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          строительные_Объекты.Адрес.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Материалы материалы)
                        {
                            matchesName = материалы.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Оборудование оборудование)
                        {
                            matchesName = оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          оборудование.Тип.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          оборудование.Производитель.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Распределение_Оборудования_На_Объект распределение_Оборудования_На_Объект)
                        {
                            bool matchesobjects = true;
                            if (распределение_Оборудования_На_Объект.Строительные_Объекты != null && распределение_Оборудования_На_Объект.Строительные_Объекты.Название != null)
                            {
                                matchesobjects = распределение_Оборудования_На_Объект.Строительные_Объекты.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesequipment = true;
                            if (распределение_Оборудования_На_Объект.Оборудование != null && распределение_Оборудования_На_Объект.Оборудование.Название != null)
                            {
                                matchesobjects = распределение_Оборудования_На_Объект.Оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesobjects || matchesequipment;
                        }
                        else if (item is Распределение_Материалов_На_Объект распределение_Материалов_На_Объект)
                        {
                            bool matchesmaterial = true;
                            if (распределение_Материалов_На_Объект.Материалы != null && распределение_Материалов_На_Объект.Материалы.Название != null)
                            {
                                matchesmaterial = распределение_Материалов_На_Объект.Материалы.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesobjects = true;
                            if (распределение_Материалов_На_Объект.Строительные_Объекты != null && распределение_Материалов_На_Объект.Строительные_Объекты.Название != null)
                            {
                                matchesobjects = распределение_Материалов_На_Объект.Строительные_Объекты.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesmaterial || matchesobjects;
                        }
                        else if (item is Работа_На_Объекте работа_На_Объекте)
                        {
                            bool matchesobjects = true;
                            if (работа_На_Объекте.Строительные_Объекты != null && работа_На_Объекте.Строительные_Объекты.Название != null)
                            {
                                matchesobjects = работа_На_Объекте.Строительные_Объекты.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesEmployees = true;
                            if (работа_На_Объекте.Сотрудники != null && работа_На_Объекте.Сотрудники.ФИО != null)
                            {
                                matchesEmployees = работа_На_Объекте.Сотрудники.ФИО.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesobjects || matchesEmployees;
                        }
                        else if (item is Затраты_На_Оборудование затраты_На_Оборудование)
                        {
                            bool matchesobjects = true;
                            if (затраты_На_Оборудование.Строительные_Объекты != null && затраты_На_Оборудование.Строительные_Объекты.Название != null)
                            {
                                matchesobjects = затраты_На_Оборудование.Строительные_Объекты.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesEquipment = true;
                            if (затраты_На_Оборудование.Оборудование != null && затраты_На_Оборудование.Оборудование.Название != null)
                            {
                                matchesEquipment = затраты_На_Оборудование.Оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesobjects || matchesEquipment;
                        }
                        // Фильтрация по обычным строковым свойствам
                        if (!matchesName)
                        {
                            var nameProperties = new[] { "ФИО", "Название" };
                            matchesName = nameProperties.Any(propName =>
                            {
                                var property = item.GetType().GetProperty(propName);
                                if (property != null)
                                {
                                    var value = property.GetValue(item);
                                    if (value is string stringValue)
                                    {
                                        return stringValue.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                                    }
                                }
                                return false;
                            });
                        }
                    }

                    return matchesId && matchesName;
                }).ToList();

                if (filteredList.Count == 0)
                {
                    MessageBox.Show("Фильтрация не дала результатов. Попробуйте изменить фильтр");
                    return;
                }

                DataGrid.ItemsSource = filteredList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка фильтрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            var window = (MainWindow)mainWindow;
            window.MainFrame.Navigate(new ReportsPage());
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
        }
    }
}
