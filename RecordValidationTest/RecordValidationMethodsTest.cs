using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordValidation;

namespace RecordValidationTest
{
    [TestClass]
    public class RecordValidationMethodsTest
    {
        private static string[] mapping;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            mapping = new string[] { "id", "linija", "kelias", "km", "pk", "m", "siule", "salyginis_kodas", "suvirino", "tikrinimo_data", "aparatas", "kelintas_tikrinimas" };
        }


        [TestMethod]
        public void TestValidateId()
        {
            int index = 0;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "long as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, false, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", false, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateId(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateLinija()
        {
            int index = 1;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {"46", true, "string from array"},
                new object[] {46, true, "int - like string from array"},
                new object[] {-46, false, "negative int like string from array"},
                new object[] {"ab", false, "non-digit string not from array"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {"53", false, "digit string not from array"},
                new object[] {0, false, "zero"},
                new object[] {" 46", true, "ws + string from array"},
                new object[] {"4 6", false, "ws inside string from array"},
                new object[] {"0", false, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateLinija(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateKelias()
        {
            int index = 2;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "int as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, false, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", false, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateKelias(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateKm()
        {
            int index = 3;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "int as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, false, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", false, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateKm(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidatePk()
        {
            int index = 4;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "int as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, true, "null"},
                new object[] {"", true, "es"},
                new object[] {"  ", true, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, true, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", true, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidatePk(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateM()
        {
            int index = 5;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "int as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, true, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", true, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateM(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateSiule()
        {
            int index = 6;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {123, true, "int"},
                new object[] {"123", true, "int as string"},
                new object[] {"ab", false, "no-digit string"},
                new object[] {null, true, "null"},
                new object[] {"", true, "es"},
                new object[] {"  ", true, "ws"},
                new object[] {-123, false, "negative int"},
                new object[] {0, true, "zero"},
                new object[] {"-123", false, "negative int as string"},
                new object[] {"0", true, "zero as string"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateSiule(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateSalKodas()
        {
            int index = 7;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {"06.3", true, "value from array"},
                new object[] {"06. 4", false, "value from array with ws inside"},
                new object[] {" 06.3", true, "ws + value from array"},
                new object[] {"06.2", false, "value not from array"},
                new object[] {6.4, false, "float similar to value from array"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {"06.4 ", true, "value from array + ws"},
                new object[] {" 06.3 ", true, "ws + value from array + ws"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateSalKodas(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateAparatas()
        {
            int index = 10;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {"831", true, "value from array"},
                new object[] {"8 31", false, "value from array with ws inside"},
                new object[] {" 831", true, "ws + value from array"},
                new object[] {"888", false, "value not from array"},
                new object[] {831, true, "int from array"},
                new object[] {null, false, "null"},
                new object[] {"", false, "es"},
                new object[] {"  ", false, "ws"},
                new object[] {"831 ", true, "value from array + ws"},
                new object[] {" 831 ", true, "ws + value from array + ws"},
                new object[] {888, false, "int not from array"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateAparatas(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateTikrinimoData()
        {
            int index = 9;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {null, false, "null"},
                new object[] {"", false, "empty string"},
                new object[] {"   ", false, "white spaces"},

                new object[] {new DateTime(2017, 10, 23), true, "ordinary date"},

                new object[] {"2017-10-13", true, "yyyy-mm-dd"},
                new object[] {"2017-1-13", true, "yyyy-m-dd"},
                new object[] {"2017-10-3", true, "yyyy-mm-d"},
                new object[] {"2017-1-3", true, "yyyy-m-d"},
                new object[] {"2017.10.13", true, "yyyy.mm.dd"},
                new object[] {"2017.1.13", true, "yyyy.m.dd"},
                new object[] {"2017.10.3", true, "yyyy.mm.d"},
                new object[] {"2017.1.3", true, "yyyy.m.d"},
                new object[] {"2017/10/13", true, "yyyy/mm/dd"},
                new object[] {"2017/1/13", true, "yyyy/m/dd"},
                new object[] {"2017/10/3", true, "yyyy/mm/d"},
                new object[] {"2017/1/3", true, "yyyy/m/d"},
                new object[] {"2017 10 13", true, "yyyy mm dd"},
                new object[] {"2017 1 13", true, "yyyy m dd"},
                new object[] {"2017 10 3", true, "yyyy mm d"},
                new object[] {"2017 1 3", true, "yyyy m d"},

                new object[] {"2017-13-13", false, "yyyy-MM-dd"},
                new object[] {"2017-13-3", false, "yyyy-MM-d"},
                new object[] {"2017.13.13", false, "yyyy.MM.dd"},
                new object[] {"2017.13.3", false, "yyyy.MM.d"},
                new object[] {"2017/13/13", false, "yyyy/MM/dd"},
                new object[] {"2017/13/3", false, "yyyy/MM/d"},
                new object[] {"2017 13 13", false, "yyyy MM dd"},
                new object[] {"2017 13 3", false, "yyyy MM d"},

                new object[] {"2017-10-33", false, "yyyy-mm-DD"},
                new object[] {"2017-1-33", false, "yyyy-m-DD"},
                new object[] {"2017.10.33", false, "yyyy.mm.DD"},
                new object[] {"2017.1.33", false, "yyyy.m.DD"},
                new object[] {"2017/10/33", false, "yyyy/mm/DD"},
                new object[] {"2017/1/33", false, "yyyy/m/DD"},
                new object[] {"2017 10 33", false, "yyyy mm DD"},
                new object[] {"2017 1 33", false, "yyyy m DD"},

                new object[] {"2017-13-33", false, "yyyy-MM-DD"},
                new object[] {"2017.13.33", false, "yyyy.MM.DD"},
                new object[] {"2017/13/33", false, "yyyy/MM/DD"},
                new object[] {"2017 13 33", false, "yyyy MM DD"},
                
                new object[] {"3/10/2017", false, "d/mm/yyyy"},
                new object[] {"03/10/2017", false, "dd/mm/yyyy"},

                new object[] {"03/06/2017", false, "dd/mm/yyyy"},
                new object[] {"03/6/2017", false, "dd/m/yyyy"},

                new object[] {"3/6/2017", false, "d/m/yyyy"},

                new object[] {"3-10-2017", false, "d-mm-yyyy"},
                new object[] {"03-10-2017", false, "dd-mm-yyyy"},

                new object[] {"03-06-2017", false, "dd-mm-yyyy"},
                new object[] {"03-6-2017", false, "dd-m-yyyy"},

                new object[] {"3-6-2017", false, "d-m-yyyy"},

                new object[] {"3.10.2017", false, "d.mm.yyyy"},
                new object[] {"03.10.2017", false, "dd.mm.yyyy"},

                new object[] {"03.06.2017", false, "dd.mm.yyyy"},
                new object[] {"03.6.2017", false, "dd.m.yyyy"},

                new object[] {"3.6.2017", false, "d.m.yyyy"},

                new object[] {"3 10 2017", false, "d mm yyyy"},
                new object[] {"03 10 2017", false, "dd mm yyyy"},

                new object[] {"03 06 2017", false, "dd mm yyyy"},
                new object[] {"03 6 2017", false, "dd m yyyy"},

                new object[] {"3 6 2017", false, "d m yyyy"},

                new object[] {"3/10/17", false, "d/mm/yy"},
                new object[] {"03/10/17", false, "dd/mm/yy"},

                new object[] {"03/06/17", false, "dd/mm/yy"},
                new object[] {"03/6/17", false, "dd/m/yy"},

                new object[] {"3/6/17", false, "d/m/yy"},

                new object[] {"3-10-17", false, "d-mm-yy"},
                new object[] {"03-10-17", false, "dd-mm-yy"},

                new object[] {"03-06-17", false, "dd-mm-yy"},
                new object[] {"03-6-17", false, "dd-m-yy"},

                new object[] {"3-6-17", false, "d-m-yy"},

                new object[] {"3.10.17", false, "d.mm.yy"},
                new object[] {"03.10.17", false, "dd.mm.yy"},

                new object[] {"03.06.17", false, "dd.mm.yy"},
                new object[] {"03.6.17", false, "dd.m.yy"},

                new object[] {"3.6.17", false, "d.m.yy"},

                new object[] {"3 10 17", false, "d mm yy"},
                new object[] {"03 10 17", false, "dd mm yy"},

                new object[] {"03 06 17", false, "dd mm yy"},
                new object[] {"03 6 17", false, "dd m yy"},

                new object[] {"3 6 17", false, "d m yy"},

                new object[] {"13/14/2017", false, "dd/MM/yyyy"},
                new object[] {"50/10/2017", false, "DD/mm/yyyy"},
                new object[] {56987, false, "integer"},
                new object[] {456987.4, false, "float"},
                new object[] {"456987.4", false, "float string"},
                new object[] {"56987", false, "integer string"},
                new object[] {"abcd", false, "non digit character string"}
            };

            foreach(var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateTikrinimoData(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateTikrinimoDataIsReal()
        {
            int index = 9;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                // wrong dates must be valid - they must be validate with the ValidateTikrinimoData
                // invalid are the future dates and dates far in the past
                new object[] {null, true, "null"},
                new object[] {"", true, "empty string"},
                new object[] {"   ", true, "white spaces"},
                new object[] {"2017.13.3", true, "month is too big number"},
                new object[] {"03-06-2017", true, "wrong format"},
                new object[] {DateTime.Now, true, "today"},
                new object[] {DateTime.Now.AddDays(1), false, "tomorrow"},
                new object[] {DateTime.Now.AddDays(-1), true, "yesterday"},
                new object[] {DateTime.Now.AddDays(-100), false, "100 days before"}
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateTikrinimoDataIsReal(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidatePrivalomasPk()
        {
            int keliasIndex = 2, pkIndex = 4;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {"ab", 5, true, "oo.abooo.05.oo.o"}, // atras ten, kur tikrins kelią
                new object[] {null, 5, true, "oo.nullooo.05.oo.o"}, // atras ten, kur tikrins kelią
                new object[] {null, null, true, "oo.nullooo.null.oo.o"}, // atras ten, kur tikrins kelią
                new object[] {"ab", null, true, "oo.abooo.null.oo.o"}, // atras ten, kur tikrins kelią
                new object[] {-5, "", true, "oo.-5ooo.es.oo.o"}, // atras ten, kur tikrins kelią
                new object[] {5, -5, true, "oo.5ooo.-5.oo.o"}, // atras ten, kur tikrins pk
                new object[] {null, -5, true, "oo.5ooo.-5.oo.o"}, // atras ten, kur tikrins kelią ir pk

                new object[] {1, 9, true, "oo.1ooo.09.oo.o"},
                new object[] {9, 5, true, "oo.9ooo.05.oo.o"},

                new object[] {8, 5, false, "oo.8ooo.5.oo.o"},

                new object[] {8, 0, true, "oo.8ooo.00.oo.o"},
                new object[] {8, null, true, "oo.8ooo.null.oo.o"},

                new object[] {9, 0, false, "oo.9ooo.00.oo.o"},
                new object[] {5, null, false, "oo.5ooo.null.oo.o"},
                new object[] {9, "   ", false, "oo.9ooo.ws.oo.o"},
            };

            foreach (var obj in valuesMessages)
            {
                record[keliasIndex] = obj[0];
                record[pkIndex] = obj[1];
                Assert.IsTrue((RecordValidationMethods.ValidatePrivalomasPk(record, mapping) == null) == (bool)obj[2], obj[3].ToString());
            }
        }

        [TestMethod]
        public void TestValidateSiuleIesme()
        {
            int keliasIndex = 2, siuleIndex = 6;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {"ab", 5, true, "oo.abooo.oo.oo.5"}, // atras ten, kur tikrins kelią
                new object[] {null, 5, true, "oo.nullooo.oo.oo.5"}, // atras ten, kur tikrins kelią
                new object[] {null, null, true, "oo.nullooo.oo.oo.null"}, // atras ten, kur tikrins kelią
                new object[] {"ab", null, true, "oo.abooo.oo.oo.null"}, // atras ten, kur tikrins kelią
                new object[] {-5, "", true, "oo.-5ooo.oo.oo."}, // atras ten, kur tikrins kelią
                new object[] {"", -5, true, "oo.5ooo.oo.oo.-5"}, // atras ten, kur tikrins kelią ir siūlę
                new object[] {"   ", "ab", true, "oo.9ooo.oo.oo.ab"}, // atras ten, kur tikrins kelią siūlę
                new object[] {5, -5, true, "oo.5ooo.oo.oo.-5"}, // atras ten, kur tikrins siūlę
                new object[] {5, "ab", true, "oo.9ooo.oo.oo.ab"}, // atras ten, kur tikrins siūlę

                new object[] {1, 9, true, "oo.1ooo.oo.oo.9"},
                new object[] {1, 0, true, "oo.1ooo.oo.oo.0"},
                new object[] {8, 5, false, "oo.8ooo.oo.oo.5"},
                new object[] {9, 5, false, "oo.9ooo.oo.oo.5"},
                new object[] {8, 0, false, "oo.8ooo.oo.oo.0"},
                new object[] {9, 0, false, "oo.9ooo.oo.oo.0"},

                new object[] {8, null, true, "oo.8ooo.oo.oo.null"},
                new object[] {9, "  ", true, "oo.9ooo.oo.oo.ws"},
                new object[] {9, "", true, "oo.9ooo.oo.oo.es"},

                new object[] {5, null, false, "oo.5ooo.oo.oo.null"},
                new object[] {5, "   ", false, "oo.5ooo.oo.oo.ws"},
                new object[] {5, "", false, "oo.5ooo.oo.oo.es"},
            };

            foreach (var obj in valuesMessages)
            {
                record[keliasIndex] = obj[0];
                record[siuleIndex] = obj[1];
                Assert.IsTrue((RecordValidationMethods.ValidateSiuleIesmeEmpty(record, mapping) == null) == (bool)obj[2], obj[3].ToString());
            }
        }


        [TestMethod]
        public void TestValidateSuvirino()
        {
            int index = 8;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {null, false, "null"},
                new object[] {"", false, "empty string"},
                new object[] {"   ", false, "white spaces"},
                new object[] {"GTC", true, "value from array"},
                new object[] {"IF-4", false, "value not from array"},
                new object[] {" GTC", true, "ws + value from array"},
                new object[] {" GTC ", true, "ws + value from array + ws"}
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateSuvirino(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }


        [TestMethod]
        public void TestValidateKelintas()
        {
            int index = 11;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", new DateTime(2017, 10, 23), "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {null, false, "null"},
                new object[] {"", false, "empty string"},
                new object[] {"   ", false, "white spaces"},
                new object[] {"1", true, "value from array"},
                new object[] {"5", false, "value not from array"},
                new object[] {" 2", true, "ws + value from array"},
                new object[] {" papild ", true, "ws + value from array + ws"},
                new object[] {2, true, "integer"},
                new object[] {5, false, "integer not from array"},
                new object[] {1, true, "1 integer"},
                new object[] {-1, false, "-1 integer"},
                new object[] {0, false, "0 integer"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateKelintas(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }

        [TestMethod]
        public void TestValidateNegaliButiPirmas()
        {
            int index = 11;
            object[] record = new object[] { 123, "01", 5, 325, 6, 92, 0, "06.4", "GTC", DateTime.Now, "831", 2 };
            object[][] valuesMessages =
            {
                new object[] {null, true, "null"},
                new object[] {"", true, "empty string"},
                new object[] {"   ", true, "white spaces"},
                new object[] {"1", false, "1 string"},
                new object[] {"5", true, "string value not from array"},
                new object[] {" 2", true, "ws + 2"},
                new object[] {" papild ", true, "ws + papild + ws"},
                new object[] {2, true, "2 integer"},
                new object[] {5, true, "integer not from array"},
                new object[] {1, false, "1 integer"},
                new object[] {-1, true, "-1 integer"},
                new object[] {0, true, "0 integer"},
            };

            foreach (var obj in valuesMessages)
            {
                record[index] = obj[0];
                Assert.IsTrue((RecordValidationMethods.ValidateNegaliButiPirmas(record, mapping) == null) == (bool)obj[1], obj[2].ToString());
            }
        }
    }
}
