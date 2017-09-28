using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSheetsActions
{
    public class SheetConfig
    {
        public string SheetName { get; set; }
        public string RangeAddress { get; set; }
        public int StartRowIndex { get; set; }
        public int FilterColumnIndex { get; set; }
        public string FilterAddressFormat { get; set; }
        public string[] Mapping { get; set; }
    }

    
    // Dictionary<string, SheetConfig> sheetDataDic = JsonConvert.DeserializeObject<Dictionary<string, SheetConfig>>(jsonString);
}
