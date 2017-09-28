using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;
using System.Collections.Generic;
using TagMaking;
using WidmShared;

namespace RecordValidationTest
{
    [TestClass]
    public class NepirmRecordValidatorTest
    {
        string[] nepirmiejiMapping = new string[] 
        {
          "id",
          "linija",
          "kelias",
          "km",
          "pk",
          "m",
          "siule",
          "skodas",
          "tdata",
          "kelintas",
          "aparatas",
        };

        [TestMethod]
        public void ValidateNepirmRecordTest()
        {
            NepirmiejiRecordTagMaker tagMaker = new NepirmiejiRecordTagMaker();
            NepirmRecordValidator validator = new NepirmRecordValidator(nepirmiejiMapping, tagMaker);
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "nepirmieji tikrinimai";

            IList<IList<object>> recs = new object[][]
            {
                //id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas
                new object[] { 12345, "01", 5, 325, 6, 92, 0, "06.4", "2017-09-15", 2, "831" }, // ok
                new object[] { 87654, "01", 4, 325, 6, 92, 0, "06.4", "2017-09-15", "papild", "831" }, // papild
                new object[] { 87654, "01", 4, 325, 6, 92, 0, "06.4", "2017-09-15", "1", "831" }, // kelintas 1
                new object[] { 789523, "01", 3, 325, 6, 92, 0, "06.4", DateTime.Now }, // no kelintas, no aparatas
                new object[] { }, // empty
                new object[] { 0, "01", 8, 3, 6, 12, 0, "06.4", DateTime.Now, 5 } // 0 id, 5 kelintas, aparatas
            };

            List<InvalidInfo> invalidList;

            invalidList = validator.ValidateRecord(recs[0], ctx);
            Assert.IsTrue(invalidList.Count == 0, "recs[0] count");

            invalidList = validator.ValidateRecord(recs[1], ctx);
            Assert.IsTrue(invalidList.Count == 0, "recs[1] count");

            invalidList = validator.ValidateRecord(recs[2], ctx);
            Assert.IsTrue(invalidList.Count == 1, "recs[2] count");

            invalidList = validator.ValidateRecord(recs[3], ctx);
            Assert.IsTrue(invalidList.Count == 2, "recs[3] count");

            invalidList = validator.ValidateRecord(recs[4], ctx);
            Assert.IsTrue(invalidList.Count == 9, "recs[4] count");

            invalidList = validator.ValidateRecord(recs[5], ctx);
            Assert.IsTrue(invalidList.Count == 3, "recs[5] count");
        }
    }
}
