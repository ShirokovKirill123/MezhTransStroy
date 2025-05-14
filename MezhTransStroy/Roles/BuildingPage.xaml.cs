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

namespace MezhTransStroy.Roles
{
    /// <summary>
    /// Логика взаимодействия для BuildingPage.xaml
    /// </summary>
    public partial class BuildingPage : Page
    {
        private string currentTable;

        public BuildingPage()
        {
            InitializeComponent();

            this.DataContext = this;
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

        private void Button_Employees_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var отделы = context.Отделы.ToList();

                var EmployeesList = context.Сотрудники
                    .Include(lfa => lfa.Отделы)
                   .ToList();
                DataGrid.ItemsSource = EmployeesList;
                currentTable = "Сотрудники";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ФИО", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("ФИО") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Должность", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Должность") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Квалификация", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Квалификация") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата приёма", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Приёма") { StringFormat = "dd.MM.yyyy" } });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Контакты", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Контакты") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id отдела", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Отдела") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Отделы",
                    ItemsSource = отделы,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Отдела")
                });
                DataGrid.Columns[1].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                DataGrid.Columns[2].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                DataGrid.Columns[3].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
                DataGrid.Columns[7].Width = new DataGridLength(1.5, DataGridLengthUnitType.Star);
                StackPanelVisibility();
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

        private void Button_Work_On_Object_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var объект = context.Строительные_Объекты.ToList();
                var сотрудник = context.Сотрудники.ToList();
                var статусы = new List<string> { "Не начат", "В работе", "Завершён", "Отложен", "Отменён" };


                var Work_On_ObjectList = context.Работа_На_Объекте
                .Include(emp => emp.Сотрудники)
                .Include(emp => emp.Строительные_Объекты)
                .ToList();

