using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MezhTransStroy;
using MezhTransStroy.Database;
using MezhTransStroy.Сosts;
using Moq;

namespace TestProject1
{
    public class ProfitCalculationServiceTests
    {
        [Fact]
        public void CalculateProfit_Should_Return_Correct_Profit()
        {
            // Arrange
            var mockContext = new Mock<СтроительствоEntities>();

            var строительныеОбъекты = new List<Строительные_Объекты>
            {
                new Строительные_Объекты { id = 1, Выделенный_Бюджет = 100000 }
            }.AsQueryable();

            var затратыНаОборудование = new List<Затраты_На_Оборудование>
            {
                new Затраты_На_Оборудование { id_Объекта = 1, Затраты = 20000 }
            }.AsQueryable();

            var работаНаОбъекте = new List<Работа_На_Объекте>
            {
                new Работа_На_Объекте { id_Объекта = 1, id_Сотрудника = 1 }
            }.AsQueryable();

            var зарплаты = new List<Заработная_Плата_Сотрудников>
            {
                new Заработная_Плата_Сотрудников { id_Сотрудника = 1, Затраты = 30000 }
            }.AsQueryable();

            var распределениеМатериалов = new List<Распределение_Материалов_На_Объект>
            {
                new Распределение_Материалов_На_Объект { id_Объекта = 1, Стоимость_Материалов = 25000 }
            }.AsQueryable();

            mockContext.Setup(c => c.Строительные_Объекты).Returns(DbSetMock.Create(строительныеОбъекты));
            mockContext.Setup(c => c.Затраты_На_Оборудование).Returns(DbSetMock.Create(затратыНаОборудование));
            mockContext.Setup(c => c.Работа_На_Объекте).Returns(DbSetMock.Create(работаНаОбъекте));
            mockContext.Setup(c => c.Заработная_Плата_Сотрудников).Returns(DbSetMock.Create(зарплаты));
            mockContext.Setup(c => c.Распределение_Материалов_На_Объект).Returns(DbSetMock.Create(распределениеМатериалов));

            var service = new ProfitCalculationService(mockContext.Object);
            var profit = service.CalculateProfit(1);
            Assert.Equal(25000, profit);
        }
        
    }
}
