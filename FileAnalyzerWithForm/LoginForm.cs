using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
namespace FileAnalyzerWithForm
{
    public partial class LoginForm : Form
    {
        SqlConnection connection = new SqlConnection("Data Source =DESKTOP-OBIK10F;Initial Catalog=Users;Integrated Security=True");
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btn_Click(object sender, EventArgs e)
        {
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text;

            using (var con = new SqlConnection("Data Source=DESKTOP-OBIK10F;Initial Catalog=Users;Integrated Security=True"))
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Users WHERE UserName=@u AND Password=@p", con))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Giriş başarılı!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre yanlış.");
                }
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
       
        }
    }
}
