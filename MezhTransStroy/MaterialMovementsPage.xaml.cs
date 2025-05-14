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

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для MaterialMovementsPage.xaml
    /// </summary>
    public partial class MaterialMovementsPage : Page
    {
        public MaterialMovementsPage()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            using (var context = new СтроительствоEntities())
            {
                var история = context.История_Перемещений
                    .OrderBy(h => h.id)
                    .Select(h => h.Описание)
                    .ToList();

                NotificationList.ItemsSource = история.Any()
                    ? история
                    : new List<string> { "История пуста" };
            }
        }

        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить всю историю движений?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new СтроительствоEntities())
                {
                    context.История_Перемещений.RemoveRange(context.История_Перемещений);
                    context.SaveChanges();
                }

                LoadHistory();
                MessageBox.Show("История движений успешно очищена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
