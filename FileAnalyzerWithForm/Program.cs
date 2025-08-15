using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using System.IO;
using Serilog;
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

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            var appLogger = loggerFactory.CreateLogger("Program");
            appLogger.LogInformation("Uygulama başladı.");
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    Log.CloseAndFlush();
                    return;
                }
            }
            var mainLogger = loggerFactory.CreateLogger<MainForm>();
            Application.Run(new MainForm(mainLogger, loggerFactory));


            Log.CloseAndFlush();

        }
    }
}
