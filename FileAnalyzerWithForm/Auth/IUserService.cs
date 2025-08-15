namespace FileAnalyzerWithForm.Auth
{
    public interface IUserService
    {
        bool TryRegister(string username, string password, out string error);
        bool TryLogin(string username, string password);
    }
}
