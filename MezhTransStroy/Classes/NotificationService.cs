using MezhTransStroy.Database;
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
        public static void Add(string текст, int applicationId)
        {
            using (var context = new СтроительствоEntities())
            {
                context.Уведомления.Add(new Уведомления
                {
                    Текст = текст,
                    Дата_Создания = DateTime.Now,
                    id_Заявки = applicationId
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

        public static void Remove(string текст)
        {
            using (var context = new СтроительствоEntities())
            {
                var notification = context.Уведомления.FirstOrDefault(u => u.Текст == текст);
                if (notification != null)
                {
                    context.Уведомления.Remove(notification);
                    context.SaveChanges();
                }
            }
        }

        public static int GetNotificationCount()
        {
            using (var context = new СтроительствоEntities())
            {
                return context.Уведомления.Count();
            }
        }

        public static void Update(string oldText, string newText)
        {
            using (var context = new СтроительствоEntities())
            {
                var notification = context.Уведомления.FirstOrDefault(u => u.Текст == oldText);
                if (notification != null)
                {
                    notification.Текст = newText;
                    notification.Дата_Создания = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        public static int? GetApplicationIdByMessage(string message)
        {
            using (var context = new СтроительствоEntities())
            {
                return context.Уведомления
                    .Where(u => u.Текст == message)
                    .Select(u => (int?)u.id_Заявки)
                    .FirstOrDefault();
            }
        }
    }
}
