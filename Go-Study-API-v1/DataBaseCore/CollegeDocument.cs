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
    
    public partial class CollegeDocument
    {
        public int Id { get; set; }
        public Nullable<int> CollegeId { get; set; }
        public string DocumentURL { get; set; }
        public string Description { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
    
        public virtual College College { get; set; }
        public virtual DocumentType DocumentType { get; set; }
    }
}
