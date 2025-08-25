using FileAnalyzerWithForm.Extensions;
using FileAnalyzerWithForm.Reader;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileAnalyzerWithForm.Extensions;   // ← extension method için şart

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
  
        private async void btnUpload_Click(object sender, EventArgs e)
        {
            var choiceRaw = cboType.SelectedItem?.ToString() ?? "";
            var ext = choiceRaw.Trim().TrimStart('.').ToLowerInvariant();


            using (var dlg = new OpenFileDialog
            {
                Filter = ext.BuildFilter(),
                Title = $"{ext.ToUpperInvariant()} dosyası seçiniz",
                DefaultExt = ext,
                CheckFileExists = true,
                Multiselect = false,
                FilterIndex = 1
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    btnUpload.Enabled = false;
                    cboType.Enabled = false;

                    // >>> tüm türlerde aynı: marquee açık
                    SetProgressVisible(true, marquee: true);

                    var reader = FileReaderFactory.Create(dlg.FileName, _loggerFactory);

                    // IO ve analiz CPU işini UI dışına at
                    string content = await Task.Run(() => reader.ReadContent(dlg.FileName) ?? string.Empty);
                    var res = await Task.Run(() => TextAnalyzer.Analyze(content));

                    // Grid'leri doldur (senin mevcut bağlama şeklinle)
                    var words = res.TopWords
                                   .Where(w => w.Count >= 2)
                                   .OrderByDescending(w => w.Count)
                                   .ThenBy(w => w.Word)
                                   .ToList();
                    gridWords.DataSource = words;

                    var punc = res.PunctuationCounts
                                  .OrderByDescending(kv => kv.Value)
                                  .Select(kv => new { Symbol = kv.Key, Count = kv.Value })
                                  .ToList();
                    gridPunc.DataSource = punc;

                    _logger.LogInformation("Analiz OK: {File}", dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                finally
                {
                    SetProgressVisible(false);
                    btnUpload.Enabled = true;
                    cboType.Enabled = true;
                }
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Çıkmak İstediğinize Emin misiniz?", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (gridWords.Rows.Count == 0)
            {
                MessageBox.Show("Dışa aktarılacak veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Dışa Aktar",
                Filter = "CSV (Kelimeler)|*.csv",
                FileName = "Kelimeler.csv",
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;
            ExpertGridToCsv(gridWords, dlg.FileName, delimiter: ';');
            MessageBox.Show("Dışa aktarma tamam.", "Bilgi");
        }

        private void ExpertGridToCsv(DataGridView grid, string path, char delimiter = ';')
        {
            var sb = new StringBuilder();

            var visibleCols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            sb.AppendLine(string.Join(delimiter.ToString(), visibleCols.Select(c => CsvEscape(c.HeaderText, delimiter))));


            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                var cells = visibleCols.Select(c =>
                {
                    var v = row.Cells[c.Index].Value;
                    return CsvEscape(v?.ToString() ?? string.Empty, delimiter);
                });
                sb.AppendLine(string.Join(delimiter.ToString(), cells));
            }


            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        }
        private static string CsvEscape(string s, char delimiter)
        {
            if (s == null) s = string.Empty;
            bool mustQuote = s.Contains("\"") || s.Contains("\r") || s.Contains("\n") || s.Contains(delimiter.ToString());
            s = s.Replace("\"", "\"\"");
            return mustQuote ? $"\"{s}\"" : s;
        }

      
        private async Task<string> ReadTextFileWithProgressAsync(string path, IProgress<int> progress)
        {
            // TXT için yüzde ilerleme (yaklaşık). Büyük dosyalarda UI donmaz.
            var sb = new StringBuilder();
            const int charBufSize = 8192;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, useAsync: true))
            using (var sr = new StreamReader(fs, detectEncodingFromByteOrderMarks: true))
            {
                long total = fs.Length;
                var buf = new char[charBufSize];
                int read;

                while ((read = await sr.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    sb.Append(buf, 0, read);
                    // StreamReader iç buffer kullandığı için yüzde yaklaşık; fs.Position iş görür.
                    if (total > 0) progress?.Report((int)(fs.Position * 100L / total));
                }
            }
            progress?.Report(100);
            return sb.ToString();
        }
        private void MainForm_Load(object s, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
        }
        private void SetProgressVisible(bool visible, bool marquee = false)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => SetProgressVisible(visible, marquee))); return; }

            progressBar1.Visible = visible;
            progressBar1.Style = marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnimationSpeed = marquee ? 30 : 0;
            if (!marquee) progressBar1.Value = 0;

            progressBar1.BringToFront();   // grid’ler kapatmasın
        }

        private void SetProgressValue(int v)
        {
            if (InvokeRequired) { BeginInvoke(new Action<int>(SetProgressValue), v); return; }

            if (progressBar1.Style != ProgressBarStyle.Blocks)
                progressBar1.Style = ProgressBarStyle.Blocks;

            if (v < 0) v = 0; if (v > 100) v = 100;
            progressBar1.Value = v;
        }

    }
}

