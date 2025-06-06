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
    /// Логика взаимодействия для WarehousePage.xaml
    /// </summary>
    public partial class WarehousePage : Page
    {
        private string currentTable;
        private DataGridLoader _loader;

        public WarehousePage()
        {
            InitializeComponent();
            NotificationManager.NotificationCountChanged += DisplayNotifications;
            NotificationManager.LoadNotificationCount();
            DisplayNotifications();
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

        private void DisplayNotifications()
        {
            NotificationCountText.Text = NotificationManager.NotificationCount.ToString();

            if (NotificationManager.NotificationCount > 0)
            {
                BtnNotifications.Visibility = Visibility.Visible;
                NotificationBadge.Visibility = Visibility.Visible;
            }
            else
            {
                NotificationCountText.Text = "0";
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

        private void Button_Warehouse_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadWarehouses();
            currentTable = "Склады";
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

        private void Equipment_In_Storage_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);

            using (var context = new СтроительствоEntities())
            {
                var склады = context.Склады.ToList();
                var оборудование = context.Оборудование.ToList();
                var статусы = new List<string> { "На складе", "На объекте" };

                var equipmentList = context.Оборудование_На_Складах
                    .Include(o => o.Склады)
                    .Include(o => o.Оборудование)
                    .ToList();

                DataGrid.ItemsSource = equipmentList;
                currentTable = "Оборудование_На_Складах";

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
                    Header = "Оборудование",
                    ItemsSource = оборудование,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Оборудования"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Статус",
                    ItemsSource = статусы,
                    SelectedItemBinding = new Binding("Статус"),
                    IsReadOnly = true,
                    ElementStyle = new Style(typeof(ComboBox))
                    {
                        Setters = { new Setter(ComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    },
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });

                // Создание стиля кнопки
                var buttonStyle = new Style(typeof(Button));
                buttonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CD853F"))));
                buttonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
                buttonStyle.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.SemiBold));
                buttonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 6, 10, 6)));
                buttonStyle.Setters.Add(new Setter(Button.MarginProperty, new Thickness(4)));
                buttonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
                buttonStyle.Setters.Add(new Setter(Button.CursorProperty, Cursors.Hand));

                var template = new ControlTemplate(typeof(Button));
                var borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
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
                buttonFactory.SetValue(Button.ContentProperty, "Отправить на объект");
                buttonFactory.SetValue(Button.WidthProperty, 160.0);
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(SendToSite_Click));
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

        private void SendToSite_Click(object sender, RoutedEventArgs e)
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

                var selectedEquipment = (Оборудование_На_Складах)DataGrid.SelectedItem;
                if (selectedEquipment == null)
                {
                    MessageBox.Show("Выберите оборудование для перенаправления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var оборудование = context.Оборудование.FirstOrDefault(o => o.id == selectedEquipment.id_Оборудования);
                if (оборудование == null)
                {
                    MessageBox.Show("Выбранное оборудование не найдено в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var existingDistribution = context.Распределение_Оборудования_На_Объект
                    .FirstOrDefault(ro => ro.id_Оборудования == selectedEquipment.id_Оборудования);

                if (existingDistribution != null)
                {
                    var objectName = context.Строительные_Объекты
                        .Where(obj => obj.id == existingDistribution.id_Объекта)
                        .Select(obj => obj.Название)
                        .FirstOrDefault();

                    MessageBox.Show($"Оборудование \"{оборудование.Название}\" уже перенаправлено на объект: {objectName}",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var selectObjectWindow = new SelectObjectWindow();
                selectObjectWindow.SetEquipmentName(оборудование.Название);

                if (selectObjectWindow.ShowDialog() == true)
                {
                    int selectedObjectId = selectObjectWindow.SelectedObjectId;

                    var newDistribution = new Распределение_Оборудования_На_Объект
                    {
                        id_Склада = selectedEquipment.id_Склада,
                        id_Объекта = selectedObjectId,
                        id_Оборудования = selectedEquipment.id_Оборудования,
                        Дата_Передачи = DateTime.Now
                    };

                    context.Распределение_Оборудования_На_Объект.Add(newDistribution);

                    var equipmentInStorage = context.Оборудование_На_Складах
                        .FirstOrDefault(p => p.id == selectedEquipment.id);
                    if (equipmentInStorage != null)
                    {
                        equipmentInStorage.Статус = "На объекте";
                    }

                    var objectName = context.Строительные_Объекты
                        .Where(o => o.id == selectedObjectId)
                        .Select(o => o.Название)
                        .FirstOrDefault();

                    var newHistory = new История_Перемещений_Оборудования
                    {
                        id_Оборудования = оборудование.id,
                        id_Склада = selectedEquipment.id_Склада,
                        id_Объекта = selectedObjectId,
                        Дата_Перемещения_На_Объект = newDistribution.Дата_Передачи,
                        Описание = $"Оборудование \"{оборудование.Название}\" перенаправлено со склада №{selectedEquipment.id_Склада} на объект \"{objectName}\" ({newDistribution.Дата_Передачи:dd.MM.yyyy HH:mm}), пользователем: {fio} ({отдел})"
                    };
                    context.История_Перемещений_Оборудования.Add(newHistory);

                    context.SaveChanges();
                    MessageBox.Show($"Оборудование \"{оборудование.Название}\" успешно перенаправлено на объект \"{objectName}\".",
                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    Equipment_In_Storage_Click(null, null);
                }
            }
        }       

        private void Button_Suppliers_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadSuppliers();
            currentTable = "Поставщики";
            StackPanelVisibility();
        }

        private void Button_Materials_In_Stock_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadMaterialsInStock();
            currentTable = "Материалы_На_Складах";
            StackPanelVisibility();
        }

        private void ButtonNotifications_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new NotificationPage());
        }

        private void ButtonMaterialMovements_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new MaterialMovementsPage());
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
                    IsReadOnly = true,
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

                    case "Оборудование_На_Складах":
                        var newEquipment_In_Storage = new Оборудование_На_Складах
                        {
                            id_Склада = 1,
                            id_Оборудования = 1,
                            Статус = "На складе"
                        };
                        context.Оборудование_На_Складах.Add(newEquipment_In_Storage);
                        break;

                    case "Поставщики":
                        var newSuppliers = new Поставщики
                        {
                            Название = "",
                            Контактное_Лицо = "",
                            Телефон = "",
                            Адрес = ""
                        };
                        context.Поставщики.Add(newSuppliers);
                        break;

                    case "Склады":
                        MessageBox.Show("Невозможно добавить новый склад", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;

                    case "Материалы_На_Складах":
                        var newMaterials_In_Stock = new Материалы_На_Складах
                        {
                            id_Склада = 1,
                            id_Материала = 1,
                            Количество = 0,
                            Стоимость_Материалов = 0,
                            id_Поставщика = 1,
                            Дата_Поступления = DateTime.Now.Date
                        };
                        context.Материалы_На_Складах.Add(newMaterials_In_Stock);
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

                else if (currentTable == "Оборудование_На_Складах")
                {
                    var Equipment_In_StorageFromGrid = DataGrid.ItemsSource as List<Оборудование_На_Складах>;

                    if (Equipment_In_StorageFromGrid != null)
                    {
                        foreach (var Equipment in Equipment_In_StorageFromGrid)
                        {
                            if (Equipment.id == 0)
                            {
                                context.Оборудование_На_Складах.Add(Equipment);
                            }
                            else
                            {
                                var existingEquipment = context.Оборудование_На_Складах.Find(Equipment.id);
                                if (existingEquipment != null)
                                {
                                    context.Entry(existingEquipment).CurrentValues.SetValues(Equipment);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }

                else if (currentTable == "Поставщики")
                {
                    var SuppliersFromGrid = DataGrid.ItemsSource as List<Поставщики>;

                    if (SuppliersFromGrid != null)
                    {
                        foreach (var Suppliers in SuppliersFromGrid)
                        {
                            if (Suppliers.id == 0)
                            {
                                context.Поставщики.Add(Suppliers);
                            }
                            else
                            {
                                var existingSuppliers = context.Поставщики.Find(Suppliers.id);
                                if (existingSuppliers != null)
                                {
                                    context.Entry(existingSuppliers).CurrentValues.SetValues(Suppliers);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Склады")
                {
                    var warehouseFromGrid = DataGrid.ItemsSource as List<Склады>;

                    if (warehouseFromGrid != null)
                    {
                        foreach (var warehouse in warehouseFromGrid)
                        {
                            if (warehouse.id == 0)
                            {
                                context.Склады.Add(warehouse);
                            }
                            else
                            {
                                var existingwarehouse = context.Склады.Find(warehouse.id);
                                if (existingwarehouse != null)
                                {
                                    context.Entry(existingwarehouse).CurrentValues.SetValues(warehouse);
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                }
                else if (currentTable == "Материалы_На_Складах")
                {
                    var Materials_In_StockFromGrid = DataGrid.ItemsSource as List<Материалы_На_Складах>;

                    if (Materials_In_StockFromGrid != null)
                    {
                        foreach (var Materials_In_Stock in Materials_In_StockFromGrid)
                        {
                            if (Materials_In_Stock.id == 0)
                            {
                                context.Материалы_На_Складах.Add(Materials_In_Stock);
                            }
                            else
                            {
                                var existingMaterials_In_Stock = context.Материалы_На_Складах.Find(Materials_In_Stock.id);
                                if (existingMaterials_In_Stock != null)
                                {
                                    context.Entry(existingMaterials_In_Stock).CurrentValues.SetValues(Materials_In_Stock);
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

                    case "Оборудование_На_Складах":
                        itemToDelete = context.Оборудование_На_Складах.FirstOrDefault(eq => eq.id == deleteID);
                        break;

                    case "Поставщики":
                        itemToDelete = context.Поставщики.FirstOrDefault(op => op.id == deleteID);
                        break;

                    case "Склады":
                        MessageBox.Show("Невозможно удалить склад", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;

                    case "Материалы_На_Складах":
                        itemToDelete = context.Материалы_На_Складах.FirstOrDefault(ms => ms.id == deleteID);
                        break;

                    case "Распределение_Материалов_На_Объект":
                        itemToDelete = context.Распределение_Материалов_На_Объект.FirstOrDefault(si => si.id == deleteID);
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
                case "Оборудование_На_Складах":
                    Equipment_In_Storage_Click(null, null);
                    break;
                case "Поставщики":
                    Button_Suppliers_Click(null, null);
                    break;
                case "Склады":
                    Button_Warehouse_Click(null, null);
                    break;
                case "Материалы_На_Складах":
                    Button_Materials_In_Stock_Click(null, null);
                    break;
                case "Распределение_Материалов_На_Объект":
                    Button_Allocation_of_Materials_to_the_Object_Click(null, null);
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
                        else if (item is Оборудование_На_Складах оборудование_На_Складах)
                        {
                            matchesId = оборудование_На_Складах.id == id;
                        }
                        else if (item is Поставщики поставщики)
                        {
                            matchesId = поставщики.id == id;
                        }
                        else if (item is Склады склады)
                        {
                            matchesId = склады.id == id;
                        }
                        else if (item is Материалы_На_Складах материалы_На_Складах)
                        {
                            matchesId = материалы_На_Складах.id == id;
                        }
                        else if (item is Распределение_Материалов_На_Объект распределение_Материалов_На_Объект)
                        {
                            matchesId = распределение_Материалов_На_Объект.id_Объекта == id;
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
                        else if (item is Оборудование_На_Складах оборудование_На_Складах)
                        {
                            bool matchesequipment = true;
                            if (оборудование_На_Складах.Оборудование != null && оборудование_На_Складах.Оборудование.Название != null)
                            {
                                matchesequipment = оборудование_На_Складах.Оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesStatus = true;
                            if (item is Оборудование_На_Складах Equipment)
                            {
                                matchesStatus = Equipment.Статус.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesequipment || matchesStatus;
                        }
                        else if (item is Поставщики поставщики)
                        {
                            matchesName = поставщики.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          поставщики.Контактное_Лицо.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          поставщики.Телефон.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Материалы_На_Складах материалы_На_Складах)
                        {
                            bool matchesmaterial = true;
                            if (материалы_На_Складах.Материалы != null && материалы_На_Складах.Материалы.Название != null)
                            {
                                matchesmaterial = материалы_На_Складах.Материалы.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesSupplier = true;
                            if (материалы_На_Складах.Поставщики != null && материалы_На_Складах.Поставщики.Название != null)
                            {
                                matchesSupplier = материалы_На_Складах.Поставщики.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesmaterial || matchesSupplier;
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
                        // Фильтрация по обычным строковым свойствам
                        if (!matchesName)
                        {
                            var nameProperties = new[] { "Название" };
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
