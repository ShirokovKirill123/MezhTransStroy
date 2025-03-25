using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy.Memento
{
    //Хранение состояния авторизации
    class Memento
    {
        public string Username { get; private set; }
        public string Role { get; private set; }

        public Memento(string username, string role)
        {
            Username = username;
            Role = role;
        }
    }
}
