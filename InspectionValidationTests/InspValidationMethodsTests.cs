using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InspectionLib;
using InspectionValidation;
using WidmShared;

namespace InspectionValidation.Tests
{
    [TestClass()]
    public class ValidationMethodsTests
    {
        private static DataTable db;

        [TestCleanup()]
        public void Cleanup()
        {
            db.Rows.Clear();
        }


        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            db = new DataTable();
            db.Columns.Add("id", typeof(Int64));
            db.Columns.Add("linija", typeof(String));
            db.Columns.Add("kelias", typeof(Int32));
            db.Columns.Add("km", typeof(Int32));
            db.Columns.Add("pk", typeof(Int32));
            db.Columns.Add("m", typeof(Int32));
            db.Columns.Add("siule", typeof(Int32));
            db.Columns.Add("skodas", typeof(String));
            db.Columns.Add("data1", typeof(DateTime));
            db.Columns.Add("data2", typeof(DateTime));
            db.Columns.Add("data3", typeof(DateTime));
            db.Columns.Add("data4", typeof(DateTime));
        }


        [TestMethod()]
        public void InspValidation_ValidatePirmTest_TaPatiVieta()
        {
            Insp insp = new Insp(null, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.First);
            db.Rows.Add(1, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);
            db.Rows.Add(2, "01", 1, 256, 3, 36, 0, "06.3", new DateTime(2016, 2, 18), new DateTime(2017, 3, 15), null, null);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidatePirm(insp, db).Cast<SuspInspInfo>().ToList();

            Assert.IsTrue(susps.Count == 1);
        }


