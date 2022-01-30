using CoreModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TextAnalizator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var a = new DistrictAnalyzer(new District[] {new District("none") }, new Address[0]);
            await a.AnalyzeDistrict("правый");

            Console.WriteLine(Path.GetFullPath(@"..\..\..\ArticleAnalyzer\full7z-mlteast-ru.lem"));

            Console.WriteLine("Hello World!");
        }
    }
}
