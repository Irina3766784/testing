//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Тестирование_персонала
{
    using System;
    using System.Collections.Generic;
    
    public partial class Results
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Results()
        {
            this.ResultAnswers = new HashSet<ResultAnswers>();
        }
    
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> TestId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultAnswers> ResultAnswers { get; set; }
        public virtual Users Users { get; set; }
        public virtual Tests Tests { get; set; }
    }
}
