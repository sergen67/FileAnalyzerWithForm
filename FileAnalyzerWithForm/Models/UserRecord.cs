using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileAnalyzerWithForm.Auth
{
    // Tablonun gerçek adı ve şeması
    [Table("Users", Schema = "dbo")]  // örn: dbo.Users
    public class UserRecord
    {
        [Key]
       

        [Required, MaxLength(100)]
        [Column("UserName")]           // kullanıcı adı sütun adın
        public string Username { get; set; }

        [Column("Password")]
        public string Password { get; set; }
    }
}
