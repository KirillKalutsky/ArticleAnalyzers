using CoreModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using LemmaSharp.Classes;
using Python;
using System.Linq;

namespace TextAnalizator
{
    class Program
    {
        static async Task Main(string[] args)
        {

           
            var python = new PythonExecutor(@"D:\anaconda\python.exe", @"D:\ВКР\Soft\ArticleAnamlyzer\ArticleAnamlyzer\1.py");
            //var district = new Dictionary<string, Dictionary<string, double>>();

            var dataFilepath = @"D:\ВКР\Soft\ArticleAnamlyzer\ArticleAnamlyzer\full7z-mlteast-ru.lem";
            Console.WriteLine(Path.GetFullPath(dataFilepath));
            FileStream stream = File.OpenRead(dataFilepath);
            var lemmatizer = new Lemmatizer(stream);
            stream.Close();
            var districtsNameByAddress = new Dictionary<string, HashSet<string>>();

            var path = "D:\\ВКР\\streets_list_with_districts_update.csv";
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    var street = values[1].ToLower().Trim().Split(" ");

                    string districtName;
                    var distr = values[2].Split(" ");
                    if (distr.Length == 2)
                        districtName = distr[1].ToLower().Trim();
                    else
                        districtName = values[2].ToLower().Trim();

                    if (!districtsNameByAddress.ContainsKey(districtName))
                        districtsNameByAddress[districtName] = new HashSet<string>() { districtName };

                    foreach (var s in street)
                    {
                        //var word = lemmatizer.Lemmatize(s);
                        districtsNameByAddress[districtName].Add(s);
                    }

                }
            }

            var text = "Днем 9 марта в Екатеринбурге на улице Щербакова сгорел пассажирский автобус «Богдан», который шел по маршруту №083. Пассажиры успели покинуть транспортное средство." +
                "Как сообщили в ГУ МЧС России по Свердловской области, площадь пожара составила 20 квадратных метров. С огнем справились за 5 минут.В тушении возгорания были задействованы две единицы техники и 11 человек личного состава." +
                "В администрации Екатеринбурга сообщили, что в салоне автобуса находилось 12 пассажиров.Именно они сообщили водителю о запахе гари.Водитель остановил автобус и высадил пассажиров." +
                "Потушить возгорание автобуса первичными средствами не удалось.Пострадавших нет." +
                "Ранее «МК - Урал» сообщал, что 6 декабря 2021 года в Екатеринбурге на перекрестке улиц Большакова и 8 Марта загорелся пассажирский автобус. Водитель и пассажиры успели покинуть транспортное средство." +
                "В ГУ МЧС России по Свердловской области рассказали, что площадь пожара составила 4 квадратных метра, был поврежден моторный отсек автобуса «МАЗ».";

            var districtAnalyzer = new DistrictAnalyzer(districtsNameByAddress, python, lemmatizer);

            var result = await districtAnalyzer.AnalyzeDistrict(text);

            Console.WriteLine(result);
        }
    }
}
