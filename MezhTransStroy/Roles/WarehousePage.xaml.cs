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
using System.Text.Json;
using System.Globalization;
using Newtonsoft.Json;

using System.Data.Entity; // Для Entity Framework 

namespace MezhTransStroy.Roles
{
    /// <summary>
    /// Логика взаимодействия для WarehousePage.xaml
    /// </summary>
    public partial class WarehousePage : Page
    {
        private string currentTable;
        int notificationcount = 0;

        public WarehousePage()
        {
            InitializeComponent();
            NotificationManager.NotificationCountChanged += DisplayNotifications;
            NotificationManager.LoadNotificationCount();
            notificationcount = GetNotificationCount();
            DisplayNotifications();
            this.DataContext = this;
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

        private int GetNotificationCount()
        {
            string уведомленияPath = "уведомления.json";
            if (File.Exists(уведомленияPath))
            {
                string json = File.ReadAllText(уведомленияPath);
                var уведомления = JsonConvert.DeserializeObject<List<string>>(json);
                return уведомления?.Count ?? 0;
            }
            return 0;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            SideMenu.Visibility = SideMenu.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
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

        private void Button_Construction_Objects_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var Construction_ObjectsList = context.Строительные_Объекты.ToList();

                DataGrid.ItemsSource = Construction_ObjectsList;
                currentTable = "Строительные_Объекты";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Адрес") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата начала", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Начала") { StringFormat = "dd.MM.yyyy" } });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата окончания", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Окончания") { StringFormat = "dd.MM.yyyy" } });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Выделенный бюджет", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Выделенный_Бюджет") });


                StackPanelVisibility();
            }
        }

        private void Button_Materials_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var MaterialsList = context.Материалы.ToList();

                DataGrid.ItemsSource = MaterialsList;
                currentTable = "Материалы";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Единица измерения", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Единица_Измерения") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Стоимость") });

                StackPanelVisibility();
            }
        }

        private void Button_Warehouse_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var WarehouseList = context.Склады.ToList();

                DataGrid.ItemsSource = WarehouseList;
                currentTable = "Склады";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Номер склада", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Номер_Склада") });

                StackPanelVisibility();
            }
        }

        private void Button_Equipment_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var EquipmentList = context.Оборудование.ToList();

                DataGrid.ItemsSource = EquipmentList;
                currentTable = "Оборудование";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Тип", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Тип") });

                StackPanelVisibility();
            }
        }

        private void Button_Suppliers_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                DataGrid.ContextMenu = null;

                var SuppliersList = context.Поставщики.ToList();

                DataGrid.ItemsSource = SuppliersList;
                currentTable = "Поставщики";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Контактное лицо", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Контактное_Лицо") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Телефон", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Телефон") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Адрес") });

                StackPanelVisibility();
            }
        }

        private void Button_Materials_In_Stock_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var материал = context.Материалы.ToList();
                var поставщик = context.Поставщики.ToList();
                var склад = context.Склады.ToList();

                var WarehouseList = context.Материалы_На_Складах
                  .Include(la => la.Материалы)
                  .Include(la => la.Склады)
                  .Include(la => la.Поставщики)
                 .ToList();

                foreach (var item in WarehouseList)
                {
                    var материалItem = материал.FirstOrDefault(m => m.id == item.id_Материала);
                    if (материалItem != null)
                    {
                        item.Стоимость_Материалов = item.Количество * материалItem.Стоимость;
                    }
                }

                DataGrid.ItemsSource = WarehouseList;
                currentTable = "Материалы_На_Складах";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("id") });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Склад",
                    ItemsSource = склад,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "id",
                    SelectedValueBinding = new Binding("id_Склада")
                });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id материала", Binding = new Binding("id_Материала") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Материал",
                    ItemsSource = материал,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Материала"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Количество") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Стоимость_Материалов") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id поставщика", Binding = new Binding("id_Поставщика") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Поставщик",
                    ItemsSource = поставщик,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Поставщика"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" } });

                StackPanelVisibility();
            }
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
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id объекта", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Объекта") });

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
                    SelectedValueBinding = new Binding("id_Склада")
                });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id материала", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Материала") });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Материал",
                    ItemsSource = материал,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Материала")
                });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Количество") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость материалов", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Стоимость_Материалов") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата передачи", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Передачи") { StringFormat = "dd.MM.yyyy" } });

                StackPanelVisibility();
            }
        }        

        private void Button_Equipment_Сosts_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var объект = context.Строительные_Объекты.ToList();
                var оборудование = context.Оборудование.ToList();

                var Equipment_СostsList = context.Затраты_На_Оборудование
                .Include(emp => emp.Оборудование)
                .Include(emp => emp.Строительные_Объекты)
                .ToList();

                DataGrid.ItemsSource = Equipment_СostsList;
                currentTable = "Затраты_На_Оборудование";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id объекта", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Объекта") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Объект",
                    ItemsSource = объект,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Объекта")
                });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id оборудования", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Оборудования") });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Оборудование",
                    ItemsSource = оборудование,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Оборудования")
                });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Часы работы", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Часы_Работы") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Стоимость в час", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Стоимость_в_Час") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Затраты", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Затраты") });
                StackPanelVisibility();
            }
        }       

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                switch (currentTable)
                {                                    
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
                            Тип = ""
                        };
                        context.Оборудование.Add(newEquipment);
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
                            Дата_Передачи = DateTime.Now.Date
                        };
                        context.Распределение_Материалов_На_Объект.Add(newAllocation_of_Materials_to_the_Object);
                        break;
                 
                    case "Затраты_На_Оборудование":
                        var newEquipment_Сosts = new Затраты_На_Оборудование
                        {
                            id_Объекта = 1,
                            id_Оборудования = 1,
                            Часы_Работы = 0,
                            Стоимость_в_Час = 0
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
                if (currentTable == "Строительные_Объекты")
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
                MessageBox.Show("Введите ID удаляемого элемента:");
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
                    case "Строительные_Объекты":
                        itemToDelete = context.Строительные_Объекты.FirstOrDefault(s => s.id == deleteID);
                        break;

                    case "Материалы":
                        itemToDelete = context.Материалы.FirstOrDefault(o => o.id == deleteID);
                        break;

                    case "Оборудование":
                        itemToDelete = context.Оборудование.FirstOrDefault(eq => eq.id == deleteID);
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

                context.SaveChanges();

                MessageBox.Show($"Запись с ID {deleteID} была успешно удалена.");
                RefreshDataGrid();
            }
        }

        private void RefreshDataGrid()
        {
            switch (currentTable)
            {                
                case "Строительные_Объекты":
                    Button_Construction_Objects_Click(null, null);
                    break;
                case "Материалы":
                    Button_Materials_Click(null, null);
                    break;
                case "Оборудование":
                    Button_Equipment_Click(null, null);
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
                        if (item is Строительные_Объекты cтроительные_Объекты)
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
                        if (item is Строительные_Объекты строительные_Объекты)
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
                            matchesName = оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
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
