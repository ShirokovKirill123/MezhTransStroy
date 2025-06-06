using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy
{
    public static class NotificationManager
    {
        public static int NotificationCount { get; set; }

        public static event Action NotificationCountChanged;

        public static void LoadNotificationCount()
        {
            NotificationCount = NotificationService.GetNotificationCount();
            NotificationCountChanged?.Invoke();
        }
    }
}
