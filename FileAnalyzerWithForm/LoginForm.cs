using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using FileAnalyzerWithForm.Auth;

namespace FileAnalyzerWithForm
{
    public partial class LoginForm : Form
    {
        private readonly IUserService _users;
        private readonly ILogger _logger;

        public string LoggedInUser { get; private set; }

        public LoginForm(IUserService users, ILogger<LoginForm> logger)
        {
            InitializeComponent();

            _users = users ?? throw new ArgumentNullException(nameof(users));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            txtPassword.UseSystemPasswordChar = true;
            AcceptButton = btnLogin;
            lblMsg.Text = string.Empty;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var u = (txtUserName.Text ?? "").Trim();
            var p = txtPassword.Text ?? "";

            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrEmpty(p))
            {
                SetMsg("Kullanıcı adı ve şifre boş olamaz.", error: true);
                return;
            }

            if (_users.TryLogin(u, p))
            {
                LoggedInUser = u;
                _logger.LogInformation("Login OK: {User}", u);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _logger.LogWarning("Login FAIL: {User}", u);
                SetMsg("Kullanıcı adı veya şifre yanlış.", error: true);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var u = (txtUserName.Text ?? "").Trim();
            var p = txtPassword.Text ?? "";

            if (_users.TryRegister(u, p, out var err))
            {
                _logger.LogInformation("Register OK: {User}", u);
                SetMsg("Kayıt başarılı, şimdi giriş yapabilirsin.", error: false);
            }
            else
            {
                _logger.LogWarning("Register FAIL: {User} - {Err}", u, err);
                SetMsg(err, error: true);
            }
        }

        private void SetMsg(string text, bool error)
        {
            lblMsg.ForeColor = error ? Color.Firebrick : Color.DarkGreen;
            lblMsg.Text = text ?? "";
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
