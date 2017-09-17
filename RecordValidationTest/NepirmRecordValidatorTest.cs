using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;
using System.Collections.Generic;

namespace RecordValidationTest
{
    [TestClass]
    public class NepirmRecordValidatorTest
    {
        [TestMethod]
        public void ValidateNepirmRecordTest()
        {
            NepirmRecordValidator validator = new NepirmRecordValidator();
            RecordContext ctx = new RecordContext(42, true, "meaning of life");

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

            List<InvalidDataInfo> badDataList;

            badDataList = validator.ValidateRecord(recs[0], ctx);
            Assert.IsTrue(badDataList.Count == 0, "recs[0] count");

            badDataList = validator.ValidateRecord(recs[1], ctx);
            Assert.IsTrue(badDataList.Count == 0, "recs[1] count");

            badDataList = validator.ValidateRecord(recs[2], ctx);
            Assert.IsTrue(badDataList.Count == 1, "recs[2] count");

            badDataList = validator.ValidateRecord(recs[3], ctx);
            Assert.IsTrue(badDataList.Count == 2, "recs[3] count");

            badDataList = validator.ValidateRecord(recs[4], ctx);
            Assert.IsTrue(badDataList.Count == 9, "recs[4] count");

            badDataList = validator.ValidateRecord(recs[5], ctx);
            Assert.IsTrue(badDataList.Count == 3, "recs[5] count");
        }
    }
}
