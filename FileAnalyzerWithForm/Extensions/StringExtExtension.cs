namespace FileAnalyzerWithForm.Extensions
{
    public static class StringExtExtension
    {
        public static string BuildFilter(this string ext)
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
    }
}
