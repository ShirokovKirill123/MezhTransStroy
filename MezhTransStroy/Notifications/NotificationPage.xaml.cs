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
using System.IO;
using MezhTransStroy.Memento;
using MezhTransStroy.Roles;
using MezhTransStroy.Database;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : Page
    {
        public NotificationPage()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            var уведомления = NotificationService.Get();
            NotificationList.ItemsSource = уведомления.Any()
            ? уведомления.AsEnumerable().Reverse().ToList()
            : new List<string> { "Нет уведомлений" };
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
                NotificationService.Clear();
                NotificationList.ItemsSource = new List<string> { "Нет уведомлений" };
                MessageBox.Show("Все уведомления удалены", "Уведомления", MessageBoxButton.OK, MessageBoxImage.Information);
                NotificationManager.LoadNotificationCount();
            }
        }

        private void ClearSelectedNotification_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = NotificationList.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedNotification) || selectedNotification == "Нет уведомлений")
            {
                MessageBox.Show("Выберите уведомление для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить выбранное уведомление?\n\n{selectedNotification}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NotificationService.Remove(selectedNotification);
                LoadNotifications();
                NotificationManager.LoadNotificationCount();

                MessageBox.Show("Уведомление удалено", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RedirectMaterials_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new СтроительствоEntities())
            {
                var обработанныеЗаявки = context.Заявки
                    .Where(z => z.Статус == "На складе" || z.Статус == "Частично на объекте")
                    .ToList();

                int перенаправлено = 0;

                int employeeId = Manager.User.Employee;
                var сотрудник = context.Сотрудники
                    .Where(emp => emp.id == employeeId)
                    .Select(emp => new { emp.ФИО, emp.id_Отдела })
                    .FirstOrDefault();

                string fio = сотрудник?.ФИО ?? "Неизвестный пользователь";
                string отдел = сотрудник != null
                    ? context.Отделы.Where(o => o.id == сотрудник.id_Отдела).Select(o => o.Название).FirstOrDefault() ?? "Неизвестный отдел"
                    : "Неизвестный отдел";

                foreach (var заявка in обработанныеЗаявки)
                {
                    var записьНаСкладе = context.Материалы_На_Складах.FirstOrDefault(m =>
                        m.id_Склада == заявка.id_Склада &&
                        m.id_Материала == заявка.id_Материала &&
                        m.id_Поставщика == заявка.id_Поставщика &&
                        m.Дата_Поступления == заявка.Дата_Заявки);

                    if (записьНаСкладе == null || записьНаСкладе.Количество <= 0)
                        continue;

                    // Подсчёт уже перенаправленного количества по заявке
                    int alreadyTransferred = context.Распределение_Материалов_На_Объект
                        .Where(r =>
                            r.id_Объекта == заявка.id_Объекта &&
                            r.id_Материала == заявка.id_Материала &&
                            r.id_Склада == заявка.id_Склада &&
                            r.id_Заявки == заявка.id)
                        .Sum(r => (int?)r.Количество) ?? 0;

                    int remainingToTransfer = (заявка.Количество_Материала ?? 0) - alreadyTransferred;
                    if (remainingToTransfer <= 0)
                        continue;

                    // Определение максимально допустимого количества для перенаправления
                    int quantityToTransfer = Math.Min(записьНаСкладе.Количество.Value, remainingToTransfer);

                    // Перенаправление материала
                    context.Распределение_Материалов_На_Объект.Add(new Распределение_Материалов_На_Объект
                    {
                        id_Склада = заявка.id_Склада,
                        id_Объекта = заявка.id_Объекта,
                        id_Материала = заявка.id_Материала,
                        Количество = quantityToTransfer,
                        Стоимость_Материалов = заявка.Стоимость_Материалов,
                        Дата_Передачи = DateTime.Now,
                        Израсходовано = 0,
                        id_Заявки = заявка.id
                    });

                    // Уменьшение остатка на складе
                    записьНаСкладе.Количество -= quantityToTransfer;
                    if (записьНаСкладе.Количество <= 0)
                        context.Материалы_На_Складах.Remove(записьНаСкладе);

                    перенаправлено++;

                    var материал = context.Материалы.FirstOrDefault(m => m.id == заявка.id_Материала);
                    var объект = context.Строительные_Объекты.FirstOrDefault(o => o.id == заявка.id_Объекта);

                    if (материал != null && объект != null)
                    {
                        HistoryService.Add(
                             заявка.id,
                             заявка.id_Склада.Value,
                             заявка.id_Объекта.Value,
                             заявка.id_Материала.Value,
                             quantityToTransfer,
                             $"Материал \"{материал.Название}\" в количестве {quantityToTransfer} {материал.Единица_Измерения} " +
                             $"отправлен со склада {заявка.id_Склада} на объект \"{объект.Название}\" с id = {объект.id} " +
                             $"({DateTime.Now:dd.MM.yyyy}), пользователем: {fio} ({отдел})");

                        int totalTransferred = alreadyTransferred + quantityToTransfer;
                        if (totalTransferred >= заявка.Количество_Материала)
                        {
                            заявка.Статус = "На объекте";

                            // Удаление уведомления
                            string oldNotification = $"Со склада {заявка.id_Склада} необходимо доставить " +
                                                     $"{(заявка.Количество_Материала - alreadyTransferred)} материала \"{материал.Название}\" " +
                                                     $"на объект \"{объект.Название}\" c id = {объект.id}";

                            NotificationService.Remove(oldNotification);
                        }
                        else
                        {
                            заявка.Статус = "Частично на объекте";

                            string newNotification = $"Со склада {заявка.id_Склада} необходимо доставить " +
                                                     $"{(заявка.Количество_Материала - totalTransferred)} материала \"{материал.Название}\" " +
                                                     $"на объект \"{объект.Название}\" c id = {объект.id}";

                            string oldNotification = $"Со склада {заявка.id_Склада} необходимо доставить " +
                                                     $"{(заявка.Количество_Материала - alreadyTransferred)} материала \"{материал.Название}\" " +
                                                     $"на объект \"{объект.Название}\" c id = {объект.id}";

                            NotificationService.Update(oldNotification, newNotification);
                        }
                    }
                }

                context.SaveChanges();

                LoadNotifications();
                NotificationManager.LoadNotificationCount();

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

        private void RedirectSelectedNotification_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = NotificationList.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedNotification) || selectedNotification == "Нет уведомлений")
            {
                MessageBox.Show("Выберите уведомление", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

                int? applicationId = NotificationService.GetApplicationIdByMessage(selectedNotification);
                if (applicationId == null)
                {
                    MessageBox.Show("Уведомление не привязано к заявке", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var targetApplication = context.Заявки.FirstOrDefault(z => z.id == applicationId);
                if (targetApplication == null)
                {
                    MessageBox.Show("Заявка по уведомлению не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var stockEntry = context.Материалы_На_Складах.FirstOrDefault(m =>
                    m.id_Склада == targetApplication.id_Склада &&
                    m.id_Материала == targetApplication.id_Материала &&
                    m.id_Поставщика == targetApplication.id_Поставщика &&
                    m.Дата_Поступления == targetApplication.Дата_Заявки);

                if (stockEntry == null || stockEntry.Количество <= 0)
                {
                    MessageBox.Show("Нет материала на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var materialName = context.Материалы.FirstOrDefault(m => m.id == targetApplication.id_Материала)?.Название;
                var siteName = context.Строительные_Объекты.FirstOrDefault(o => o.id == targetApplication.id_Объекта)?.Название;

                // Подсчёт уже перенаправленного количества по этой заявке
                int alreadyTransferred = context.Распределение_Материалов_На_Объект
                .Where(r =>
                    r.id_Объекта == targetApplication.id_Объекта &&
                    r.id_Материала == targetApplication.id_Материала &&
                    r.id_Склада == targetApplication.id_Склада &&
                    r.id_Заявки == targetApplication.id)
                .Sum(r => (int?)r.Количество) ?? 0;

                int remainingToTransfer = (targetApplication.Количество_Материала ?? 0) - alreadyTransferred;

                if (remainingToTransfer <= 0)
                {
                    MessageBox.Show("Заявка уже полностью выполнена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    NotificationService.Remove(selectedNotification);
                    LoadNotifications();
                    return;
                }

                // Определение максимально допустимого количества для перенаправления
                int maxQuantity = Math.Min(stockEntry.Количество.Value, remainingToTransfer);

                var dialog = new RedirectQuantityWindow(materialName, maxQuantity);
                if (dialog.ShowDialog() != true)
                    return;

                int quantityToTransfer = dialog.Quantity;

                // Перенаправление материала
                context.Распределение_Материалов_На_Объект.Add(new Распределение_Материалов_На_Объект
                {
                    id_Склада = targetApplication.id_Склада,
                    id_Объекта = targetApplication.id_Объекта,
                    id_Материала = targetApplication.id_Материала,
                    Количество = quantityToTransfer,
                    Стоимость_Материалов = targetApplication.Стоимость_Материалов,
                    Дата_Передачи = DateTime.Now,
                    Израсходовано = 0,
                    id_Заявки = targetApplication.id
                });

                // Уменьшение остатка на складе
                stockEntry.Количество -= quantityToTransfer;
                if (stockEntry.Количество <= 0)
                    context.Материалы_На_Складах.Remove(stockEntry);

                // История движений
                HistoryService.Add(
                    targetApplication.id,
                    targetApplication.id_Склада.Value,
                    targetApplication.id_Объекта.Value,
                    targetApplication.id_Материала.Value,
                    quantityToTransfer,
                     $"Материал \"{materialName}\" в количестве {quantityToTransfer} отправлен со склада {targetApplication.id_Склада} " +
                     $"на объект \"{siteName}\" с id = {targetApplication.id_Объекта} ({DateTime.Now:dd.MM.yyyy}), пользователем: {fio} ({отдел})"
                );

                // Проверка удаления уведомления
                if (alreadyTransferred + quantityToTransfer >= targetApplication.Количество_Материала)
                {
                    targetApplication.Статус = "На объекте";
                    NotificationService.Remove(selectedNotification);
                }
                else
                {
                    int newRemainingToTransfer = remainingToTransfer - quantityToTransfer;

                    string partialNotification = $"Со склада {targetApplication.id_Склада} необходимо доставить " +
                                                 $"{newRemainingToTransfer} материала \"{materialName}\" " +
                                                 $"на объект \"{siteName}\" c id = {targetApplication.id_Объекта}";

                    NotificationService.Update(selectedNotification, partialNotification);
                    targetApplication.Статус = "Частично на объекте";
                }

                context.SaveChanges();
            }

            LoadNotifications();
            NotificationManager.LoadNotificationCount();
            MessageBox.Show("Материал перенаправлен на объект", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RedirectedMaterials_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ForwardedMaterialsAtObject());
        }
    }
}
