using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
			/* тестовый код:
             * begin
             * var i := 1;
             * end
             */
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
            Identificator found = table.find(":=");
            Console.WriteLine(found.Value);
            Console.ReadKey();
        }
    }
}
