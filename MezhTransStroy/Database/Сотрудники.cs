//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MezhTransStroy.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Сотрудники
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Сотрудники()
        {
            this.Заработная_Плата_Сотрудников = new HashSet<Заработная_Плата_Сотрудников>();
            this.Пользователи = new HashSet<Пользователи>();
            this.Работа_На_Объекте = new HashSet<Работа_На_Объекте>();
        }
    
        public int id { get; set; }
        public string ФИО { get; set; }
        public string Должность { get; set; }
        public string Квалификация { get; set; }
        public Nullable<System.DateTime> Дата_Приёма { get; set; }
        public string Контакты { get; set; }
        public Nullable<int> id_Отдела { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Заработная_Плата_Сотрудников> Заработная_Плата_Сотрудников { get; set; }
        public virtual Отделы Отделы { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Пользователи> Пользователи { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Работа_На_Объекте> Работа_На_Объекте { get; set; }
    }
}
