using System;
namespace MyTranslator
{
	public static class Symbol
	{
		public static readonly int star = 21; 
		public static readonly int slash = 60;
		public static readonly int equal = 16;
		public static readonly int comma = 20;
		public static readonly int semicolon = 14;
		public static readonly int colon = 5;
		public static readonly int point = 61;
		public static readonly int arrow = 62;
		public static readonly int leftpar = 9;
		public static readonly int rightpar = 4;
		public static readonly int lbracket = 11;
		public static readonly int rbracket = 12;
		public static readonly int flpar = 63;
		public static readonly int frpar = 64;
		public static readonly int later = 65;
		public static readonly int greater = 66;
		public static readonly int laterequal = 67;
		public static readonly int greaterequal = 68;
		public static readonly int latergreater = 69;
		public static readonly int plus = 70;
		public static readonly int minus = 71;
		public static readonly int lcomment = 72;
		public static readonly int rcomment = 73;
		public static readonly int assign = 51;
		public static readonly int twopoints = 74;
		public static readonly int ident = 2;
		public static readonly int floatc = 82;
		public static readonly int intc = 15;
		public static readonly int charc = 83;

		public static readonly int casesy = 31; // case
		public static readonly int elsesy = 32; // else
		public static readonly int writesy = 30; // write
		public static readonly int readsy = 29; // read
		public static readonly int whilesy = 28; // while
		public static readonly int ifsy = 57; // file
		public static readonly int gotosy = 33; // goto
		public static readonly int thensy = 52; // then
		public static readonly int typesy = 34; // type
		public static readonly int untilsy = 53; // until
		public static readonly int dosy = 54; // do
		public static readonly int withsy = 37; // with
		public static readonly int tosy = 75; // to
		public static readonly int andsy = 76; // and
		public static readonly int notsy = 77; // not
		public static readonly int forsy = 78; // for
		public static readonly int divsy = 79; // div
		public static readonly int varsy = 80; // var
		public static readonly int downtosy = 90; // downto
		public static readonly int beginsy = 91; // begin
		public static readonly int endsy = 92; // end
        internal static int arraysy;
        internal static int ofsy;
    }

	public struct Key
	{
		public int codekey;
		public char[] namekey;

		public Key(int codekey, char[] namekey)
		{
			this.codekey = codekey;
			this.namekey = namekey;
		}
	}

	public class Lexer
	{
		public int symbol { get; private set;} /* код символа */
		public TextPosition token; // позиция символа
		char addrname; // адрес идентификатора в таблице имен
		int nmb_int; // значение целой константы
		float nmb_float; // значение вещественной константы
		char one_symbol; // значение символьной константы
		public IO io;

		public static readonly int MAX_INT = Int32.MaxValue;
		public static readonly int MAX_IDENT = 10000;

		char[] name; // идентификатор
		int lname; // длина идентификатора

		Key[] keywords = {
			new Key(Symbol.ident, " ".ToCharArray()),
			// группа идентификаторов из 2х символов
			new Key(Symbol.dosy, "do".ToCharArray()),
			new Key(Symbol.ifsy, "if".ToCharArray()),
			//new Key(Symbol.insy, "in".ToCharArray()),
			//new Key(Symbol.ofsy, "of".ToCharArray()),
			//new Key(Symbol.orsy, "or".ToCharArray()),
			//new Key(Symbol.tosy, "to".ToCharArray()),
			new Key(Symbol.ident, " ".ToCharArray()),
			// группа идентификаторов из 3х символов
			new Key(Symbol.andsy, "and".ToCharArray()),
			new Key(Symbol.divsy, "div".ToCharArray()),
			new Key(Symbol.varsy, "var".ToCharArray()),
			new Key(Symbol.notsy, "not".ToCharArray()),
			new Key(Symbol.forsy, "for".ToCharArray()),
			new Key(Symbol.endsy, "end".ToCharArray()),
			new Key(Symbol.ident, " ".ToCharArray()),
			// группа идентификаторов из 4х символов
			new Key(Symbol.readsy, "read".ToCharArray()),
			new Key(Symbol.thensy, "then".ToCharArray()),
			new Key(Symbol.elsesy, "else".ToCharArray()),
			new Key(Symbol.casesy, "case".ToCharArray()),
			// группа идентификаторов из 5ти символов
			new Key(Symbol.ident, " ".ToCharArray()),
			new Key(Symbol.writesy, "write".ToCharArray()),
			new Key(Symbol.whilesy, "while".ToCharArray()),
			new Key(Symbol.beginsy, "begin".ToCharArray()),
			new Key(Symbol.ident, " ".ToCharArray()),
			new Key(Symbol.downtosy, "downto".ToCharArray()),
			new Key(Symbol.ident, " ".ToCharArray())
		};

		/*
		 * таблица ключевых слов (для тестов)
		 * с кодом ident  для группы i
		 */
		int[] last = {-1, 0, 3, 9, 14, 18, 20 };

		public Lexer(IO io)
		{
			this.io = io;
		}

		public void nextsym()
		{
			while (io.ch == ' ')
			{
				io.nextch();
			}
			token.lineNumber = io.positionNow.lineNumber;
			token.charNumber = io.positionNow.lineNumber;
			switch (io.ch)
			{
				case '<':
					io.nextch();
					if (io.ch == '=')
					{
						symbol = Symbol.laterequal;
						io.nextch();
					}
					else if(io.ch == '>')
					{
						symbol = Symbol.latergreater;
						io.nextch();
					}
					else
					{
						symbol = Symbol.later;
					}
					break;
				case ':':
					io.nextch();
					if (io.ch == '=')
					{
						symbol = Symbol.assign;
						io.nextch();
					}
					else
					{
						symbol = Symbol.colon;
					}
					break;
				case ';':
					symbol = Symbol.semicolon;
					io.nextch();
					break;
				case '+':
					symbol = Symbol.plus;
					io.nextch();
					break;
				case '-':
					symbol = Symbol.minus;
					io.nextch();
					break;
				case '*':
					symbol = Symbol.star;
					io.nextch();
					break;
				case '/':
					symbol = Symbol.slash;
					io.nextch();
					break;
				default: break;
			}
			if (io.ch >= '0' && io.ch <= '9')
			{
				nmb_int = 0;
				while (io.ch >= '0' && io.ch <= '9')
				{
					int digit = io.ch - '0';
					if (nmb_int < MAX_INT / 10 || (nmb_int == MAX_INT / 10 && digit <= MAX_INT % 10))
					{
						nmb_int = 10 * nmb_int + digit;
					}
					else
					{
						/* константа превышает предел */
						io.error(203, io.positionNow);
						nmb_int = 0;
					}
					io.nextch();
				}
				symbol = Symbol.intc;
				return;
			}
			if (io.ch >= 'a' && io.ch <= 'z' || io.ch >= 'A' && io.ch <= 'Z')
			{
				lname = 0;
				while ((io.ch >= 'a' && io.ch <= 'z' ||
					  io.ch >= 'A' && io.ch <= 'Z' ||
				        io.ch >= '0' && io.ch <= '9') && lname < MAX_IDENT)
				{
					name[lname++] = io.ch;
					io.nextch();
				}
				name[lname] = '\0';
				Array.Copy(keywords[last[lname]].namekey, name, name.Length);
				int i = last[lname - 1]+1;
				while (!keywords[i].namekey.Equals(name))
				{
					i++;
				}
				symbol = keywords[i].codekey;
			}
		}
	}
}
