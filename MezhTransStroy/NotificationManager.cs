using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy
{
    public static class NotificationManager
    {
        public static event Action NotificationCountChanged;

        private static int _notificationCount;
        public static int NotificationCount
        {
            get => _notificationCount;
            set
            {
                _notificationCount = value;
                NotificationCountChanged?.Invoke();
            }
        }

        public static void LoadNotificationCount()
        {
            string path = "уведомления.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var notifications = JsonConvert.DeserializeObject<List<string>>(json);
                NotificationCount = notifications?.Count ?? 0;
            }
            else
            {
                NotificationCount = 0;
            }
        }
    }
}