                DataGrid.ItemsSource = Work_On_ObjectList;
                currentTable = "Работа_На_Объекте";

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

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id сотрудника", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Сотрудника") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Сотрудник",
                    ItemsSource = сотрудник,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ФИО",
                    SelectedValueBinding = new Binding("id_Сотрудника")
                });

                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата назначения", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Дата_Назначения") { StringFormat = "dd.MM.yyyy" } });

                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Статус",
                    ItemsSource = статусы,
                    SelectedItemBinding = new Binding("Статус"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });

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

        private void Button_Salary_Wage_Employees_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;

            using (var context = new СтроительствоEntities())
            {
                var сотрудники = context.Сотрудники.ToList();

                var Salary_WageList = context.Заработная_Плата_Сотрудников
                .Include(emp => emp.Сотрудники)
                .ToList();

                DataGrid.ItemsSource = Salary_WageList;
                currentTable = "Заработная_Плата_Сотрудников";

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "id сотрудника", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("id_Сотрудника") });
                DataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Сотрудник",
                    ItemsSource = сотрудники,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ФИО",
                    SelectedValueBinding = new Binding("id_Сотрудника"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Ставка в день", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Ставка_в_День") });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Отработано дней", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Отработано_Дней") });
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
                    case "Сотрудники":
                        var newEmployees = new Сотрудники
                        {
                            ФИО = "",
                            Должность = "",
                            Квалификация = "",
                            Дата_Приёма = DateTime.Now.Date,
                            Контакты = "",
                            id_Отдела = 1
                        };
                        context.Сотрудники.Add(newEmployees);
                        break;

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
                            Стоимость_в_Час = 0
                        };
                        context.Затраты_На_Оборудование.Add(newEquipment_Сosts);
                        break;

                    case "Заработная_Плата_Сотрудников":
                        var newSalary_Wage = new Заработная_Плата_Сотрудников
                        {
                            id_Сотрудника = 1,
                            Ставка_в_День = 0,
                            Отработано_Дней = 0
                        };
                        context.Заработная_Плата_Сотрудников.Add(newSalary_Wage);
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
                if (currentTable == "Сотрудники")
                {
                    var EmployeesFromGrid = DataGrid.ItemsSource as List<Сотрудники>;

                    if (EmployeesFromGrid != null)
                    {
                        foreach (var Employees in EmployeesFromGrid)
                        {
                            if (Employees.id == 0)
                            {
                                context.Сотрудники.Add(Employees);
                            }
                            else
                            {
                                var existingEmployees = context.Сотрудники.Find(Employees.id);
                                if (existingEmployees != null)
                                {
                                    context.Entry(existingEmployees).CurrentValues.SetValues(Employees);
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

                else if (currentTable == "Заработная_Плата_Сотрудников")
                {
                    var Salary_WageFromGrid = DataGrid.ItemsSource as List<Заработная_Плата_Сотрудников>;

                    if (Salary_WageFromGrid != null)
                    {
                        foreach (var Salary_Wage in Salary_WageFromGrid)
                        {
                            if (Salary_Wage.id == 0)
                            {
                                context.Заработная_Плата_Сотрудников.Add(Salary_Wage);
                            }
                            else
                            {
                                var existingSalary_Wage = context.Заработная_Плата_Сотрудников.Find(Salary_Wage.id);
                                if (existingSalary_Wage != null)
                                {
                                    context.Entry(existingSalary_Wage).CurrentValues.SetValues(Salary_Wage);
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
                    case "Сотрудники":
                        itemToDelete = context.Сотрудники.FirstOrDefault(i => i.id == deleteID);
                        break;

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

                    case "Распределение_Материалов_На_Объект":
                        itemToDelete = context.Распределение_Материалов_На_Объект.FirstOrDefault(si => si.id == deleteID);
                        break;

                    case "Работа_На_Объекте":
                        itemToDelete = context.Работа_На_Объекте.FirstOrDefault(si => si.id == deleteID);
                        break;

                    case "Затраты_На_Оборудование":
                        itemToDelete = context.Затраты_На_Оборудование.FirstOrDefault(equip => equip.id == deleteID);
                        break;

                    case "Заработная_Плата_Сотрудников":
                        itemToDelete = context.Заработная_Плата_Сотрудников.FirstOrDefault(sw => sw.id == deleteID);
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
                case "Сотрудники":
                    Button_Employees_Click(null, null);
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
                case "Поставщики":
                    Button_Suppliers_Click(null, null);
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
                case "Заработная_Плата_Сотрудников":
                    Button_Salary_Wage_Employees_Click(null, null);
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
                        if (item is Сотрудники сотрудники)
                        {
                            matchesId = сотрудники.id == id;
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
                        else if (item is Поставщики поставщики)
                        {
                            matchesId = поставщики.id == id;
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
                        else if (item is Заработная_Плата_Сотрудников заработная_Плата_Сотрудников)
                        {
                            matchesId = заработная_Плата_Сотрудников.id_Сотрудника == id;
                        }
                    }

                    bool matchesName = true;
                    if (!string.IsNullOrWhiteSpace(nameFilter))
                    {
                        matchesName = false;

                        // Фильтрация по имени для связей в таблицах                      
                        if (item is Сотрудники сотрудники)
                        {
                            bool matchessection = true;
                            if (сотрудники.Отделы != null && сотрудники.Отделы.Название != null)
                            {
                                matchessection = сотрудники.Отделы.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            bool matchesemp = true;
                            if (item is Сотрудники сотр)
                            {
                                matchesemp = сотрудники.Должность.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          сотрудники.Квалификация.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          сотрудники.ФИО.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchessection || matchesemp;
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
                            matchesName = оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Поставщики поставщики)
                        {
                            matchesName = поставщики.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          поставщики.Контактное_Лицо.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          поставщики.Телефон.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
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
                        else if (item is Заработная_Плата_Сотрудников заработная_Плата_Сотрудников)
                        {
                            bool matchesEmployees = true;
                            if (заработная_Плата_Сотрудников.Сотрудники != null && заработная_Плата_Сотрудников.Сотрудники.ФИО != null)
                            {
                                matchesEmployees = заработная_Плата_Сотрудников.Сотрудники.ФИО.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                            }

                            matchesName = matchesEmployees;
                        }
                        // Фильтрация по обычным строковым свойствам
                        if (!matchesName)
                        {
                            var nameProperties = new[] { "ФИО", "Название"};
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
