using FileAnalyzerWithForm.Consts;

namespace FileAnalyzerWithForm.Extensions
{
    public static class StringExtExtension
    {
        public static string BuildFilter(this string ext)
        {
            var key = (ext ?? "")
                      .Trim()
                      .TrimStart('.')         // ".TXT" -> "TXT"
                      .ToLowerInvariant();    // "TXT" -> "txt"

            switch ((ext ?? "").ToLowerInvariant())
            {
                case FileExtensions.Txt:
                    return "Metin Dosyaları (*.txt)|*.txt";
                case FileExtensions.Docx:
                    return "Word Belgeleri (*.docx)|*.docx";
                case FileExtensions.Pdf:
                    return "PDF Dosyaları (*.pdf)|*.pdf";
                default:
                    return "Tüm Dosyalar (*.*)|*.*";
            }
        }
    }
}
