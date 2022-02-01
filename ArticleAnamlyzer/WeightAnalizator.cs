using LemmaSharp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TextAnalizator
{
    public class WeightAnalizator
    {
        private Dictionary<string, Dictionary<string, double>> categories;
        private readonly Lemmatizer lemmatizer;
        private readonly string defaultCategory;
        private readonly char[] splitSymbols = new char[] { ',', '.', ' ', '\'', '\"', '\t', '\n', '?', '!' };
        public WeightAnalizator(Dictionary<string, Dictionary<string, double>> categories, string defaultCategory)
        {
            this.defaultCategory = defaultCategory;
            this.categories = categories;
            var dataFilepath = @"..\..\..\full7z-mlteast-ru.lem";
            Console.WriteLine(Path.GetFullPath(dataFilepath));
            FileStream stream = File.OpenRead(dataFilepath);
            lemmatizer = new Lemmatizer(stream);
            stream.Close();
        }

        public Task<string> AnalizeCategoryAsync(string text)
        {
            return Task.Run(() =>
            {
                var result = new Dictionary<string, double>();

                var tokens = SplitTextToTokens(text);

                foreach (var token in tokens)
                {
                    var word = lemmatizer.Lemmatize(token);
                    foreach (var val in categories.Keys)
                    {
                        if (categories[val].ContainsKey(word))
                        {
                            var weight = categories[val][word];
                            if (result.ContainsKey(val))
                                result[val] += weight;
                            else
                                result[val] = weight;
                        }
                    }
                }

                if (!result.Keys.Any())
                    return defaultCategory;

                var resCat = result.OrderBy(val => val.Value).Last();
                return resCat.Value >= 1 ? resCat.Key : defaultCategory;
            });
        }


        private IEnumerable<string> SplitTextToTokens(string text)
        {
            return text.ToLower()
                .Split(splitSymbols)
                .Where(token => !(string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token)));
        }
    }
}
