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
            _users = users;
            _logger = logger;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var u = txtUserName.Text.Trim();
            var p = txtPassword.Text;

            if (_users.TryLogin(u, p))
            {
                LoggedInUser = u;
                _logger.LogInformation("Login OK: {User}", u);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre yanlış.");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var u = txtUserName.Text.Trim();
            var p = txtPassword.Text;

            if (_users.TryRegister(u, p, out var err))
            {
                MessageBox.Show("Kayıt başarılı, şimdi giriş yapabilirsin.");
            }
            else
            {
                MessageBox.Show(err);
            }
        }
    }
}
