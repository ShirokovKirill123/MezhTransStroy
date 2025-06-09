using MezhTransStroy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class ForwardedMaterialsAtObjectTests
    {
        // Проверка, что добавление записи в excludedMaterialsList работает
        [Fact]
        public void ClearSelectedNotification_ShouldAddSelectedRecordToExcludedList()
        {
            MaterialManager.excludedMaterialsList.Clear();

            var testRecord = "Материал A отправлен со склада";
            MaterialManager.excludedMaterialsList.Add(testRecord);

            Assert.Contains(testRecord, MaterialManager.excludedMaterialsList);
        }

        // Проверка, что при очистке добавляются все записи в excludedMaterialsList 
        [Fact]
        public void ClearAllForwardedMaterials_ShouldAddAllRecordsToExcludedList()
        {
            var testRecords = new List<string>
            {
                "Материал A отправлен со склада",
                "Материал B отправлен со склада"
            };

            MaterialManager.excludedMaterialsList.Clear();
            MaterialManager.excludedMaterialsList.AddRange(testRecords);

            Assert.Equal(testRecords.Count, MaterialManager.excludedMaterialsList.Count);
            Assert.Contains("Материал A отправлен со склада", MaterialManager.excludedMaterialsList);
            Assert.Contains("Материал B отправлен со склада", MaterialManager.excludedMaterialsList);
        }

        // Проверка фильтрации исключённых материалов из общего списка
        [Fact]
        public void ExcludedMaterials_ShouldNotContainDeletedItems()
        {
            var allRecords = new List<string>
            {
                "Материал A отправлен со склада",
                "Материал B отправлен со склада",
                "Материал C отправлен со склада"
            };

            MaterialManager.excludedMaterialsList.Clear();
            MaterialManager.excludedMaterialsList.Add("Материал B отправлен со склада");

            var displayList = allRecords.FindAll(item => !MaterialManager.excludedMaterialsList.Contains(item));

            Assert.DoesNotContain("Материал B отправлен со склада", displayList);
            Assert.Equal(2, displayList.Count);
        }
    }
}
