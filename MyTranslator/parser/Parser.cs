using System;
namespace MyTranslator
{
	public class Parser
	{
		Lexer lexer;

		public Parser(Lexer lexer)
		{
			this.lexer = lexer;
		}

		public void accept(int symbolexpected)
		{
			if (lexer.symbol == symbolexpected)
			{
				lexer.nextsym();
			}
			else
			{
				lexer.io.error(symbolexpected, lexer.token);
			}
		}

		public void block()
		{
			/*  анализ конструкции <блок>*/
			// todo расширить язык до поддержки модулей и пользовательских типов
			//labelpart(); // раздел меток
			//constpart(); // раздел констант
			//typepart(); // раздел типов
			varpart(); // раздел переменных
			procfuncpart(); // раздел процедур и функций
			statementpart(); // раздел операторов
		}

		public void whilestatement()
		{
			/* анализ конструкции цикл с предусловием */
			accept(Symbol.whilesy);
			expression();
			accept(Symbol.dosy);
			statement();
		}

		public void forstatement()
		{
			/* анализ конструкции цикл с параметром */
			accept(Symbol.forsy);
			accept(Symbol.ident);
			accept(Symbol.assign);
			expression();
			if (lexer.symbol == Symbol.tosy || lexer.symbol == Symbol.downtosy)
			{
				lexer.nextsym();
			}
			expression();
			accept(Symbol.dosy);
			statement();
		}

		public void compoundstatement()
		{
			/* анализ конструкции составной оператор */
			accept(Symbol.beginsy);
			while (lexer.symbol == Symbol.semicolon)
			{
				lexer.nextsym();
				statement();
			}
			accept(Symbol.endsy);
		}

		public void varpart()
		{
			/* анализ конструкции раздел переменных */
			if (lexer.symbol == Symbol.varsy)
			{
				accept(Symbol.varsy);
				do
				{
					vardeclaration();
					accept(Symbol.semicolon);
				} while (lexer.symbol == Symbol.ident);
			}
		}

		public void vardeclaration()
		{
			/* анализ конструкции <анализ однотипных переменных>*/
			accept(Symbol.ident);
			while (lexer.symbol == Symbol.comma)
			{
				lexer.nextsym();
				accept(Symbol.ident);
			}
			accept(Symbol.colon);
			type();
		}

		public void arraytype()
		{
			accept(Symbol.arraysy);
			accept(Symbol.lbracket);
			simpletype();
			while (lexer.symbol == Symbol.comma)
			{
				lexer.nextsym();
				simpletype();
			}
			accept(Symbol.rbracket);
			accept(Symbol.ofsy);
			type();
		}

		public void variable()
		{
			/* анализ конструкции <переменная> */
			accept(Symbol.ident);
			while (lexer.symbol == Symbol.lbracket || lexer.symbol == Symbol.point ||
				  lexer.symbol == Symbol.arrow)
			{
				if (lexer.symbol == Symbol.lbracket)
				{
					lexer.nextsym();
					expression();
					while (lexer.symbol == Symbol.comma)
					{
						lexer.nextsym();
						expression();
					}
					accept(Symbol.rbracket);
				}
				else if (lexer.symbol == Symbol.point)
				{
					lexer.nextsym();
					accept(Symbol.ident);
				}
				else if (lexer.symbol == Symbol.arrow)
				{
					lexer.nextsym();
				}
			}
		}

		public void ifstatement()
		{
			accept(Symbol.ifsy);
			expression();
			accept(Symbol.thensy);
			statement();
			if (lexer.symbol == Symbol.elsesy)
			{
				lexer.nextsym();
				statement();
			}
		}
	}
}
