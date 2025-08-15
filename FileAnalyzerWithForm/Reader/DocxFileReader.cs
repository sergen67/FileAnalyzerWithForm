using Microsoft.Extensions.Logging;
using System;
using Xceed.Words.NET;

namespace FileAnalyzerWithForm.Reader
{
    public class DocxFileReader: IFileReader
    {
        private readonly ILogger<DocxFileReader> _logger;
        public DocxFileReader(ILogger<DocxFileReader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string ReadContent(string filePath)
        {
            try
            {
               using (var doc = DocX.Load(filePath))
                {
                    var text  = doc.Text;
                    _logger.LogInformation("DOCX okundu: {File}", filePath);
                    return text;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DOCX okunamadı: {File}", filePath);
                throw; 
            }
        }
    }
}
