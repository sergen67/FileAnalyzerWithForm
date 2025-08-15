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

            var logsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logsDir);

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.File(Path.Combine(logsDir, "app-txt"),
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 7,
                            shared: true)
              .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            Microsoft.Extensions.Logging.ILogger appLogger = loggerFactory.CreateLogger("Program");
            appLogger.LogInformation("Uygulama başladı.");
            // MainForm'a logger ve factory ver
            var mainLogger = loggerFactory.CreateLogger<MainForm>();
            Application.Run(new MainForm(mainLogger, loggerFactory));

            Log.CloseAndFlush();

        }
    }
}
