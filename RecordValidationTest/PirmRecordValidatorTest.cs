using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;
using System.Collections.Generic;
using WidmShared;
using TagMaking;

namespace RecordValidationTest
{
    [TestClass]
    public class PirmRecordValidatorTest
    {
        string[] pirmiejiMapping = new string[]
        {
          "linija",
          "kelias",
          "km",
          "pk",
          "m",
          "siule",
          "skodas",
          "suvirino",
          "tdata",
          "aparatas",
          "ivesta_db"
        };

        [TestMethod]
        public void ValidatePirmRecordTest()
        {
            PirmRecValidator validator = new PirmRecValidator(pirmiejiMapping, new PirmRecTagMaker());
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";
            string earlierDate = DateTime.Now.AddDays(-5).ToShortDateString();
            string wrongFormatEarlierDate = DateTime.Now.AddDays(-5).ToString("MM-dd-yyyy");

            IList<IList<object>> recs = new object[][]
            {
                //linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
                new object[] { "01", 5, 325, 6, 92, 0, "06.4", "GTC", earlierDate, "831" },
                new object[] { "01", -1, 325, 6, 92, 0, "06.4", "GTC", earlierDate, "831" },
                new object[] { "01", -1, 325, 6, 92, 0, "06.4", "GTC", wrongFormatEarlierDate },
                new object[] { },
                new object[] { "01", 8, 3, 6, 12, 0, "06.4", "GTC", wrongFormatEarlierDate, 831 }
            };

            List<InvalidInfo> invalidList;

            invalidList = validator.ValidateRecord(recs[0], ctx);
            Assert.IsTrue(invalidList.Count == 0, "recs[0] count");
            
            invalidList = validator.ValidateRecord(recs[1], ctx);
            Assert.IsTrue(invalidList.Count == 1, "recs[1] count");
            Assert.IsTrue(invalidList[0]["operatorId"].ToString() == "402", "recs[1] context objects[1]");

            invalidList = validator.ValidateRecord(recs[2], ctx);
            Assert.IsTrue(invalidList.Count == 3, "recs[2] count");

            invalidList = validator.ValidateRecord(recs[3], ctx);
            Assert.IsTrue(invalidList.Count == 8, "recs[3] count");

            invalidList = validator.ValidateRecord(recs[4], ctx);
            Assert.IsTrue(invalidList.Count == 3, "recs[4] count");
        }
    }
}
