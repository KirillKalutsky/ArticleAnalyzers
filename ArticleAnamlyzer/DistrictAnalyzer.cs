using Newtonsoft.Json;
using Python;
using System;
using CoreModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextAnalizator
{
    public class DistrictAnalyzer
    {
        private readonly Dictionary<string, District> districts;
        private readonly PythonExecutor pythonAnalyzer;
        private readonly string defaultDistrictName;
        public DistrictAnalyzer(IEnumerable<District> districts, IEnumerable<Address> addresses, string defaultDistrictName = "none")
        {
            this.defaultDistrictName = defaultDistrictName;
            this.districts = new Dictionary<string, District>();
            foreach (var district in districts)
                this.districts[district.DistrictName] = district;
            foreach (var adr in addresses)
                this.districts[adr.AddressName] = adr.District;
            pythonAnalyzer = new PythonExecutor(@"D:\anaconda\python.exe", Path.GetFullPath(@"..\..\..\1.py"));
        }

        public async Task<District> AnalyzeDistrict(string text)
        {
            var res = await pythonAnalyzer.ExecuteScript(text);

            var output = JsonConvert.DeserializeObject<ScriptResponse>(res);

            if(output == null)
                return districts[defaultDistrictName];

            if (output.Names != null)
            {
                foreach (var name in output.Names)
                {
                    var nameL = name.ToLower();
                    if (districts.ContainsKey(nameL))
                        return districts[nameL];
                }
            }

            if (output.Addresses != null)
            {
                foreach (var adr in output.Addresses)
                {
                    var adrName = adr.Value.ToLower();
                    if (districts.ContainsKey(adrName))
                        return districts[adrName];
                }
            }

            return districts["none"];
        }
    }
}
