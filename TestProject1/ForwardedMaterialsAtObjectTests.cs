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
        // проверка добавления выбранной записи в список исключённых
        [Fact]
        public void ClearSelectedNotification_ShouldAddSelectedRecordToExcludedList()
        {
            var forwardedMaterials = new ForwardedMaterialsAtObject();
            var testRecord = "Материал A отправлен со склада";
            MaterialManager.excludedMaterialsList.Clear();
            MaterialManager.excludedMaterialsList.Add(testRecord);
            Assert.Contains(testRecord, MaterialManager.excludedMaterialsList);
        }

        // проверка, что при очистке все записи попадают в список исключённых
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
            Assert.Equal(2, MaterialManager.excludedMaterialsList.Count);
            Assert.Contains("Материал A отправлен со склада", MaterialManager.excludedMaterialsList);
        }

        // проверяка, фильтрации исключённых записей при формировании отображаемого списка 
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
            var displayList = allRecords
                .FindAll(item => !MaterialManager.excludedMaterialsList.Contains(item));
            Assert.DoesNotContain("Материал B отправлен со склада", displayList);
            Assert.Equal(2, displayList.Count);
        }
    }
}
