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

using System.Data.Entity;
using MezhTransStroy.Database; // Для Entity Framework 

namespace MezhTransStroy
{
    public class DataGridLoader
    {
        private readonly DataGrid _dataGrid;

        public DataGridLoader(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
        }

        public void LoadSections()
        {
            using (var context = new СтроительствоEntities())
            {
                var list = context.Отделы.ToList();

                _dataGrid.ItemsSource = list;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Название отдела",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Название"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Телефон",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Телефон"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
            }
        }

        public void LoadEmployees()
        {
            using (var context = new СтроительствоEntities())
            {
                var отделы = context.Отделы.ToList();
                var employees = context.Сотрудники.Include(e => e.Отделы).ToList();

                _dataGrid.ItemsSource = employees;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Width = DataGridLength.Auto, Binding = new Binding("id") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "ФИО", Width = DataGridLength.Auto, Binding = new Binding("ФИО") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Должность", Width = DataGridLength.Auto, Binding = new Binding("Должность") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Квалификация", Width = DataGridLength.Auto, Binding = new Binding("Квалификация") });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата приёма",
                    Width = DataGridLength.Auto,
                    Binding = new Binding("Дата_Приёма") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Контакты", Width = DataGridLength.Auto, Binding = new Binding("Контакты") });
                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Отделы",
                    ItemsSource = отделы,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Отдела"),
                    Width = DataGridLength.Auto
                });               
            }
        }

        public void LoadConstructionObjects()
        {
            using (var context = new СтроительствоEntities())
            {
                var list = context.Строительные_Объекты.ToList();

                _dataGrid.ItemsSource = list;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Объект", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Адрес") });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата начала",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Дата_Начала") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата окончания",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Дата_Окончания") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Выделенный бюджет",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Выделенный_Бюджет") { StringFormat = "{0:N2} ₽" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            }
        }

        public void LoadMaterials()
        {
            using (var context = new СтроительствоEntities())
            {
                var materials = context.Материалы.ToList();

                _dataGrid.ItemsSource = materials;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Материал",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Название"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Единица измерения",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Единица_Измерения"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Стоимость",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Стоимость") { StringFormat = "{0:N2} ₽" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
            }
        }

        public void LoadWarehouses()
        {
            using (var context = new СтроительствоEntities())
            {
                var warehouses = context.Склады.ToList();

                _dataGrid.ItemsSource = warehouses;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Номер склада",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Номер_Склада"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Адрес",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Адрес")
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Телефон",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Телефон"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
            }
        }

        public void LoadEquipment()
        {
            using (var context = new СтроительствоEntities())
            {
                var equipmentList = context.Оборудование.ToList();

                _dataGrid.ItemsSource = equipmentList;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Оборудование", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Тип", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Тип") });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Год выпуска",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Год_Выпуска"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Производитель", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Производитель") });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Стоимость в час",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Стоимость_в_Час") { StringFormat = "{0:N2} ₽" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            }
        }

        public void LoadSuppliers()
        {
            using (var context = new СтроительствоEntities())
            {
                var suppliersList = context.Поставщики.ToList();

                _dataGrid.ItemsSource = suppliersList;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Название") });
                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Контактное лицо", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Контактное_Лицо") });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Телефон",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Телефон"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn { Header = "Адрес", Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding("Адрес") });
            }
        }

        public void LoadMaterialsInStock()
        {
            using (var context = new СтроительствоEntities())
            {
                var материалы = context.Материалы.ToList();
                var склады = context.Склады.ToList();
                var поставщики = context.Поставщики.ToList();

                var warehouseList = context.Материалы_На_Складах
                    .Include(m => m.Материалы)
                    .Include(m => m.Склады)
                    .Include(m => m.Поставщики)
                    .ToList();

                foreach (var item in warehouseList)
                {
                    var material = материалы.FirstOrDefault(m => m.id == item.id_Материала);
                    if (material != null)
                    {
                        item.Стоимость_Материалов = item.Количество * material.Стоимость;
                    }
                }

                _dataGrid.ItemsSource = warehouseList;
                _dataGrid.Columns.Clear();

                var textStyle = new Style(typeof(TextBlock));
                textStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                textStyle.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5, 0, 5, 0)));

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(0.3, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
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

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Материал",
                    ItemsSource = материалы,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Материала"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Количество",
                    Binding = new Binding("Количество"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Ед. изм.",
                    Binding = new Binding("Материалы.Единица_Измерения"),
                    IsReadOnly = true,
                    ElementStyle = textStyle,
                    Width = DataGridLength.Auto
                });


                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Стоимость материалов",
                    Width = new DataGridLength(1.3, DataGridLengthUnitType.Star),
                    IsReadOnly = true,
                    Binding = new Binding("Стоимость_Материалов") { StringFormat = "{0:N2} ₽" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Поставщик",
                    ItemsSource = поставщики,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Поставщика"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата поступления",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Дата_Поступления") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            }
        }        

        public void LoadWork_On_Object()
        {
            using (var context = new СтроительствоEntities())
            {
                var объект = context.Строительные_Объекты.ToList();
                var сотрудник = context.Сотрудники.ToList();
                var статусы = new List<string> { "Не начат", "В работе", "Завершён", "Отложен", "Отменён" };

                var Work_On_ObjectList = context.Работа_На_Объекте
                .Include(emp => emp.Сотрудники)
                .Include(emp => emp.Строительные_Объекты)
                .ToList();

                _dataGrid.ItemsSource = Work_On_ObjectList;

                _dataGrid.Columns.Clear();
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Объект",
                    ItemsSource = объект,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Объекта")
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Сотрудник",
                    ItemsSource = сотрудник,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ФИО",
                    SelectedValueBinding = new Binding("id_Сотрудника")
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Дата назначения",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Дата_Назначения") { StringFormat = "dd.MM.yyyy" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Статус",
                    ItemsSource = статусы,
                    SelectedItemBinding = new Binding("Статус"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    ElementStyle = new Style(typeof(ComboBox))
                    {
                        Setters = { new Setter(ComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    }
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
                _dataGrid.Columns[1].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
                _dataGrid.Columns[2].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
                _dataGrid.Columns[3].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
                _dataGrid.Columns[4].Width = new DataGridLength(0.8, DataGridLengthUnitType.Star);
            }
        }

        public void LoadEquipment_Сosts()
        {
            using (var context = new СтроительствоEntities())
            {
                var объект = context.Строительные_Объекты.ToList();
                var оборудование = context.Оборудование.ToList();

                var Equipment_СostsList = context.Затраты_На_Оборудование
                .Include(emp => emp.Оборудование)
                .Include(emp => emp.Строительные_Объекты)
                .ToList();

                foreach (var item in Equipment_СostsList)
                {
                    var equipmentItem = оборудование.FirstOrDefault(o => o.id == item.id_Оборудования);
                    if (equipmentItem != null)
                    {
                        item.Затраты = equipmentItem.Стоимость_в_Час * item.Часы_Работы;
                    }
                    else
                    {
                        item.Затраты = 0;
                    }
                }

                _dataGrid.ItemsSource = Equipment_СostsList;
                _dataGrid.Columns.Clear();
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Объект",
                    ItemsSource = объект,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Объекта")
                });

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Оборудование",
                    ItemsSource = оборудование,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "Название",
                    SelectedValueBinding = new Binding("id_Оборудования")
                });

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Часы работы",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Часы_Работы"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                var стоимостьПоId = оборудование.Select(o => new
                {
                    id = o.id,
                    ОтображениеСтоимости = $"{o.Стоимость_в_Час:N2} ₽"
                }).ToList();

                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Стоимость в час",
                    ItemsSource = стоимостьПоId,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ОтображениеСтоимости",
                    SelectedValueBinding = new Binding("id_Оборудования"),
                    IsReadOnly = true,
                    ElementStyle = new Style(typeof(ComboBox))
                    {
                        Setters = { new Setter(ComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Затраты",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" },
                    IsReadOnly = true,
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
                _dataGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                _dataGrid.Columns[2].Width = new DataGridLength(0.9, DataGridLengthUnitType.Star);
                _dataGrid.Columns[3].Width = new DataGridLength(0.6, DataGridLengthUnitType.Star);
                _dataGrid.Columns[4].Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
                _dataGrid.Columns[5].Width = new DataGridLength(1.3, DataGridLengthUnitType.Star);
            }
        }

        public void LoadSalary_Wage_Employees()
        {
            using (var context = new СтроительствоEntities())
            {
                var сотрудники = context.Сотрудники.ToList();

                var Salary_WageList = context.Заработная_Плата_Сотрудников
                .Include(emp => emp.Сотрудники)
                .ToList();

                _dataGrid.ItemsSource = Salary_WageList;
                _dataGrid.Columns.Clear();
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Сотрудник",
                    ItemsSource = сотрудники,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ФИО",
                    SelectedValueBinding = new Binding("id_Сотрудника"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Ставка в день",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Ставка_в_День") { StringFormat = "{0:N2} ₽" },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Отработано дней",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Отработано_Дней"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Затраты",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Затраты") { StringFormat = "{0:N2} ₽" },
                    IsReadOnly = true,
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            }
        }

        public void LoadUsers()
        {
            using (var context = new СтроительствоEntities())
            {
                var сотрудники = context.Сотрудники.ToList();

                var usersList = context.Пользователи
                .Include(emp => emp.Сотрудники)
                .ToList();

                _dataGrid.ItemsSource = usersList;
                _dataGrid.Columns.Clear();

                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("id"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Логин",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Логин"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Пароль",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Пароль"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Уровень Доступа",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("Уровень_Доступа"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
                    }
                });
                _dataGrid.Columns.Add(new DataGridComboBoxColumn
                {
                    Header = "Сотрудник",
                    ItemsSource = сотрудники,
                    SelectedValuePath = "id",
                    DisplayMemberPath = "ФИО",
                    SelectedValueBinding = new Binding("id_Сотрудника"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });

                _dataGrid.Columns[0].Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            }
        }
    }
}
