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
using Newtonsoft.Json;
using System.IO;
using MezhTransStroy.Memento;
using MezhTransStroy.Roles;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : Page
    {
        private readonly string уведомленияPath = "уведомления.json";

        public NotificationPage()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
            "Вы уверены, что хотите удалить все уведомления?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                File.WriteAllText(уведомленияPath, JsonConvert.SerializeObject(new List<string>(), Formatting.Indented));
                NotificationList.ItemsSource = new List<string> { "Нет уведомлений" };
                MessageBox.Show("Все уведомления удалены", "Уведомления", MessageBoxButton.OK, MessageBoxImage.Information);
                NotificationManager.LoadNotificationCount();
            }
        }

        private void LoadNotifications()
        {
            if (File.Exists(уведомленияPath))
            {
                string json = File.ReadAllText(уведомленияPath);
                var уведомления = JsonConvert.DeserializeObject<List<string>>(json);

                if (уведомления != null && уведомления.Count > 0)
                {
                    NotificationList.ItemsSource = уведомления;
                }
                else
                {
                    NotificationList.ItemsSource = new List<string> { "Нет уведомлений" };
                }
            }
            else
            {
                NotificationList.ItemsSource = new List<string> { "Нет уведомлений" };
            }
        }

        private void RedirectMaterials_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                var обработанныеЗаявки = context.Заявки
                    .Where(z => z.Статус == "Обработано")
                    .ToList();

                int перенаправлено = 0;

                string уведомленияPath = "уведомления.json";
                List<string> уведомления = new List<string>();

                if (File.Exists(уведомленияPath))
                {
                    string json = File.ReadAllText(уведомленияPath);
                    уведомления = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
                }

                foreach (var заявка in обработанныеЗаявки)
                {
                    var записьНаСкладе = context.Материалы_На_Складах.FirstOrDefault(m =>
                        m.id_Склада == заявка.id_Склада &&
                        m.id_Материала == заявка.id_Материала &&
                        m.id_Поставщика == заявка.id_Поставщика &&
                        m.Дата_Поступления == заявка.Дата_Заявки);

                    if (записьНаСкладе != null)
                    {
                        context.Материалы_На_Складах.Remove(записьНаСкладе);

                        context.Распределение_Материалов_На_Объект.Add(new Распределение_Материалов_На_Объект
                        {
                            id_Склада = заявка.id_Склада,
                            id_Объекта = заявка.id_Объекта,
                            id_Материала = заявка.id_Материала,
                            Количество = заявка.Количество_Материала,
                            Стоимость_Материалов = заявка.Стоимость_Материалов,
                            Дата_Передачи = DateTime.Now
                        });

                        перенаправлено++;

                        var материал = context.Материалы.FirstOrDefault(m => m.id == заявка.id_Материала);
                        var объект = context.Строительные_Объекты.FirstOrDefault(o => o.id == заявка.id_Объекта);

                        if (материал != null && объект != null)
                        {
                            string сообщение = $"Со склада {заявка.id_Склада} необходимо доставить " +
                                               $"{заявка.Количество_Материала} материала \"{материал.Название}\" " +
                                               $"на объект \"{объект.Название}\" c id = {объект.id}";

                            уведомления.Remove(сообщение);

                            HistoryHelper.Add($"Материал \"{материал.Название}\" в количестве {заявка.Количество_Материала} {материал.Единица_Измерения} отправлен со склада {заявка.id_Склада} на объект \"{объект.Название}\" с id = {объект.id} {DateTime.Now:dd.MM.yyyy HH:mm}");
                        }
                        заявка.Статус = "Уже на объекте";
                    }
                }

                context.SaveChanges();

                File.WriteAllText(уведомленияPath, JsonConvert.SerializeObject(уведомления, Formatting.Indented));
                NotificationManager.LoadNotificationCount();

                NotificationList.ItemsSource = уведомления.Count > 0
                    ? уведомления
                    : new List<string> { "Нет уведомлений" };

                if (перенаправлено > 0)
                {
                    MessageBox.Show($"Материалы успешно перенаправлены на объекты.\nВсего: {перенаправлено} записей",
                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Нет новых материалов для перенаправления.\nВозможно, они уже были перемещены",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
