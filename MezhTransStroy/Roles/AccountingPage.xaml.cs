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
    /// Логика взаимодействия для AccountingPage.xaml
    /// </summary>
    public partial class AccountingPage : Page
    {
        private string currentTable;
        private DataGridLoader _loader;

        public AccountingPage()
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

        private void Button_Employees_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadEmployees();
            currentTable = "Сотрудники";
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

        private void Button_Materials_In_Stock_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadMaterialsInStock();
            currentTable = "Материалы_На_Складах";
            StackPanelVisibility();
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

                var textStyle = new Style(typeof(TextBlock));
                textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

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
                    Header = "Ед. изм.",
                    Binding = new Binding("Материалы.Единица_Измерения"),
                    IsReadOnly = true,
                    ElementStyle = textStyle,
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

        private void Button_Equipment_Сosts_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadEquipment_Сosts();
            currentTable = "Затраты_На_Оборудование";
            StackPanelVisibility();
        }        

        private void Button_Salary_Wage_Employees_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ContextMenu = null;
            SetTableNameFromButton(sender);
            _loader.LoadSalary_Wage_Employees();
            currentTable = "Заработная_Плата_Сотрудников";
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
                            Тип = "",
                            Год_Выпуска = 0,
                            Производитель = "",
                            Стоимость_в_Час = 0m
                        };
                        context.Оборудование.Add(newEquipment);
                        break;                                    

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
                            Затраты = 0m
                        };
                        context.Затраты_На_Оборудование.Add(newEquipment_Сosts);
                        break;

                    case "Заработная_Плата_Сотрудников":
                        var newSalary_Wage = new Заработная_Плата_Сотрудников
                        {
                            id_Сотрудника = 1,
                            Ставка_в_День = 0m,
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
                else if (currentTable == "Сотрудники")
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

                    case "Материалы_На_Складах":
                        itemToDelete = context.Материалы_На_Складах.FirstOrDefault(ms => ms.id == deleteID);
                        break;                    

                    case "Распределение_Материалов_На_Объект":
                        itemToDelete = context.Распределение_Материалов_На_Объект.FirstOrDefault(si => si.id == deleteID);
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
                case "Отделы":
                    Button_Sections_Click(null, null);
                    break;
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
                case "Материалы_На_Складах":
                    Button_Materials_In_Stock_Click(null, null);
                    break;                
                case "Распределение_Материалов_На_Объект":
                    Button_Allocation_of_Materials_to_the_Object_Click(null, null);
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
                        if (item is Отделы отделы)
                        {
                            matchesId = отделы.id == id;
                        }
                        else if (item is Сотрудники сотрудники)
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
                        if (item is Отделы отдел)
                        {
                            matchesName = отдел.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else if (item is Сотрудники сотрудники)
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
                            matchesName = оборудование.Название.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          оборудование.Тип.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                          оборудование.Производитель.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
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
                            var nameProperties = new[] { "ФИО", "Название", "Уровень_Доступа" };
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

        private void ButtonCosts_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this);
            var window = (MainWindow)mainWindow;
            window.MainFrame.Navigate(new ObjectInfoPage());
        }
    }
}
