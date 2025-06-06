using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MezhTransStroy;
using MezhTransStroy.Database;

namespace TestProject1
{
    public class AllocationofMaterialstoObjectTests
    {
        [Fact]
        public void AllocationofMaterialstoObject_Should_Set_Properties_Correctly()
        {
            var распределение = new Распределение_Материалов_На_Объект
            {
                id_Склада = 8,
                id_Объекта = 9,
                id_Материала = 10,
                Количество = 300,
                Стоимость_Материалов = 45000m,
                Дата_Передачи = new DateTime(2024, 3, 5)
            };

            Assert.Equal(8, распределение.id_Склада);
            Assert.Equal(9, распределение.id_Объекта);
            Assert.Equal(10, распределение.id_Материала);
            Assert.Equal(300, распределение.Количество);
            Assert.Equal(45000m, распределение.Стоимость_Материалов);
            Assert.Equal(new DateTime(2024, 3, 5), распределение.Дата_Передачи);
        }
    }
}