        [TestMethod()]
        public void InspValidation_ValidatePirmTest_NoTaPatiVieta()
        {
            Insp insp = new Insp(null, "01", 1, 312, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.First);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidatePirm(insp, db);

            Assert.IsTrue(susps.Count == 0);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Nepirmasis tikrinimas tikrinamas kaip pirmasis")]
        public void InspValidation_ValidatePirm_NepirmKaipPirm()
        {
            Insp insp = new Insp(null, "01", 1, 312, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidatePirm(insp, db);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "Nepirmasis tikrinimas tikrinamas kaip pirmasis")]
        public void InspValidation_ValidateNepirm_PirmKaipNepirm()
        {
            Insp insp = new Insp(null, "01", 1, 312, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.First);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidateNepirm(insp, db);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "DB keli įrašai su tuo pačiu ID")]
        public void InspValidation_ValidateNepirm_KeliIrasai()
        {
            db.Rows.Add(1, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);
            db.Rows.Add(2, "01", 1, 256, 3, 36, 0, "06.3", new DateTime(2016, 2, 18), new DateTime(2017, 3, 15), null, null);
            Insp insp = new Insp(null, "01", 1, 312, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.First);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidateNepirm(insp, db);
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_Nerastas()
        {
            Insp insp = new Insp(null, "01", 1, 312, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);

            IList<SuspInspInfo> susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("toks įrašas nerastas ir jo duomenys negali būti pakeisti", susps[0].Message);
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_VietaDoesntAgree()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);

            insp = new Insp(4, "01", 1, 256, 3, 36, 0, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "siule");

            insp = new Insp(4, "01", 2, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "kelias");

            insp = new Insp(4, "01", 1, 255, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "km");

            insp = new Insp(4, "01", 1, 256, 1, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "pk");

            insp = new Insp(4, "1", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "linija");

            insp = new Insp(4, "01", 1, 256, 3, 33, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "m");

            insp = new Insp(4, "23", 1, 15, 3, 33, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("neatitinka vietos kodas", susps[0].Message, "linija, km, m");
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_SkodaiDontAgree()
        {
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);

            Insp insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.3", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            IList<SuspInspInfo> susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1);
            Assert.AreEqual<string>("esantis sąlyginis kodas", susps[0].Message.Substring(0, 23));
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_AlreadyInspected()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            // tikrinamas II
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "2");
            Assert.AreEqual<string>("2 tikrinimas jau atliktas", susps[0].Message, "II");

            // tikrinamas III
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), new DateTime(2017, 09, 16), null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "3");
            Assert.AreEqual<string>("3 tikrinimas jau atliktas", susps[0].Message, "III");

            // tikrinamas IV
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), new DateTime(2017, 09, 16), new DateTime(2017, 09, 22));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 18), "GTC", Kelintas.Fourth);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "4");
            Assert.AreEqual<string>("4 tikrinimas jau atliktas", susps[0].Message, "IV");

            // tikrinamas Extra
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Extra);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 0, "Extra");
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_FormerIsntDone()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            // tikrinamas II
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", null, null, null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "2");
            Assert.AreEqual<string>("neatliktas ankstesnis patikrinimas (data1 tuščias)", susps[0].Message, "II");

            // tikrinamas III
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), null, null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "3");
            Assert.AreEqual<string>("neatliktas ankstesnis patikrinimas (data2 tuščias)", susps[0].Message, "III");

            // tikrinamas IV
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 18), "GTC", Kelintas.Fourth);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "4");
            Assert.AreEqual<string>("neatliktas ankstesnis patikrinimas (data3 tuščias)", susps[0].Message, "IV");
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_LaterIsDone()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            // tikrinamas II - xoxo
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 15), null, new DateTime(2017, 9, 15), null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count II - xoxo");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data3 netuščias)", susps[0].Message, "II - xoxo");

            // tikrinamas II - xoxx
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 15), null, new DateTime(2017, 9, 15), new DateTime(2017, 9, 20));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count II - xoxx");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data3 netuščias)", susps[0].Message, "II - xoxx");

            // tikrinamas II - ooxo
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", null, null, new DateTime(2017, 9, 15), null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 2, "count II - ooxo");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data3 netuščias)", susps[1].Message, "II - ooxo");

            // tikrinamas II - ooxx
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", null, null, new DateTime(2017, 9, 15), new DateTime(2017, 9, 20));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 2, "count II - ooxx");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data3 netuščias)", susps[1].Message, "II - ooxx");

            // tikrinamas III - ooox
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", null, null, null, new DateTime(2017, 9, 15));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 2, "count III - ooox");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data4 netuščias)", susps[1].Message, "III - ooox");

            // tikrinamas III - xoox
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), null, null, new DateTime(2017, 9, 20));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 2, "count III - xoox");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data4 netuščias)", susps[1].Message, "III - xoox");


            // tikrinamas III - xxox
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 2, 1), null, new DateTime(2017, 9, 15));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count III - xxox");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data4 netuščias)", susps[0].Message, "III - xxox");

            // tikrinamas III - oxox
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 1), new DateTime(2017, 3, 15), null, new DateTime(2017, 9, 15));
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 9, 18), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count III - oxox");
            Assert.AreEqual<string>("įrašyta, kad jau atliktas vėlesnis patikrinimas (data4 netuščias)", susps[0].Message, "IV");
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_FormerIsLater()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            // tikrinamas II
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 5, 15), null, null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 5, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count II");
            Assert.AreEqual<string>("ankstesnis patikrinimas data1 atliktas vėliau", susps[0].Message.Substring(0, 45), "message II");

            // tikrinamas III
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 5, 15), new DateTime(2017, 6, 15), null, null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 6, 12), "GTC", Kelintas.Third);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count III");
            Assert.AreEqual<string>("ankstesnis patikrinimas data2 atliktas vėliau", susps[0].Message.Substring(0, 45), "message III");

            // tikrinamas IV
            db.Rows.Clear();
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 5, 15), new DateTime(2017, 6, 15), new DateTime(2017, 7, 15), null);
            insp = new Insp(4, "01", 1, 256, 3, 36, 9, "06.4", "428", "829", new DateTime(2017, 7, 12), "GTC", Kelintas.Fourth);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 1, "count IV");
            Assert.AreEqual<string>("ankstesnis patikrinimas data3 atliktas vėliau", susps[0].Message.Substring(0, 45), "message IV");
        }


        [TestMethod()]
        public void InspValidation_ValidateNepirm_Multiple()
        {
            Insp insp;
            IList<SuspInspInfo> susps;

            // tikrinamas II
            db.Rows.Add(4, "01", 1, 256, 3, 36, 9, "06.4", new DateTime(2017, 1, 15), new DateTime(2017, 1, 15), null, null);
            insp = new Insp(4, "01", 1, 252, 3, 36, 9, "06.3", "428", "829", new DateTime(2017, 5, 12), "GTC", Kelintas.Second);
            susps = InspValidationMethods.ValidateNepirm(insp, db);
            Assert.IsTrue(susps.Count == 3);
        }
    }
}