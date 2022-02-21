using CoreModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TextAnalizator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var j = new JsonSerializerSettings();
            j.MissingMemberHandling = MissingMemberHandling.Ignore;

            var ser = JsonConvert.SerializeObject
                (
                    new List<Event> 
                    {
                        new Event() 
                        {
                            DateOfDownload = DateTime.Now, 
                            IncidentCategory="cp",
                            Title = "Title",
                            Link = "link",
                            Source = new Source()
                        } 
                    }
                );


            var des = JsonConvert.DeserializeObject<IEnumerable<Event>>(null, j);

            Console.WriteLine(des);

            /*var a = new DistrictAnalyzer(new District[] {new District("район называется Правым") }, new Address[0]);
            var res = await a.AnalyzeDistrict("правый");
            Console.WriteLine(res.DistrictName);*/
            /*
                        var dict = new Dictionary<string, Dictionary<string, double>>() 
                        {
                            { "пожар", new Dictionary<string, double>(){ {"огонь",1 } } },
                        };

                        var b = new WeightAnalizator(dict, "Не ЧП");
                        var res = await b.AnalizeCategoryAsync("произошел пожар, много огня");
                        Console.WriteLine(res);*/
        }
    }
}
