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
    
    public partial class FAQ
    {
        public int Id { get; set; }
        public int EducationalInstitutionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    
        public virtual EducationalInstitution EducationalInstitution { get; set; }
    }
}
