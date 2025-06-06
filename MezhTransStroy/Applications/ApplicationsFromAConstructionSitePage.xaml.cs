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
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using MezhTransStroy.Database;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для ApplicationsFromAConstructionSitePage.xaml
    /// </summary>
    public partial class ApplicationsFromAConstructionSitePage : Page
    {
        private СтроительствоEntities context;

        public ApplicationsFromAConstructionSitePage()
        {
            InitializeComponent();
            context = new СтроительствоEntities();
            LoadApplications();
        }

        private void ClearSelectedNotification_Click(object sender, RoutedEventArgs e)
        {
            var selectedNotification = ApplicationsList.SelectedItem as string;
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
                string fileName = "Заявки.json";

                if (File.Exists(fileName))
                {
                    string json = File.ReadAllText(fileName);
                    var applications = JsonConvert.DeserializeObject<List<dynamic>>(json);

                    var context = new СтроительствоEntities();

                    // Поиск заявки по тексту
                    var applicationToRemove = applications.FirstOrDefault(app =>
                    {
                        int employeeId = (int)app.EmployeeId;
                        int objectId = app.ObjectId != null ? (int)app.ObjectId : 0;
                        string employeeName = "Неизвестный сотрудник";
                        string objectName = "Неизвестный объект";

                        var employee = context.Сотрудники.FirstOrDefault(i => i.id == employeeId);
                        var obj = context.Строительные_Объекты.FirstOrDefault(o => o.id == objectId);

                        if (employee != null) employeeName = employee.ФИО;
                        if (obj != null) objectName = obj.Название;

                        string text = $"Сотрудник: {employeeName} запросил {app.Quantity} {app.Unit} материала {app.Material} на объект {objectName} с id = {objectId}";

                        return text == selectedNotification;
                    });

                    if (applicationToRemove != null)
                    {
                        applications.Remove(applicationToRemove);

                        File.WriteAllText(fileName, JsonConvert.SerializeObject(applications, Formatting.Indented));
                        LoadApplications();

                        MessageBox.Show("Уведомление удалено", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти уведомление в файле.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Файл с заявками не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadApplications()
        {
            ApplicationsList.Items.Clear();

            string fileName = "Заявки.json";

            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                var applications = JsonConvert.DeserializeObject<List<dynamic>>(json);

                if (applications.Count == 0)
                {
                    ApplicationsList.Items.Add("Заявок пока нет");
                    return;
                }

                foreach (var app in applications)
                {
                    int employeeId = (int)app.EmployeeId;
                    int objectId = app.ObjectId != null ? (int)app.ObjectId : 0;

                    var employee = context.Сотрудники.FirstOrDefault(e => e.id == employeeId);
                    var obj = context.Строительные_Объекты.FirstOrDefault(o => o.id == objectId);

                    string employeeName = employee != null ? employee.ФИО : "Неизвестный сотрудник";
                    string objectName = obj != null ? obj.Название : "Неизвестный объект";

                    string text = $"Сотрудник: {employeeName} запросил {app.Quantity} {app.Unit} материала {app.Material} " +
                                  $"на объект {objectName} с id = {objectId}";

                    ApplicationsList.Items.Add(text);
                }
            }
            else
            {
                ApplicationsList.Items.Add("Файл с заявками не найден");
            }
        }
    }
}
