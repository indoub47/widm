using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoomyTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int sd = 4;
            Assert.AreEqual(4, sd);
        }
    }
}
