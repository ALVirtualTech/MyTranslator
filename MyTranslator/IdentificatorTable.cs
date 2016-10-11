using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTranslator
{
    public class IdentificatorTable
    {
        Identificator head;

        public IdentificatorTable()
        {
            head = new Identificator();
        }

        /// <summary>
        /// добавление элемента в таблицу идентификаторов, если в хэш таблице данное значение хэш функции встретилось впервые
        /// </summary>
        /// <param name="identificator"></param>
        /// <returns></returns>
        public Identificator addElement(Identificator identificator)
        {
            Identificator current = head;
            while (current.next != null)
            {
                current = current.next;
            }
            return current.next = (Identificator) identificator.Clone();    
        }

        /// <summary>
        /// добавляет идентификаторы с одинаковым хэшем в цепочку
        /// </summary>
        /// <param name="identificator"></param>
        /// <param name="newIdent"></param>
        public void addInList(Identificator identificator, Identificator newIdent)
        {
            Identificator current = head;
            while (current.next != null)
            {
                current = current.next;
                if (current == identificator)
                {
                    var innerCurrent = identificator.innerList;
                    while (innerCurrent != null)
                    {
                        innerCurrent = innerCurrent.innerList;
                    }
                    innerCurrent = (Identificator) newIdent.Clone();
                    return;
                }
            }  
        }

        public Identificator find(Identificator identificator, Identificator neededToFind)
        {
            if (identificator.Value != neededToFind.Value)
            {
                var innerCurrent = identificator.innerList;
                while (innerCurrent != null)
                {
                    if(innerCurrent.Value == neededToFind.Value)
                        return innerCurrent;
                    innerCurrent = innerCurrent.innerList;
                }
                throw new NotFoundException(neededToFind);
            }
            else
            {
                return identificator;
            }
        }

        ~IdentificatorTable()
        {
            try
            {
                Identificator current = head, next = current;
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
