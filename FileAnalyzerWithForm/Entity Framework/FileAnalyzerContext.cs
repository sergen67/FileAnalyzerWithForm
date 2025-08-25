using System.Data.Entity;

namespace FileAnalyzerWithForm.Auth
{
    public class FileAnalyzerContext : DbContext
    {
        static FileAnalyzerContext()
        {
            Database.SetInitializer<FileAnalyzerContext>(null);
        }

        public FileAnalyzerContext() : base("name=Users") { }

        public DbSet<UserRecord> Users { get; set; }
    }
}
