using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MezhTransStroy
{
    public static class HistoryService
    {
        public static void Add(int idЗаявки, int idСклада, int idОбъекта, int idМатериала, int количество, string описание)
        {
            using (var context = new СтроительствоEntities())
            {
                context.История_Перемещений.Add(new История_Перемещений
                {
                    id_Заявки = idЗаявки,
                    id_Склада = idСклада,
                    id_Объекта = idОбъекта,
                    id_Материала = idМатериала,
                    Количество = количество,
                    Дата_Перемещения = DateTime.Now, 
                    Описание = описание
                });

                context.SaveChanges();
            }
        }
    }
}
