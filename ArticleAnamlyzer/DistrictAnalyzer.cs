using Newtonsoft.Json;
using Python;
using System;
using CoreModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LemmaSharp.Classes;

namespace TextAnalizator
{
    public class DistrictAnalyzer
    {
        private readonly Dictionary<string, HashSet<string>> districts;
        private readonly Dictionary<string, HashSet<string>> lemmatizeDistricts;
        private readonly PythonExecutor pythonAnalyzer;
        private readonly string defaultDistrictName;
        private readonly Lemmatizer lemmatizer;
        public DistrictAnalyzer(Dictionary<string, HashSet<string>> districtNameByAddress,
            PythonExecutor pythonExecutor, Lemmatizer lemmatizer, string defaultDistrictName = "none")
        {
            this.defaultDistrictName = defaultDistrictName;
            districts = districtNameByAddress;
            pythonAnalyzer = pythonExecutor;
            this.lemmatizer = lemmatizer;
            lemmatizeDistricts = new Dictionary<string, HashSet<string>>();
            foreach(var p in districts)
            {
                var words = new HashSet<string>();
                lemmatizeDistricts[p.Key] = words;
                foreach(var word in p.Value)
                    words.Add(lemmatizer.Lemmatize(word));
            }
        }

        public async Task<string> AnalyzeDistrict(string text)
        {
            var res = await pythonAnalyzer.ExecuteScript(text);
            var districtsStatistics = new Dictionary<string, int>();
            var output = JsonConvert.DeserializeObject<ScriptResponse>(res);

            if(output == null)
                return defaultDistrictName;

            var addresName = new HashSet<string>();
            var lemAddresName = new HashSet<string>();

            if (output.Names != null)
            {
                foreach (var name in output.Names)
                    foreach (var n in name.Split(" "))
                    {
                        addresName.Add(n);
                        lemAddresName.Add(lemmatizer.Lemmatize(n));
                    }
            }

            if (output.Addresses != null)
                foreach (var addr in output.Addresses)
                    foreach (var n in addr.Value.Split(" "))
                    {
                        addresName.Add(n);
                        lemAddresName.Add(lemmatizer.Lemmatize(n));
                    }

            await FindKeyWord(addresName, districts, districtsStatistics,1);

            if(districtsStatistics.Any())
                return districtsStatistics.OrderBy(d => d.Value).Last().Key;

            await FindKeyWord(lemAddresName, lemmatizeDistricts, districtsStatistics, 1);

            if (districtsStatistics.Any())
                return districtsStatistics.OrderBy(d => d.Value).Last().Key;

            return defaultDistrictName;
        }

        private Task FindKeyWord(IEnumerable<string> words, Dictionary<string, HashSet<string>> districtAddress, Dictionary<string, int> districtProbably, int weight)
        {
            return Task.Run(() =>
            {
                foreach (var adr in words)
                {
                    foreach (var a in adr.Trim().Split(" "))
                    {
                        var word = lemmatizer.Lemmatize(a.ToLower());
                        foreach (var p in districtAddress)
                        {
                            if (p.Value.Contains(word))
                                if (districtProbably.ContainsKey(p.Key))
                                    districtProbably[p.Key] += weight;
                                else
                                    districtProbably[p.Key] = weight;
                        }
                    }
                }
            });
        }
    }
}
