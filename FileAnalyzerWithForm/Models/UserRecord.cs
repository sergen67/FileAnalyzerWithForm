using System;

namespace FileAnalyzerWithForm.Auth
{
    public class UserRecord
    {
        public int Id { get; set; }
        public string Username { get; set; }        
        public byte[] PasswordHash { get; set; }    
        public byte[] Salt { get; set; }            
        public int Iterations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
