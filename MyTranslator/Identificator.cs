using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTranslator
{
    public class Identificator
    {
        public int ID { get; private set; }
        public Identificator next
        {
            get;
            set;
        }
        public Identificator innerList
        {
            get;
            set;
        }
        public String Value { get;  set; }

        public object Clone()
        {
            return new Identificator { ID = this.ID, Value = this.Value, next = this.next };
        }

        public static Identificator createFromString(String strIdent){
            Identificator ident = new Identificator();
            ident.Value = strIdent;
            return ident;
        }

        public long HashCode()
        {
            return 1;
        }
    }
}
