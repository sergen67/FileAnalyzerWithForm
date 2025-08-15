using FileAnalyzerWithForm.Reader;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;


namespace FileAnalyzerWithForm
{
    public partial class MainForm : Form
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        public MainForm(ILogger<MainForm> mainLogger, ILoggerFactory loggerFactory)
        {
            InitializeComponent();


            btnUpload.Enabled = false;
            gridWords.AutoGenerateColumns = false;
           

            _logger = mainLogger ?? throw new ArgumentNullException(nameof(mainLogger));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            var ext = (string)cboType.SelectedItem ?? "TXT";

            using (var dlg = new OpenFileDialog
            {
                Filter = BuildFilter(ext),
                Title = $"{ext} dosyası seçiniz",
                DefaultExt = ext.ToLowerInvariant(),   // "txt" / "docx" / "pdf"
                CheckFileExists = true,
                Multiselect = false
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    _logger.LogInformation("Dosya seçimi iptal edildi.");
                    return;
                }

                try
                {
                    IFileReader reader = FileReaderFactory.Create(dlg.FileName, _loggerFactory);
                    string content = reader.ReadContent(dlg.FileName) ?? string.Empty;

                    var res = TextAnalyzer.Analyze(content);


                    var words = res.TopWords
                                   .Where(w => w.Count >= 2)
                                   .OrderByDescending(w => w.Count)
                                   .ThenBy(w => w.Word)
                                   .ToList();
                    gridWords.DataSource = words;


                    var punc = res.PunctuationCounts
                                  .OrderByDescending(kv => kv.Value)
                                  .Select(kv => new PuncItem { Symbol = kv.Key, Count = kv.Value })
                                  .ToList();
                    gridPunc.DataSource = punc;

                    _logger.LogInformation("Dosya başarıyla analiz edildi: {File}", dlg.FileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dosya analizi sırasında hata oluştu: {File}", dlg.FileName);
                    MessageBox.Show($"Dosya analizi sırasında hata oluştu: {ex.Message}",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnUpload.Enabled = true;
                    cboType.Enabled = true;
                    UseWaitCursor = false;
                }
            }
        }
        private class PuncItem
        {
            public string Symbol { get; set; }
            public int Count { get; set; }
        }


        private string BuildFilter(string ext)
        {
            switch ((ext ?? "").ToLowerInvariant())
            {
                case "txt":
                    return "Metin Dosyaları (*.txt)|*.txt";
                case "docx":
                    return "Word Belgeleri (*.docx)|*.docx";
                case "pdf":
                    return "PDF Dosyaları (*.pdf)|*.pdf";
                default:
                    return "Tüm Dosyalar (*.*)|*.*";
            }
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboType.SelectedIndex >= 0)
            {
                btnUpload.Enabled = true;
            }
            else
            {
                btnUpload.Enabled = false;
            }
        }
    }
}
