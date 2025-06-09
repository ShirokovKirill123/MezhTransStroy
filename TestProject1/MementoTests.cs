using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MezhTransStroy.Memento;

namespace TestProject1
{
    // Проверка, что Memento правильно сохраняет переданные данные состояния
    public class MementoTests
    {
        [Fact]
        public void Memento_ShouldStoreProvidedState()
        {
            var memento = new Memento("админ", "админ", 18);

            Assert.Equal("админ", memento.Username);
            Assert.Equal("админ", memento.Role);
            Assert.Equal(18, memento.Employee);
        }
        // Проверка, что Ordinator корректно создаёт снимок текущего состояния
        [Fact]
        public void Ordinator_ShouldCreateMementoWithCurrentState()
        {
            var ordinator = new Ordinator
            {
                Username = "план1",
                Role = "планирование",
                Employee = 1
            };
            var memento = ordinator.CreateMemento();
            Assert.Equal("план1", memento.Username);
            Assert.Equal("планирование", memento.Role);
            Assert.Equal(1, memento.Employee);
        }
        // Проверка, что Ordinator корректно восстанавливает данные из Memento
        [Fact]
        public void Ordinator_ShouldRestoreStateFromMemento()
        {
            var ordinator = new Ordinator();
            var memento = new Memento("строй1", "строительство", 3);

            ordinator.SetMemento(memento);

            Assert.Equal("строй1", ordinator.Username);
            Assert.Equal("строительство", ordinator.Role);
            Assert.Equal(3, ordinator.Employee);
        }
    }
}
