using Microsoft.VisualStudio.TestTools.UnitTesting;
using InspPoolCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InspectionLib;

namespace InspPoolCommunication.Tests
{
    [TestClass()]
    public class OleDbInspPoolCommunicatorTests
    {
        OleDbInspPoolCommunicator communicator;

        [TestInitialize()]
        public void Initialize()
        {
            communicator = new OleDbInspPoolCommunicator(
                InspPoolCommunicationTests.Properties.Settings.Default.TestDbPath);
        }

        [TestMethod()]
        public void FetchByIdTest_FetchExisting()
        {
            Insp insp = new Insp(17085, "17", 1, 81, 8, 16, 9, "06.4", "421", "831", new DateTime(2017, 9, 22), "GTC", Kelintas.First);
            DataTable tbl = communicator.FetchById(insp);
            Assert.IsTrue(tbl.Rows.Count == 1, "rows count");
            DataRow row = tbl.Rows[0];
            Assert.IsTrue(Convert.ToInt64(row["id"]) == insp.Id, "fetched record id");
        }

        [TestMethod()]
        public void FetchByIdTest_FetchNotExisting()
        {
            Insp insp = new Insp(16916, "17", 1, 81, 8, 16, 9, "06.4", "421", "831", new DateTime(2017, 9, 22), "GTC", Kelintas.First);
            DataTable tbl = communicator.FetchById(insp);
            Assert.IsTrue(tbl.Rows.Count == 0, "rows count");
        }

        [TestMethod()]
        public void FetchByVietaTest_ExistsOne()
        {
            Insp insp = new Insp(null, "17", 1, 81, 8, 16, 9, "06.4", "421", "831", new DateTime(2017, 9, 22), "GTC", Kelintas.First);
            DataTable tbl = communicator.FetchByVieta(insp);
            Assert.IsTrue(tbl.Rows.Count == 1, "rows count");
            DataRow row = tbl.Rows[0];
            Assert.IsTrue(Convert.ToInt64(row["id"]) == 16946, "fetched record id");
        }

        [TestMethod()]
        public void FetchByVietaTest_ExistSeveral()
        {
            Insp insp = new Insp(null, "17", 1, 81, 8, 15, 9, "06.4", "421", "831", new DateTime(2017, 9, 22), "GTC", Kelintas.First);
            DataTable tbl = communicator.FetchByVieta(insp);
            Assert.IsTrue(tbl.Rows.Count == 3, "rows count");
        }

        [TestMethod()]
        public void FetchByVietaTest_ExistsNoone()
        {
            Insp insp = new Insp(null, "12", 1, 81, 8, 15, 9, "06.4", "421", "831", new DateTime(2017, 9, 22), "GTC", Kelintas.First);
            DataTable tbl = communicator.FetchByVieta(insp);
            Assert.IsTrue(tbl.Rows.Count == 0, "rows count");
        }

        [TestMethod()]
        public void InsertInspTest()
        {
            Insp insp = new Insp(null, "01", 1, 1, 1, 1, 1, "06.4", "421", "831", DateTime.Now.Date, "GTC", Kelintas.First, "inserted by InsertInspTest()");
            int result = communicator.InsertInsp(insp);
            Assert.AreEqual(1, result, "insert count");
        }

        [TestMethod()]
        public void BatchInsertInspTest()
        {
            Insp[] insps = new Insp[]
            {
                new Insp(null, "02", 2, 2, null, 2, 2, "06.4", "421", "831", DateTime.Now.Date, "GTC", Kelintas.First, "inserted by BatchInsertInspTest()"),
                new Insp(null, "03", 3, 3, 3, 3, null, "06.4", "421", "831", DateTime.Now.Date, "GTC", Kelintas.First, "inserted by BatchInsertInspTest()"),
                new Insp(null, "03", 3, 3, 3, 3, 3, "06.4", "421", "831", DateTime.Now.Date, "GTC", Kelintas.First, "inserted by BatchInsertInspTest()"),
                new Insp(null, "04", 4, 4, null, 4, null, "06.4", "421", "831", DateTime.Now.Date, "GTC", Kelintas.First, "inserted by BatchInsertInspTest()")
            };
            int result = communicator.BatchInsertInsp(insps);
            Assert.AreEqual(4, result, "insert count");
        }

        [TestMethod()]
        public void BatchInsertInspTest_EmptyList()
        {
            List<Insp> list = new List<Insp>();
            int result = communicator.BatchInsertInsp(list);
            Assert.AreEqual(0, result, "insert count");
        }

