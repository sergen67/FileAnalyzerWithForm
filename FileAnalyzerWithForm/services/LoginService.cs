using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace FileAnalyzerWithForm.Auth
{
    public class LoginService : IUserService
    {
        private readonly ILogger _logger;

        public LoginService() { }
        public LoginService(ILogger<LoginService> logger) { _logger = logger; }

        public bool TryRegister(string username, string password, out string error)
        {
            error = null;
            username = (username ?? "").Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            { error = "Kullanıcı adı/şifre boş olamaz."; return false; }

            try
            {
                using (var db = new FileAnalyzerContext())
                {
                    bool exists = db.Users.Any(u => u.Username == username);
                    if (exists) { error = "Bu kullanıcı zaten kayıtlı."; return false; }

                    db.Users.Add(new UserRecord
                    {
                        Username = username,
                        Password = password
                    });
                    db.SaveChanges();
                }
                _logger?.LogInformation("Register OK: {User}", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Register FAIL: {User}", username);
                error = "Kayıt sırasında hata oluştu.";
                return false;
            }
        }

        public bool TryLogin(string username, string password)
        {
            username = (username ?? "").Trim();

            try
            {
                using (var db = new FileAnalyzerContext())
                {

                    var ok = db.Users.Any(u => u.Username == username && u.Password == password);
                    if (!ok) _logger?.LogWarning("Login FAIL: {User}", username);
                    else _logger?.LogInformation("Login OK: {User}", username);
                    return ok;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Login ERROR: {User}", username);
                return false;
            }
        }
    }
}
