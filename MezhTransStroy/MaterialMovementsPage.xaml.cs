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

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для MaterialMovementsPage.xaml
    /// </summary>
    public partial class MaterialMovementsPage : Page
    {
        private readonly string историяPath = "перемещения.json";

        public MaterialMovementsPage()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            if (File.Exists(историяPath))
            {
                string json = File.ReadAllText(историяPath);
                var история = JsonConvert.DeserializeObject<List<string>>(json);

                if (история != null && история.Count > 0)
                {
                    NotificationList.ItemsSource = история;
                }
                else
                {
                    NotificationList.ItemsSource = new List<string> { "История пуста" };
                }
            }
            else
            {
                NotificationList.ItemsSource = new List<string> { "Файл истории не найден" };
            }
        }

        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить всю историю движений?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                File.WriteAllText(историяPath, JsonConvert.SerializeObject(new List<string>(), Formatting.Indented));
                NotificationList.ItemsSource = new List<string> { "История очищена" };
                MessageBox.Show("История движений успешно очищена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
