//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBaseCore
{
    using System;
    using System.Collections.Generic;
    
    public partial class DormitoryMedia
    {
        public int Id { get; set; }
        public Nullable<int> DormitoryId { get; set; }
        public string MediaURL { get; set; }
        public string Description { get; set; }
    
        public virtual Dormitory Dormitory { get; set; }
    }
}
