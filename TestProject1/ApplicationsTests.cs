using MezhTransStroy;
using MezhTransStroy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class ApplicationsTests
    {
        [Fact]
        public void Applications_Should_Set_Properties_Correctly()
        {
            var заявка = new Заявки
            {
                id_Объекта = 1,
                id_Склада = 2,
                id_Поставщика = 3,
                id_Материала = 4,
                Количество_Материала = 100,
                Стоимость_Материалов = 15000m,
                Статус = "Обработано",
                Дата_Заявки = new DateTime(2024, 1, 15)
            };

            Assert.Equal(1, заявка.id_Объекта);
            Assert.Equal(2, заявка.id_Склада);
            Assert.Equal(3, заявка.id_Поставщика);
            Assert.Equal(4, заявка.id_Материала);
            Assert.Equal(100, заявка.Количество_Материала);
            Assert.Equal(15000m, заявка.Стоимость_Материалов);
            Assert.Equal("Обработано", заявка.Статус);
            Assert.Equal(new DateTime(2024, 1, 15), заявка.Дата_Заявки);
        }
    }
}