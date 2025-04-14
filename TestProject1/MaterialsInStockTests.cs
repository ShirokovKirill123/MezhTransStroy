using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MezhTransStroy;
namespace TestProject1
{
    public class MaterialsInStockTests
    {
        [Fact]
        public void МатериалыНаСкладах_Should_Set_Properties_Correctly()
        {
            var запись = new Материалы_На_Складах
            {
                id_Склада = 5,
                id_Материала = 6,
                Количество = 200,
                Стоимость_Материалов = 30000m,
                id_Поставщика = 7,
                Дата_Поступления = new DateTime(2024, 2, 10)
            };

            Assert.Equal(5, запись.id_Склада);
            Assert.Equal(6, запись.id_Материала);
            Assert.Equal(200, запись.Количество);
            Assert.Equal(30000m, запись.Стоимость_Материалов);
            Assert.Equal(7, запись.id_Поставщика);
            Assert.Equal(new DateTime(2024, 2, 10), запись.Дата_Поступления);
        }
    }
}
