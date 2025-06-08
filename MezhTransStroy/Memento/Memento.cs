using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy.Memento
{
    //Хранение состояния авторизации
    public class Memento
    {
        public string Username { get; private set; }
        public string Role { get; private set; }
        public int Employee { get; private set; }

        public Memento(string username, string role, int employee)
        {
            Username = username;
            Role = role;
            Employee = employee;
        }
    }
}
