using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using System.IO;
using Serilog;
namespace FileAnalyzerWithForm

{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            });
            //var mainLogger = loggerFactory.CreateLogger<MainForm>();
            //Application.Run(new MainForm(mainLogger, loggerFactory));
            //Microsoft.Extensions.Logging.ILogger appLogger = loggerFactory.CreateLogger("Program");
            //appLogger.LogInformation("Uygulama başladı.");

            // ... loggerFactory kurulduktan hemen sonra ekle:
            var appLogger = loggerFactory.CreateLogger("Program");
            appLogger.LogInformation("Uygulama başladı.");

            // 1) LOGIN ÖNCE
            // A) LoginForm'un parametresiz ctor'u varsa:
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    Log.CloseAndFlush();
                    return; // iptal -> uygulamadan çık
                }
            }

            // // B) LoginForm logger istiyorsa (böyleyse A'yı sil, bunu aç):
            // using (var login = new LoginForm(loggerFactory.CreateLogger<LoginForm>()))
            // {
            //     if (login.ShowDialog() != DialogResult.OK)
            //     {
            //         Log.CloseAndFlush();
            //         return;
            //     }
            // }

            // 2) LOGIN OK => MainForm
            var mainLogger = loggerFactory.CreateLogger<MainForm>();
            Application.Run(new MainForm(mainLogger, loggerFactory));


            Log.CloseAndFlush();

        }
    }
}
