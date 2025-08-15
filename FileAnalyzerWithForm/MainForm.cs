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
            _logger = mainLogger ?? throw new ArgumentNullException(nameof(mainLogger));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            cboType.SelectedIndexChanged += (s, e) => btnUpload.Enabled = cboType.SelectedIndex >= 0;
            cboType.Items.AddRange(new string[] { "TXT", "DOCX", "PDF" });  
            btnUpload.Enabled = false;  
            progressBar.Visible = false;

            cboType.SelectedIndexChanged += (s, e) =>
            {
                if (cboType.SelectedIndex >= 0)
                {
                    btnUpload.Enabled = true;
                }
                else
                {
                    btnUpload.Enabled = false;
                }
            };
            btnUpload.Click += btnUpload_Click;


        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            var ext = (string)cboType.SelectedItem ?? "TXT";

            using(var dlg = new OpenFileDialog
            {
                Filter = BuildFilter(ext),
                Title = "Dosya Seçin",  
                DefaultExt = $"{ext.ToLowerInvariant()} Dosya Seçiniz" ,
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
                    progressBar.Visible = true;
                    IFileReader reader = FileReaderFactory.Create(dlg.FileName, _loggerFactory);
                    string content = reader.ReadContent(dlg.FileName) ?? string.Empty;

                    var res = TextAnalyzer.Analyze(content);
                    var words = res.TopWords.Where(w => w.Count >= 2).ToList();

                    txtOutPut.Clear();
                    txtOutPut.AppendText($"Dosya: {dlg.FileName}\n");
                    txtOutPut.AppendText($"Kelime Sayısı: {res.DistinctWordCount}\n");
                    txtOutPut.AppendText("En Sık Kullanılan Kelimeler:\n");
                    foreach (var wc in words)
                    {
                        txtOutPut.AppendText($"- {wc.Word}: {wc.Count}\n");
                    }
                    txtOutPut.AppendText("\nNoktalama İşaretleri:\n");
                    foreach (var kvp in res.PunctuationCounts.OrderByDescending(k => k.Value))
                    {
                        txtOutPut.AppendText($"- {kvp.Key}: {kvp.Value}\n");
                    }
                    _logger.LogInformation("Dosya başarıyla analiz edildi: {File}", dlg.FileName);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dosya analizi sırasında hata oluştu: {File}", dlg.FileName);
                    MessageBox.Show($"Dosya analizi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressBar.Visible = false;
                }
            }

        }

        private string BuildFilter(string ext)
        {
            switch((ext ?? "").ToLowerInvariant())
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

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
