using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;
using System.Collections.Generic;

namespace RecordValidationTest
{
    [TestClass]
    public class PirmRecordValidatorTest
    {
        [TestMethod]
        public void ValidatePirmRecordTest()
        {
            PirmRecordValidator validator = new PirmRecordValidator();
            RecordContext ctx = new RecordContext(42, true, "meaning of life");

            IList<IList<object>> recs = new object[][]
            {
                //linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
                new object[] { "01", 5, 325, 6, 92, 0, "06.4", "GTC", "2017-09-15", "831" },
                new object[] { "01", -1, 325, 6, 92, 0, "06.4", "GTC", "2017-09-15", "831" },
                new object[] { "01", -1, 325, 6, 92, 0, "06.4", "GTC", "09-14-2017" },
                new object[] { },
                new object[] { "01", 8, 3, 6, 12, 0, "06.4", "GTC", "09-14-2017", 831 }
            };

            List<InvalidDataInfo> badDataList;

            badDataList = validator.ValidateRecord(recs[0], ctx);
            Assert.IsTrue(badDataList.Count == 0, "recs[0] count");
            
            badDataList = validator.ValidateRecord(recs[1], ctx);
            Assert.IsTrue(badDataList.Count == 1, "recs[1] count");
            Assert.IsTrue((bool) badDataList[0].Context.Objects[1], "recs[1] context objects[1]");

            badDataList = validator.ValidateRecord(recs[2], ctx);
            Assert.IsTrue(badDataList.Count == 3, "recs[2] count");

            badDataList = validator.ValidateRecord(recs[3], ctx);
            Assert.IsTrue(badDataList.Count == 8, "recs[3] count");

            badDataList = validator.ValidateRecord(recs[4], ctx);
            Assert.IsTrue(badDataList.Count == 3, "recs[4] count");
        }
    }
}
