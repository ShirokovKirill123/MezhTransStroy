using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy.Memento
{
    // Управление состоянием авторизации
    class Ordinator
    {
        public string Username { get; set; }
        public string Role { get; set; }

        public Memento CreateMemento()
        {
            return new Memento(Username, Role);
        }

        public void SetMemento(Memento memento)
        {
            Username = memento.Username;
            Role = memento.Role;
        }
    }
}
