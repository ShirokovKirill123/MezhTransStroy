using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MezhTransStroy
{
    public static class NotificationService
    {
        public static void Add(string текст)
        {
            using (var context = new СтроительствоEntities())
            {
                context.Уведомления.Add(new Уведомления
                {
                    Текст = текст,
                    Дата_Создания = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        public static List<string> Get()
        {
            using (var context = new СтроительствоEntities())
            {
                return context.Уведомления
                    .OrderByDescending(u => u.Дата_Создания)
                    .Select(u => u.Текст)
                    .ToList();
            }
        }

        public static void Clear()
        {
            using (var context = new СтроительствоEntities())
            {
                context.Уведомления.RemoveRange(context.Уведомления);
                context.SaveChanges();
            }
        }

        public static int GetNotificationCount()
        {
            using (var context = new СтроительствоEntities())
            {
                return context.Уведомления.Count();
            }
        }
    }
}
