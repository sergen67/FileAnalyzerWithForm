using FileAnalyzer_.Extensions;
using FileAnalyzerWithForm.Consts;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace FileAnalyzerWithForm.Reader
{
    public class FileReaderFactory
    {
        public static IFileReader Create(string path, ILoggerFactory loggerFactory)
        {
            var ext = (Path.GetExtension(path) ?? string.Empty)
                    .Trim()
                    .TrimStart('.')        // ".PDF" -> "PDF"
                    .ToLowerInvariant();   // "PDF"  -> "pdf"

           
            switch (ext)
            {
                case FileExtensions.Txt:
                    return new TxtFileReader(loggerFactory.CreateLogger<TxtFileReader>());
                case FileExtensions.Docx:
                    return new DocxFileReader(loggerFactory.CreateLogger<DocxFileReader>());
                case FileExtensions.Pdf:
                    return new PdfFileReader(loggerFactory.CreateLogger<PdfFileReader>());
                default:
                    throw new NotSupportedException($"Unsupported extension: {ext}");
            }
        }
    }
}
