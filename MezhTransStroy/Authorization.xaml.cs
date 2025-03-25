using MezhTransStroy.Memento;
using Newtonsoft.Json;
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

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        // Объект паттерна Memento
        private Caretaker _caretaker = new Caretaker();
        private const string MementoFileName = "memento.json";

        public Authorization()
        {
            InitializeComponent();
            Manager.MainFrame = SecondFrame;
            RestoreLoginFromMemento();
        }

        private void RestoreLoginFromMemento()
        {
            if (File.Exists(MementoFileName))
            {
                string json = File.ReadAllText(MementoFileName);
                _caretaker.Memento = JsonConvert.DeserializeObject<Memento.Memento>(json);               
            }
        }

        private void SaveLoginToMemento()
        {
            string json = JsonConvert.SerializeObject(_caretaker.Memento);
            File.WriteAllText(MementoFileName, json);
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

                    _caretaker.Memento = Manager.User.CreateMemento();

                    SaveLoginToMemento();

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
