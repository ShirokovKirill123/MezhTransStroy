using MezhTransStroy.Memento;
using System.IO;
using MezhTransStroy.Roles;
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
using MezhTransStroy.Database;
using MezhTransStroy;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private Caretaker _caretaker = new Caretaker();

        public AuthorizationPage()
        {
            InitializeComponent();
            Manager.MainFrame = SecondFrame;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            using (var context = new СтроительствоEntities())
            {
                var user = context.Пользователи.FirstOrDefault(u => u.Логин == username && u.Пароль == password);

                if (user != null)
                {
                    Manager.User.Username = username;
                    Manager.User.Role = user.Уровень_Доступа;
                    Manager.User.Employee = user.id_Сотрудника ?? 0;

                    _caretaker.Memento = Manager.User.CreateMemento();

                    MainWindow mainWindow = new MainWindow();

                    if (user.Уровень_Доступа == "админ")
                    {
                        mainWindow.MainFrame.Navigate(new AdminPage());
                    }
                    else if (user.Уровень_Доступа == "планирование")
                    {
                        mainWindow.MainFrame.Navigate(new PlanningPage());
                    }
                    else if (user.Уровень_Доступа == "склад")
                    {
                        mainWindow.MainFrame.Navigate(new WarehousePage());
                    }
                    else if (user.Уровень_Доступа == "строительство")
                    {
                        mainWindow.MainFrame.Navigate(new BuildingPage());
                    }
                    else if (user.Уровень_Доступа == "бухгалтерия")
                    {
                        mainWindow.MainFrame.Navigate(new AccountingPage());
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная роль пользователя");
                        return;
                    }

                    mainWindow.Show();
                    Window.GetWindow(this).Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль");
                }
            }
        }
    }
}
