using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileAnalyzerWithForm.Auth
{
 
    [Table("Users", Schema = "dbo")]  
    public class UserRecord
    {
        [Key]
       

        [Required, MaxLength(100)]
        [Column("UserName")]         
        public string Username { get; set; }

        [Column("Password")]
        public string Password { get; set; }
    }
}
