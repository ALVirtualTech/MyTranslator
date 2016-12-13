using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyTranslator
{
	public struct TextPosition
	{
		public TextPosition(int lineNumber, int charNumber)
		{
			this.lineNumber = lineNumber;
			this.charNumber = charNumber;
		}

		public int lineNumber;
		public int charNumber;
	}

	public struct ErrList
	{
		public ErrList(TextPosition textPosition, int errorcode)
		{
			this.textPosition = textPosition;
			this.errorcode = errorcode;
		}

		public TextPosition textPosition;
		public int errorcode;
		public static readonly int ERRMAX = 100;

	}

	public class IO
	{
		public TextPosition positionNow;
		ErrList[] errList; 
		int errInx; // количество обнаруженных ошибок в текущей строке
		bool errorOverFlow; // максимальное количество ошибок
		public static readonly int MAXLINE = 255; // максимальная длина строки
		int lastInLine; // количество символов в текущей строке
		public char[] line { get; private set; } // текущая строка
		public char ch { get; private set; }// текущий символ
		StreamReader sr;
		StreamWriter sw;

		public IO(String inputPath, String outputPath)
		{
			errList = new ErrList[ErrList.ERRMAX];
			positionNow = new TextPosition(0, 0);
			errInx = -1;
			sr = new StreamReader(inputPath);
			sw = new StreamWriter(outputPath);
		}

		public bool nextch()
		{
			if (positionNow.charNumber == lastInLine)
			{
				ListThisLine();
				var errorsInCurrentLine =
					errList.ToList().FindAll(
						obj => obj.textPosition.lineNumber == positionNow.lineNumber
					);
				if (errorsInCurrentLine.Count != 0)
				{
					ListErrors(errorsInCurrentLine);
				}
				ReadNextLine();
				positionNow.lineNumber++;
				positionNow.charNumber = -1;
			}
			else
			{
				positionNow.charNumber++;
				ch = line[positionNow.charNumber];
			}
			return true;
		}

		void ListThisLine()
		{
			Console.WriteLine(line.ToString());
		}

		void ReadNextLine()
		{
			string lines = string.Empty;
			if ((lines = sr.ReadLine()) != null)
			{
				line = lines.ToCharArray();
				lastInLine = line.Length;
			}
			else
			{
				EndReading();
			}
		}

		void ListErrors(List<ErrList> errorsInCurrentLine)
		{
			for (int i = 0; i < errInx; i++)
			{
				sw.WriteLine(string.Format("**{0}**\terror code {1}", i, errorsInCurrentLine[i].errorcode));
			}
		}

		public void error(int errorcode, TextPosition position)
		{
			if (errInx == ErrList.ERRMAX)
			{
				errorOverFlow = true;
			}
			else
			{
				++errInx;
				errList[errInx].errorcode = errorcode;
				errList[errInx].textPosition.lineNumber = position.lineNumber;
				errList[errInx].textPosition.charNumber = position.charNumber;
			}
		}

		void EndReading()
		{
			sr.Close();
			sw.WriteLine(string.Format("Компиляция окончена: ошибок {1}", errList.Count()));
			sw.Close();
		}
	}
}

