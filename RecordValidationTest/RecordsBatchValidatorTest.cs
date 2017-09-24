using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;
using System.Collections.Generic;
using WidmShared;
using TagMaking;

namespace RecordValidationTest
{
    [TestClass]
    public class RecordsBatchValidatorTest
    {
        [TestMethod]
        public void TestBatchPirmValidatorAllRecsOK()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            PirmRecordValidator pirmValidator = new PirmRecordValidator(new PirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(pirmValidator);
            batchValidator.Context = ctx;

            List<IList<object>> bunchOfRecords = new List<IList<object>>
            {
                // linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
                new object[]{"01", "3", "254", "9", 80, 9, "06.3", "IF4", DateTime.Now, "831"},
                new object[]{"23", 1, 25, 3, 15, 0, "06.4", "GTC", DateTime.Now.AddDays(-3), "807"},
                new object[]{"47", 4, "10", 12, 82, 0, "06.3", "IF4", DateTime.Now.AddDays(-2), "831"},
                new object[]{"22", 9, 3, 12, 82, "", "06.4", "VitrasS", DateTime.Now.AddDays(-1), "831"},
                new object[]{"52", 8, "10", 0, 12, "  ", "06.4", "IF4", DateTime.Now.AddDays(-2), "831"}
            };

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(0, idList.Count);
        }


        [TestMethod]
        public void TestBatchPirmValidatorFewRecsNotOK()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            PirmRecordValidator pirmValidator = new PirmRecordValidator(new PirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(pirmValidator);
            batchValidator.Context = ctx;

            List<IList<object>> bunchOfRecords = new List<IList<object>>
            {
                // linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
                new object[]{"01", "3", "254", "9", 80, 9, "06.3", "IF4"}, // data, aparatas
                new object[]{"23", 1, 25, 3, 15, 0, "06.2", "GTC", DateTime.Now.AddDays(3), "807"}, // sąlyg.kodas, data
                new object[]{"47", 4, "10", 12, 82, 0, "06.3", "IF4", DateTime.Now.AddDays(-2), "831"}, // ok
                new object[]{"27", 9, 3, 12, 82, "", "06.4", "VitrasS", DateTime.Now.AddDays(-1), "831"}, // linija
                new object[]{"52", 8, "10", 0, 12, 9, "06.4", "IF4", DateTime.Now.AddDays(-2), "831"} // siule iešme
            };

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(6, idList.Count);
        }


        [TestMethod]
        public void TestBatchPirmValidatorEmptyList()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            PirmRecordValidator pirmValidator = new PirmRecordValidator(new PirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(pirmValidator);
            batchValidator.Context = ctx;

            List<IList<object>> bunchOfRecords = new List<IList<object>>{};

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(0, idList.Count);
        }

        [TestMethod]
        public void TestBatchNepirmAllRecsOK()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            NepirmRecordValidator nepirmValidator = new NepirmRecordValidator(new NepirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(nepirmValidator);
            batchValidator.Context = ctx;

            // id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas

            List<IList<object>> bunchOfRecords = new List<IList<object>>
            {
                new object[]{123, "01", "3", "254", "9", 80, 9, "06.3", DateTime.Now, 2, "831"},
                new object[]{"123", "23", 1, 25, 3, 15, 0, "06.4", DateTime.Now.AddDays(-3), 3, "807"},
                new object[]{"999999", "47", 4, "10", 12, 82, 0, "06.3", DateTime.Now.AddDays(-2), "4", "831"},
                new object[]{1, "22", 9, 3, 12, 82, "", "06.4", DateTime.Now.AddDays(-1), "papild", "831"},
                new object[]{"1", "52", 8, "10", 0, 12, "  ", "06.4", DateTime.Now.AddDays(-2), "  2", "831"}
            };

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(0, idList.Count);
        }


        [TestMethod]
        public void TestBatchNepirmFewRecsNotOK()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            NepirmRecordValidator nepirmValidator = new NepirmRecordValidator(new NepirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(nepirmValidator);
            batchValidator.Context = ctx;

            List<IList<object>> bunchOfRecords = new List<IList<object>>
            {
                // id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas
                new object[]{123, "01", "3", "254", "9", 80, 9, "06.3"}, // data, kelintas, aparatas
                new object[]{"123", "23", 1, 25, 3, 15, 0, "06.2", DateTime.Now.AddDays(3), 3, "807"}, // sąlyg.kodas, data
                new object[]{"999999", "47", 4, "10", 12, 82, 0, "06.3", DateTime.Now.AddDays(-2), "1", "831"}, // pirmas
                new object[]{1, "27", 9, 3, 12, 82, "", "06.4", DateTime.Now.AddDays(-1), "papild", "831"}, // linija
                new object[]{"1", "52", 8, "10", 0, 12, 9, "06.4", DateTime.Now.AddDays(-2), "   2", "831"} // siule iešme netikrinama
            };

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(7, idList.Count);
        }


        [TestMethod]
        public void TestBatchNepirmEmptyList()
        {
            Dictionary<string, object> ctx = new Dictionary<string, object>();
            ctx["operatorId"] = "402"; ctx["sheetName"] = "pirmieji tikrinimai";

            NepirmRecordValidator nepirmValidator = new NepirmRecordValidator(new NepirmiejiRecordTagMaker());

            BatchRecordValidator batchValidator = new BatchRecordValidator(nepirmValidator);
            batchValidator.Context = ctx;

            List<IList<object>> bunchOfRecords = new List<IList<object>>{};

            List<InvalidInfo> idList = batchValidator.ValidateBatch(bunchOfRecords);
            Assert.AreEqual(0, idList.Count);
        }
    }
}
