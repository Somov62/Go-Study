using System;
using SQLite;

namespace DataBaseCore.Entities
{
    [Table("UserData")]
    public class UserDataModel
    {
        [PrimaryKey, Column("Token")]
        public string Token { get; set; }

        [Column("RefreshToken")]
        public string RefreshToken { get; set; }

        [Column("DateExpired")]
        public DateTime DateExpired { get; set; }
    }
}
