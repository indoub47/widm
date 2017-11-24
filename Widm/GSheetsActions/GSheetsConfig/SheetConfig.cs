using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Widm
{
    public class SheetConfig
    {
        public string SheetName { get; set; }
        public string RangeAddress { get; set; }
        public int StartRowIndex { get; set; }
        public int FilterColumnIndex { get; set; }
        public string FilterAddressFormat { get; set; }
        public string[] Mapping { get; set; }

        public static Dictionary<string, SheetConfig> GetConfig()
        {
            string jsonString = System.IO.File.ReadAllText("GSheetsActions/GSheetsConfig/" + Properties.GSheets.Default.SheetConfigData);
            return JsonConvert.DeserializeObject<Dictionary<string, SheetConfig>>(jsonString); 
        }
    }
}
