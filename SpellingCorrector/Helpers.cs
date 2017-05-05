using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellingCorrector
{
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

        public static string Max(this IEnumerable<string> source, Func<string, double> selector)
        {
            double max = 0;

            string itemMax = "";

            foreach (var item in source)
            {
                double maxAux = selector.Invoke(item);

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
