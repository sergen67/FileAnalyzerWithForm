using System;
using System.Linq;

namespace FileAnalyzerWithForm.Auth
{
    public class EfUserService : IUserService
    {
        public bool TryRegister(string username, string password, out string error)
        {
            error = null;
            username = (username ?? "").Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            { error = "Kullanıcı adı/şifre boş olamaz."; return false; }

            using (var db = new FileAnalyzerContext())
            {
                if (db.Users.Any(u => u.Username == username))
                { error = "Bu kullanıcı zaten kayıtlı."; return false; }

                PasswordHasher.Create(password, out var hash, out var salt, out var iter);

                db.Users.Add(new UserRecord
                {
                    Username = username,
                    PasswordHash = hash,
                    Salt = salt,
                    Iterations = iter,
                    CreatedAt = DateTime.UtcNow
                });
                db.SaveChanges();
                return true;
            }
        }

        public bool TryLogin(string username, string password)
        {
            username = (username ?? "").Trim();
            using (var db = new FileAnalyzerContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Username == username);
                if (user == null) return false;

                var ok = PasswordHasher.Verify(password, user.PasswordHash, user.Salt, user.Iterations);
                if (ok)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    db.SaveChanges();
                }
                return ok;
            }
        }
    }
}
