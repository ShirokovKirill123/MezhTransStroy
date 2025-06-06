using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MezhTransStroy.Database;

namespace MezhTransStroy.Сosts
{
    public class ProfitCalculationService
    {
        private readonly СтроительствоEntities _context;

        public ProfitCalculationService(СтроительствоEntities context)
        {
            _context = context;
        }

        public decimal? CalculateProfit(int objectId)
        {
            var profitData = _context.Строительные_Объекты
                .Where(o => o.id == objectId)
                .Select(d => new
                {
                    d.Выделенный_Бюджет,

                    ЗатратыНаОборудование = _context.Затраты_На_Оборудование
                        .Where(z => z.id_Объекта == d.id)
                        .Sum(z => (decimal?)z.Затраты) ?? 0,

                    ЗатратыНаЗарплату = _context.Работа_На_Объекте
                        .Where(r => r.id_Объекта == d.id)
                        .Select(r => _context.Заработная_Плата_Сотрудников
                            .Where(z => z.id_Сотрудника == r.id_Сотрудника)
                            .Select(z => (decimal?)z.Затраты)
                            .FirstOrDefault() ?? 0)
                        .Sum(),

                    ЗатратыНаМатериалы = _context.Распределение_Материалов_На_Объект
                        .Where(m => m.id_Объекта == d.id)
                        .Sum(m => (decimal?)m.Стоимость_Материалов) ?? 0
                })
                .FirstOrDefault();

            if (profitData == null)
                return null;

            var profit = profitData.Выделенный_Бюджет
                         - (profitData.ЗатратыНаОборудование
                         + profitData.ЗатратыНаЗарплату
                         + profitData.ЗатратыНаМатериалы);

            return profit;
        }
    }
}
