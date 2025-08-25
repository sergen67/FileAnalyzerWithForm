using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FileAnalyzerWithForm;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace FileAnalyzer_.Extensions
{
    public class DocxFileReader : IFileReader
    {
        private readonly ILogger<DocxFileReader> _logger;
        public DocxFileReader(ILogger<DocxFileReader> logger) => _logger = logger;

        public string ReadContent(string filePath)
        {
            try
            {
                var sb = new StringBuilder();

                using (var word = WordprocessingDocument.Open(filePath, false))
                {
                    var body = word.MainDocumentPart?.Document?.Body;
                    if (body != null)
                        foreach (var p in body.Elements<Paragraph>())
                            sb.AppendLine(p.InnerText);

                    foreach (var hp in word.MainDocumentPart.HeaderParts)
                        foreach (var p in hp.RootElement.Descendants<Paragraph>())
                            sb.AppendLine(p.InnerText);

                    foreach (var fp in word.MainDocumentPart.FooterParts)
                        foreach (var p in fp.RootElement.Descendants<Paragraph>())
                            sb.AppendLine(p.InnerText);
                }

                var text = sb.ToString();
                _logger.LogInformation("DOCX okundu: {File}", filePath);
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DOCX okunamadı: {File}", filePath);
                throw;
            }
        }
    }
}
