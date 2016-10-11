using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTranslator;

namespace UnitTestProject
{
    /* тестовый код:
     * begin
     * var i := 1;
     * end
     */
    [TestClass]
    public class HashTableTest
    {
        [TestMethod]
        public void TestCalcHash()
        {
            HashTable table = new HashTable();
            Console.WriteLine(table.DJBHash("var"));
        }

        [TestMethod]
        public void TestPut()
        {
            HashTable table = new HashTable();
            table.identsTable = new IdentificatorTable();
            Console.WriteLine(table.DJBHash("var"));
            table.put("begin");
            table.put("var");
            table.put("i");
            table.put(" ");
            table.put(":=");
            table.put("1");
            table.put("end");
        }

        [TestMethod]
        public void TestFindString()
        {
            HashTable table = new HashTable();
            table.identsTable = new IdentificatorTable();
            Console.WriteLine(table.DJBHash("var"));
            table.put("begin");
            table.put("var");
            table.put("i");
            table.put(" ");
            table.put(":=");
            table.put("1");
            table.put("end");
            Identificator found = table.find("var");
            Console.WriteLine(found.Value);
        }
    }
}