        [TestMethod()]
        public void UpdateInspInfoTest()
        {
            // second - 14427
            // third - 14388
            // fourth - 12165
            // extra - 8438

            Insp insp = new Insp(14427, "01", 1, 1, 1, 1, 1, "06.4", "402", "802", DateTime.Now.Date, "GTC", Kelintas.Second, "updated 2 by UpdateInspInfoTest()");
            int result = communicator.UpdateInspInfo(insp);
            Assert.AreEqual(1, result, "update second");


            insp = new Insp(14388, "01", 1, 1, 1, 1, 1, "06.4", "403", "803", DateTime.Now.Date, "GTC", Kelintas.Third, "updated 3 by UpdateInspInfoTest()");
            result = communicator.UpdateInspInfo(insp);
            Assert.AreEqual(1, result, "update third");


            insp = new Insp(12165, "01", 1, 1, 1, 1, 1, "06.4", "404", "804", DateTime.Now.Date, "GTC", Kelintas.Fourth, "updated 4 by UpdateInspInfoTest()");
            result = communicator.UpdateInspInfo(insp);
            Assert.AreEqual(1, result, "update fourth");


            insp = new Insp(8438, "01", 1, 1, 1, 1, 1, "06.4", "400", "800", DateTime.Now.Date, "GTC", Kelintas.Extra, "updated extra by UpdateInspInfoTest()");
            result = communicator.UpdateInspInfo(insp);
            Assert.AreEqual(1, result, "update extra");
        }

        [TestMethod()]
        public void BatchUpdateInspTest()
        {
            // second - 14428, 14370, 14993
            // third - 14447, 14389, 14448
            // fourth - 12156, 12158, 12159
            // extra - 10254, 8606, 14069

            Insp[] insps = new Insp[]
            {
                new Insp(14428, "02", 2, 2, 2, 2, 2, "06.4", "422", "822", DateTime.Now.Date, "GTC", Kelintas.Second, "updated 2 by BatchUpdateInspTest()"),
                new Insp(14447, "03", 3, 3, 3, 3, 3, "06.4", "433", "833", DateTime.Now.Date, "GTC", Kelintas.Third, "updated 3 by BatchUpdateInspTest()"),
                new Insp(12156, "04", 4, 4, 4, 4, 4, "06.4", "444", "844", DateTime.Now.Date, "GTC", Kelintas.Fourth, "updated 4 by BatchUpdateInspTest()"),
                new Insp(8606, "05", 5, 5, 5, 5, 5, "06.4", "400", "800", DateTime.Now.Date, "GTC", Kelintas.Extra, "updated extra by BatchUpdateInspTest()"),
                new Insp(14389, "03", 3, 3, 3, 3, 3, "06.4", "433", "833", DateTime.Now.Date, "GTC", Kelintas.Third, "updated 3 by BatchUpdateInspTest()"),
                new Insp(14448, "03", 3, 3, 3, 3, 3, "06.4", "433", "833", DateTime.Now.Date, "GTC", Kelintas.Third, "updated 3 by BatchUpdateInspTest()"),
                new Insp(12158, "04", 4, 4, 4, 4, 4, "06.4", "444", "844", DateTime.Now.Date, "GTC", Kelintas.Fourth, "updated 4 by BatchUpdateInspTest()"),
                new Insp(14370, "02", 2, 2, 2, 2, 2, "06.4", "422", "822", DateTime.Now.Date, "GTC", Kelintas.Second, "updated 2 by BatchUpdateInspTest()"),
                new Insp(10254, "05", 5, 5, 5, 5, 5, "06.4", "400", "800", DateTime.Now.Date, "GTC", Kelintas.Extra, "updated extra by BatchUpdateInspTest()"),
                new Insp(14993, "02", 2, 2, 2, 2, 2, "06.4", "422", "822", DateTime.Now.Date, "GTC", Kelintas.Second, "updated 2 by BatchUpdateInspTest()"),
                new Insp(14069, "05", 5, 5, 5, 5, 5, "06.4", "400", "800", DateTime.Now.Date, "GTC", Kelintas.Extra, "updated extra by BatchUpdateInspTest()"),
                new Insp(12159, "04", 4, 4, 4, 4, 4, "06.4", "444", "844", DateTime.Now.Date, "GTC", Kelintas.Fourth, "updated 4 by BatchUpdateInspTest()")
            };
            int result = communicator.BatchUpdateInsp(insps);
            // 14993 doesn't exist
            Assert.AreEqual(11, result, "update count");
        }
    }
}