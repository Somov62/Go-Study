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
    
    public partial class UserToken
    {
        public int Id { get; set; }
        public string UserLogin { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public Nullable<System.DateTime> DateExpire { get; set; }
        public string DeviceId { get; set; }
    
        public virtual User User { get; set; }
    }
}
