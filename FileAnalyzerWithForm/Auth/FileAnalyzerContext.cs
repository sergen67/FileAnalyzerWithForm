using System.Data.Entity;

namespace FileAnalyzerWithForm.Auth
{
    public class FileAnalyzerContext : DbContext
    {
        public FileAnalyzerContext() : base("name=FileAnalyzerDb") { }

        public DbSet<UserRecord> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Entity<UserRecord>()
              .Property(u => u.Username).IsRequired().HasMaxLength(100);
            mb.Entity<UserRecord>()
              .Property(u => u.PasswordHash).IsRequired();
            mb.Entity<UserRecord>()
              .Property(u => u.Salt).IsRequired();
            base.OnModelCreating(mb);
        }
    }
}
