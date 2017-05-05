using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpellingCorrector
{
    public class SpellingCorrector
    {
        private static Dictionary<string, int> _words = new Dictionary<string, int>();

        private static double _wordsSum = 0;

        private static string[] _candidates = null;

        private static char[] _letters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public SpellingCorrector()
        {
            if (_words.Count == 0)
            {
                _words = File.ReadAllText(@"c:\temp\text.txt")
                           .Split('\n')
                           .SelectMany(x => x.Split(' '))
                           .Select(x => Regex.Replace(x.Trim().ToLower(), @"[^a-z]+", String.Empty))
                           .Where(x => !string.IsNullOrEmpty(x))
                           .GroupBy(x => x)
                           .OrderBy(x => x.Key)
                           .ToDictionary(x => x.Key, x => x.Count());

                _wordsSum = _words.Sum(x => x.Value);
            }
        }

        public double ProbabilityOfWord(string word)
        {
            return (_words.Any(x => x.Key == word)) ? (_words[word] / _wordsSum) : 0;
        }

        public string Correction(string word)
        {
            var watch = Stopwatch.StartNew();

            _candidates = Candidates(Regex.Replace(word.Trim().ToLower(), @"[^a-z]+", String.Empty));

            watch.Stop();

            Console.WriteLine((watch.Elapsed.TotalMilliseconds).ToString("0.00 ms"));

            return _candidates.Max(ProbabilityOfWord);
        }

        public string GetCandidates()
        {
            return string.Join(", ", _candidates.OrderByDescending(x => ProbabilityOfWord(x)));
        }

        private string[] Candidates(string word)
        {
            string[] candidates = Known(new string[] { word });

            if (candidates.Count > 0) return candidates;

            candidates = Known(Edits1(word));

            if (candidates.Count() > 0) return candidates;

            candidates = Known(Edits2(word));

            if (candidates.Count() > 0) return candidates;

            return new string[] { word };
        }

        private string[] Edits2(string word)
        {
            var edit1 = Edits1(word);

            List<string> edit2 = new List<string>();

            foreach (var item in edit1) edit2.AddRange(Edits1(item));

            return edit2.GroupBy(x => x).Select(x => x.Key).ToArray();
        }

        private string[] Edits1(string word)
        {
            var splits = word.Splits();

            return splits.Inserts(_letters).Union(splits.Deletes()).Union(splits.Transposes()).Union(splits.Replaces(_letters)).ToArray();
        }

        private string[] Known(string[] v)
        {
            List<string> item2 = new List<string>();

            List<string> wordsList = _words.Select(x => x.Key).ToList();

            for (int i = 0; i < v.Length; i++)
            {
                if (wordsList.BinarySearch(v[i]) >= 0)
                {
                    item2.Add(v[i]);
                }
            }

            return item2.ToArray();
        }
    }
}
