using Microsoft.VisualStudio.TestTools.UnitTesting;
using InspectionValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InspectionLib;


namespace InspectionValidation.Tests
{
    [TestClass()]
    public class ValidationMethodsTests
    {
        [TestMethod()]
        public void ValidatePirmTest()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("id", typeof(Int64));
            dataTable.Columns.Add("linija", typeof(String));
            dataTable.Columns.Add("kelias", typeof(Int32));
            dataTable.Columns.Add("km", typeof(Int32));
            dataTable.Columns.Add("pk", typeof(Int32));
            dataTable.Columns.Add("m", typeof(Int32));
            dataTable.Columns.Add("siule", typeof(Int32));
            dataTable.Columns.Add("skodas", typeof(String));
            dataTable.Columns.Add("data1", typeof(DateTime));
            dataTable.Columns.Add("data2", typeof(DateTime));
            dataTable.Columns.Add("data3", typeof(DateTime));
            dataTable.Columns.Add("data4", typeof(DateTime));


           

            //dataTable.Rows.Add(new object[] { 10, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15) });
            //dataTable.Rows.Add(new object[] { 12, "01", 1, 256, 3, 36, 9, "06.3", new DateTime(2016, 2, 18), new DateTime(2017, 3, 15) });
            //dataTable.Rows.Add(new object[] { 15632, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 7, 12) });

            object[][] rowData = {
            new object[] { 10, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15) },
            new object[] { 12, "01", 1, 256, 3, 36, 9, "06.3", new DateTime(2016, 2, 18), new DateTime(2017, 3, 15) },
            new object[] { 15632, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 7, 12) }
            };

            foreach (var dataArray in rowData)
            {
                DataRow row = dataTable.NewRow();
                row["id"] = Convert.ToInt64(dataArray[0]);
                row["linija"] = dataArray[1].ToString();
                row["kelias"] = Convert.ToInt32(dataArray[2]);
                row["km"] = Convert.ToInt32(dataArray[3]);
                row["pk"] = Convert.ToInt32(dataArray[4]);
                row["m"] = Convert.ToInt32(dataArray[5]);
                row["siule"] = Convert.ToInt32(dataArray[6]);
                row["skodas"] = dataArray[7].ToString();
                row["data1"] = Convert.ToDateTime(dataArray[8]);
                //row["data2"] = dataArray[0];
                //row["data3"] = dataArray[0];
                //row["data4"] = dataArray[0];
                //for (int c = 0; c < dataArray.Length; c++)
                //{
                //    row[c] = dataArray[c];
                //}
                dataTable.Rows.Add(row);
            }

            Insp insp = new Insp(null, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.First);
            IList<SuspInspInfo> susps = ValidationMethods.ValidatePirm(insp, dataTable);

            Assert.IsTrue(susps.Count == 1);


        }
    }
}