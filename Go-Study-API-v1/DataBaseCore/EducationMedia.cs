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
    
    public partial class EducationMedia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EducationMedia()
        {
            this.EducationalInstitutions = new HashSet<EducationalInstitution>();
        }
    
        public int Id { get; set; }
        public Nullable<int> EducationalInstitutionId { get; set; }
        public string MediaURL { get; set; }
        public string Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EducationalInstitution> EducationalInstitutions { get; set; }
        public virtual EducationalInstitution EducationalInstitution { get; set; }
    }
}
