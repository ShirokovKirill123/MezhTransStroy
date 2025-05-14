using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MezhTransStroy.Memento;

namespace MezhTransStroy
{
    class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Ordinator User { get; private set; } = new Ordinator();
    }
}
