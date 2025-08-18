using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Serilog;
using FileAnalyzerWithForm.Auth;

namespace FileAnalyzerWithForm
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logsDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(logsDir, "app-.txt"),
                              rollingInterval: RollingInterval.Day,
                              retainedFileCountLimit: 7,
                              shared: true)
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(b => b.AddSerilog(dispose: true));
            var appLogger = loggerFactory.CreateLogger("Program");

            try
            {
                appLogger.LogInformation("Uygulama başladı.");


                using (var db = new FileAnalyzerContext())
                {
                    db.Database.Initialize(false);
                }

                var userService = new LoginService();

                
                using (var login = new LoginForm(userService, loggerFactory.CreateLogger<LoginForm>()))
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        appLogger.LogInformation("Login iptal edildi.");
                        return;
                    }
                    appLogger.LogInformation("Login OK.");
                }

                
                var mainLogger = loggerFactory.CreateLogger<MainForm>();
                Application.Run(new MainForm(mainLogger, loggerFactory));
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
