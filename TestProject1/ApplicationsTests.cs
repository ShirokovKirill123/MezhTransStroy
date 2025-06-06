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
            var ������ = new ������
            {
                id_������� = 1,
                id_������ = 2,
                id_���������� = 3,
                id_��������� = 4,
                ����������_��������� = 100,
                ���������_���������� = 15000m,
                ������ = "����������",
                ����_������ = new DateTime(2024, 1, 15)
            };

            Assert.Equal(1, ������.id_�������);
            Assert.Equal(2, ������.id_������);
            Assert.Equal(3, ������.id_����������);
            Assert.Equal(4, ������.id_���������);
            Assert.Equal(100, ������.����������_���������);
            Assert.Equal(15000m, ������.���������_����������);
            Assert.Equal("����������", ������.������);
            Assert.Equal(new DateTime(2024, 1, 15), ������.����_������);
        }
    }
}