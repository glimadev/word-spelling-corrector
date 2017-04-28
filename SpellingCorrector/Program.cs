using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpellingCorrector
{
    class Program
    {
        static void Main(string[] args)
        {
            SpellingCorrector spellingCorrector = new SpellingCorrector();

            while (true)
            {
                Console.WriteLine("Digite uma palavra :");

                string spelling = Console.ReadLine();

                Console.WriteLine(string.Format("Você quis dizer {0} ?", spellingCorrector.Correction(spelling)));
            }
        }
    }

    public class SpellingCorrector
    {
        private static Dictionary<string, int> _words = new Dictionary<string, int>();

        private static long _wordsSum = 0;

        private static char[] _letters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public SpellingCorrector()
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

        public long ProbabilityOfWord(string word)
        {
            long i = _wordsSum;

            return (_words.Any(x => x.Key == word)) ? (i / _words[word]) : 0;
        }

        public string Correction(string word)
        {
            return Candidates(Regex.Replace(word.Trim().ToLower(), @"[^a-z]+", String.Empty)).Max(ProbabilityOfWord);
        }

        private string[] Candidates(string word)
        {
            string[] candidates = Known(new string[] { word });

            if (candidates.Count() > 0) return candidates;

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

            foreach (var item in edit1)
            {
                edit2.AddRange(Edits1(item));
            }

            return edit2.GroupBy(x => x).Select(x => x.Key).ToArray();
        }

        private string[] Edits1(string word)
        {
            var splits = word.Splits();

            return splits.Inserts(_letters)
                .Union(splits.Deletes())
                .Union(splits.Transposes())
                .Union(splits.Replaces(_letters))
                .ToArray();
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

    public static class Helpers
    {
        public static Dictionary<string, string> Splits(this string word)
        {
            var splits = new Dictionary<string, string>();

            for (int i = 0; i < word.Length + 1; i++)
            {
                splits.Add(word.Substring(0, i), word.Substring(i));
            }

            return splits;
        }

        public static string[] Deletes(this Dictionary<string, string> splits)
        {
            return splits.Where(r => r.Value != "")
                .Select(x => x.Key + x.Value.Substring(x.Value.Length - (x.Value.Length - 1))).ToArray();
        }

        public static string[] Transposes(this Dictionary<string, string> splits)
        {
            return splits.Where(r => r.Value.Length > 1)
                .Select(x => x.Key + x.Value.Substring(1, 1) + x.Value.Substring(0, 1) + x.Value.Substring(2, x.Value.Length - 2)).ToArray();
        }

        public static string[] Replaces(this Dictionary<string, string> splits, char[] letters)
        {
            string[] replaces = new string[letters.Count() * (splits.Count - 1)];

            int replaceAux = 0;

            foreach (var item in splits)
            {
                if (item.Value != "")
                {
                    foreach (var letter in letters)
                    {
                        replaces[replaceAux] = item.Key + letter + item.Value.Substring(1, item.Value.Length - 1);

                        replaceAux++;
                    }
                }
            }

            return replaces;
        }

        public static string[] Inserts(this Dictionary<string, string> splits, char[] letters)
        {
            string[] inserts = new string[letters.Count() * splits.Count];

            int insertAux = 0;

            foreach (var item in splits)
            {
                foreach (var letter in letters)
                {
                    inserts[insertAux] = item.Key + letter + item.Value;

                    insertAux++;
                }
            }

            return inserts;
        }

        public static string Max(this IEnumerable<string> source, Func<string, long> selector)
        {
            long max = 0;

            string itemMax = "";

            foreach (var item in source)
            {
                long maxAux = selector.Invoke(item);

                if (maxAux >= max)
                {
                    max = maxAux;

                    itemMax = item;
                }
            }

            return itemMax;
        }
    }
}
