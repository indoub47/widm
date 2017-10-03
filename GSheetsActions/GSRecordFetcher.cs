using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using WidmShared;

namespace GSheetsActions
{
    public class GSRecordFetcher : IRecFetcher
    {
        private string spreadsheetId;
        private SheetConfig sheetConfig;
        private SheetsService service;


        public void Initialize(string spreadsheetId, SheetConfig sheetConfig, SheetsService service)
        {
            this.spreadsheetId = spreadsheetId;
            this.sheetConfig = sheetConfig;
            this.service = service;
        }

        public List<IList<object>> Fetch()
        {
            if (spreadsheetId == string.Empty || 
                sheetConfig == null || 
                service == null)
            {
                throw new InvalidOperationException("GSRecordFetcher is uninitialized");
            }

            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, sheetConfig.RangeAddress);

            ValueRange response = request.Execute();
            IList<IList<Object>> allRecords = response.Values;

            if (allRecords == null)
                throw new Exception("Duomenų parsisiuntimo iš Google Sheets rezultatas - null.");

            // nufiltruojami tie, kurių įvedimo į db data nelygi null
            List<IList<Object>> list = allRecords.ToList<IList<Object>>();
            list.RemoveAt(0);
            return list.Where(x => x.Count <= sheetConfig.FilterColumnIndex || x[sheetConfig.FilterColumnIndex] == null).ToList<IList<Object>>();
        }

        public string[] GetMapping()
        {
            return sheetConfig.Mapping;
        }
    }
}
