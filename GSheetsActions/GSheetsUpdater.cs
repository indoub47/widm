using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GSheetsActions
{
    public class GSheetsUpdater
    {
        public BatchUpdateValuesResponse BatchUpdateSheet(
            string spreadsheetId, 
            SheetConfig sheetConfig, 
            SheetsService service)
        {
            // gaunami duomenys iš sheetų
            IList<IList<Object>> sheetValues = getRangeValues(spreadsheetId, sheetConfig.RangeAddress, service);

            List<ValueRange> requestData = new List<ValueRange>();
            string dateFormat = "yyyy-MM-dd";
            object valueToWrite = DateTime.Now.Date.ToString(dateFormat);

            // pridedami duomenys į requestData
            addDataToRequest(sheetValues, sheetConfig, valueToWrite, requestData);

            // sukuriamas batch update request
            BatchUpdateValuesRequest request = getUpdateValuesRequest(requestData);

            // updateinamas spreadsheet
            return updateSheet(request, spreadsheetId, service);
        }


        private static IList<IList<Object>> getRangeValues(
            string spreadsheetId, 
            string rangeAddress, 
            SheetsService service)
        {
            // gauna viso range duomenis, tam, kad sužinoti, kuriuos laukus reikia užpildyti
            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                    service.Spreadsheets.Values.Get(spreadsheetId, rangeAddress);

            ValueRange requestResponse = getRequest.Execute();
            IList<IList<Object>> responseValues = requestResponse.Values;
            return responseValues;
        }


        private static BatchUpdateValuesRequest getUpdateValuesRequest(
            List<ValueRange> requestData, 
            string valueInputOption = "USER_ENTERED")
        {
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                Data = requestData,
                ValueInputOption = valueInputOption
            };
            return requestBody;
        }


        private void addDataToRequest(
            IList<IList<Object>> rangeValues, 
            SheetConfig sheetConfig, 
            object valueToWrite, 
            List<ValueRange> requestData)
        {
            var updateValue = new List<object>() { valueToWrite };

            // tiems langeliams, kuriuose nėra datos, sukuria ValueRange
            for (int r = 0; r < rangeValues.Count; r++)
            {
                if (rangeValues[r].Count < sheetConfig.FilterColumnIndex + 1 ||
                    rangeValues[r][sheetConfig.FilterColumnIndex] == null ||
                    rangeValues[r][sheetConfig.FilterColumnIndex].ToString().Trim() == string.Empty)
                {
                    ValueRange vr = new ValueRange
                    {
                        Range = string.Format(sheetConfig.FilterAddressFormat, r + sheetConfig.StartRowIndex),
                        Values = new List<IList<object>> { updateValue }
                    };
                    requestData.Add(vr);
                }
            }
        }


        private static BatchUpdateValuesResponse updateSheet(
            BatchUpdateValuesRequest request, 
            string spreadsheetId, 
            SheetsService service)
        {
            return service.Spreadsheets.Values.BatchUpdate(request, spreadsheetId).Execute();
        }
    }
}
