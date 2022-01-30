using LemmaSharp.Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TextAnalizator
{
    public class Analyzer
    {
        private Dictionary<string, HashSet<string>> categories;
        private readonly Lemmatizer lemmatizer;
        private readonly string defaultCategory;
        private readonly char[] splitSymbols = new char[] { ',', '.', ' ', '\'', '\"', '\t', '\n', '?', '!' };
        public Analyzer(Dictionary<string, HashSet<string>> categories, string defaultCategory)
        {
            this.defaultCategory = defaultCategory;
            this.categories = categories;
            var dataFilepath = @"..\..\..\ArticleAnalyzer\full7z-mlteast-ru.lem";
            Debug.Print(Path.GetFullPath(dataFilepath));
            FileStream stream = File.OpenRead(dataFilepath);
            lemmatizer = new Lemmatizer(stream);
            stream.Close();
        }

        public Task<string> AnalizeCategoryAsync(string text)
        {
            return Task.Run(() =>
            {
                var result = new Dictionary<string, int>();

                var tokens = SplitTextToTokens(text);
                foreach (var token in tokens)
                {
                    foreach (var val in categories)
                    {
                        var word = lemmatizer.Lemmatize(token);
                        if (val.Value.Contains(word))
                        {
                            if (result.ContainsKey(val.Key))
                                result[val.Key]++;
                            else
                                result[val.Key] = 1;
                        }
                    }
                }

                if (!result.Keys.Any())
                    return defaultCategory;

                return result.OrderBy(val => val.Value).Last().Key;
            });
        }

        private IEnumerable<string> SplitTextToTokens(string text)
        {
            return text.ToLower()
                .Split(splitSymbols)
                .Where(token=>!(string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token)));
        }
    }
}
