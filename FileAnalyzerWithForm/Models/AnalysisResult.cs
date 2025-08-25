using System.Collections.Generic;

namespace FileAnalyzerWithForm.Models
{
    public class AnalysisResult
    {
        public int DistinctWordCount { get; }
        public List<WordCount> TopWords { get; }
        public Dictionary<string, int> PunctuationCounts { get; }
        public AnalysisResult(int distinctWordCount, List<WordCount> topWords, Dictionary<string, int> punctuationCounts)
        {
            DistinctWordCount = distinctWordCount;
            TopWords = topWords ?? new List<WordCount>();
            PunctuationCounts = punctuationCounts ?? new Dictionary<string, int>();
        }
    }
}
