using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTranslator
{
    public class HashTableElement{
        public long ID;
        public HashTableElement next;
        public Identificator value;

        public HashTableElement(long ID)
        {

        }
    }

    /// <summary>
    /// варианты хэш функций https://habrahabr.ru/post/219139/
    /// </summary>
    public class HashTable
    {
        HashTableElement head;
        public IdentificatorTable identsTable;

        public HashTable()
        {
            head = new HashTableElement(-1);
        }

        /// <summary>
        /// Расчет хэш функции для полученного идентификатора
        /// </summary>
        /// <param name="identificator">идентификатор</param>
        /// <returns>значение хэш-функции</returns>
        public long HashH37(String identificator)
        {
	        long hash = 2139062143;
            foreach (char c in identificator)
            {
                hash = 37 * hash + c;
            }
	        return hash;
        }

        

    public long DJBHash (String str)
    {
	     int hash = 5381;
	    foreach (char c in str) {
		    hash = ((hash << 5) + hash) + c;
	    }
	    return hash % 100;
    }



        public void put(String identificator)
        {
            long id = DJBHash(identificator);
            int i = 0;
            HashTableElement current = head;
            while (i <= id)
            {
                if (current.next == null)
                {
                    current.next = new HashTableElement(i);
                }
                i++;
                current = current.next;
            }
            if (current.value == null)
                current.value = identsTable.addElement(Identificator.createFromString(identificator));
            else{
                Identificator newIdent = Identificator.createFromString(identificator);
                identsTable.addInList(current.value, newIdent);
            }
            
        }

        public Identificator find(Identificator identificator)
        {
            long id = DJBHash(identificator.Value);
            int i = 0;
            HashTableElement current = head;
            while (i <= id)
            {
                current = current.next;
                i++;
            }
            return identsTable.find(current.value, identificator);
        }

        public Identificator find(String identificator)
        {
            return find(Identificator.createFromString(identificator));
        }

        ~HashTable()
        {
            try
            {
                HashTableElement current = head, next = current;
                while (current.next != null)
                {
                    next = current.next;
                    current = null;
                    current = next;
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
