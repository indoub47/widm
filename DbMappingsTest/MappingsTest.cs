using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbMappings;

namespace DbMappingsTest
{
    [TestClass]
    public class MappingsTest
    {
        [TestMethod]
        public void TestGetWithString()
        {
            MappingField mField = Mappings.Get("id");
            Assert.AreEqual("number", mField.DbName);
            Assert.AreEqual("System.Int64", mField.TypeName);
        }

        [TestMethod]
        public void TestGetByDbName()
        {
            MappingField mField = Mappings.GetByDBName("kilomrtras");
            Assert.AreEqual("km", mField.Name);
            Assert.AreEqual("System.Int32", mField.TypeName);
        }

        [TestMethod]
        public void TestIndex()
        {
            int indexPk = Mappings.Index("pk");
            Assert.AreEqual(4, indexPk);
            int indexBs = Mappings.Index("bullshit");
            Assert.AreEqual(-1, indexBs);
        }

        [TestMethod]
        public void TestIndexByDbName()
        {
            int index = Mappings.IndexByDbName("metras");
            Assert.AreEqual(5, index);
            int indexBs = Mappings.IndexByDbName("bullshit");
            Assert.AreEqual(-1, indexBs);
        }

        [TestMethod]
        public void TestDBName()
        {
            string dbColName = Mappings.DBColName("m");
            Assert.AreEqual("metras", dbColName, "metras");
            string dbColName1 = Mappings.DBColName("operatorius3");
            Assert.AreEqual("III_pat_operaqtor", dbColName1, "oper3");
        }

        [TestMethod]
        public void TestGetWithInt()
        {
            MappingField mField = Mappings.Get(6);
            Assert.AreEqual("siule", mField.Name);
            Assert.AreEqual("siule", mField.DbName);
            Assert.AreEqual("System.Int32", mField.TypeName);
        }

        [TestMethod]
        public void TestLength()
        {
            int length = Mappings.Length;
            Assert.AreEqual(27, length);
        }

        [TestMethod]
        [ExpectedException(typeof(System.IndexOutOfRangeException))]
        public void TestGetWithIntOutOfRange()
        {
            MappingField mField = Mappings.Get(30);
        }

        [TestMethod]
        public void TestAllFields()
        {
            MappingField[] fieldArr = Mappings.AllFields;
            Assert.AreEqual(27, fieldArr.Length);
            Assert.AreEqual("saliginis kodas", fieldArr[7].DbName);
        }
    }
}