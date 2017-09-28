using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSheetsActions
{
    public class SpreadSheetConfig
    {
        public string Id { get; set; }
        public string OperatorId { get; set; }
        public string[] SheetGids { get; set; }
    }
     
    // SpreadSheetConfig[] spreadSheets = JsonConvert.DeserializeObject<SpreadSheetConfig[]>(jsonString);
}
