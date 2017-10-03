using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GSheetsActions
{
    public class SpreadSheetConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OperatorId { get; set; }

        public static SpreadSheetConfig[] GetConfig()
        {
            string jsonString = System.IO.File.ReadAllText("GSheetsConfig/" + Properties.Settings.Default.SpreadsheetsConfigData);
            return JsonConvert.DeserializeObject<SpreadSheetConfig[]>(jsonString);
        }
    }
}
