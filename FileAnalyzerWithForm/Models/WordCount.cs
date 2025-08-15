using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalyzerWithForm.Models
{
    public class WordCount
    {
        public string Word { get; }
        public int Count { get; }
        public WordCount(string word, int count) { Word = word; Count = count; }
    }
}
