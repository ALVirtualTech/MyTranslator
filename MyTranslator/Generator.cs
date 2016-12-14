using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Translator
{
    public sealed class Scanner
    {
        private readonly IList<object> result;

        public Scanner(TextReader input)
        {
            this.result = new List<object>();
            this.Scan(input);
        }

        public IList<object> Tokens
        {
            get { return this.result; }
        }

        #region ArithmiticConstants

        // Constants to represent arithmitic tokens. This could
        // be alternatively written as an enum.
        public static readonly object Add = new object();
        public static readonly object Sub = new object();
        public static readonly object Mul = new object();
        public static readonly object Div = new object();
        public static readonly object Semi = new object();
        public static readonly object Equal = new object();

        #endregion

        private void Scan(TextReader input)
        {
            while (input.Peek() != -1)
            {
                char ch = (char)input.Peek();

                // Scan individual tokens
                if (char.IsWhiteSpace(ch))
                {
                    // eat the current char and skip ahead!
                    input.Read();
                }
                else if (char.IsLetter(ch) || ch == '_')
                {
                    // keyword or identifier

                    StringBuilder accum = new StringBuilder();

                    while (char.IsLetter(ch) || ch == '_')
                    {
                        accum.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            break;
                        }
                        else
                        {
                            ch = (char)input.Peek();
                        }
                    }

                    this.result.Add(accum.ToString());
                }
                else if (ch == '"')
                {
                    // string literal
                    StringBuilder accum = new StringBuilder();

                    input.Read(); // skip the '"'

                    if (input.Peek() == -1)
                    {
                        throw new System.Exception("unterminated string literal");
                    }

                    while ((ch = (char)input.Peek()) != '"')
                    {
                        accum.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            throw new System.Exception("unterminated string literal");
                        }
                    }

                    // skip the terminating "
                    input.Read();
                    this.result.Add(accum);
                }
                else if (char.IsDigit(ch))
                {
                    // numeric literal

                    StringBuilder accum = new StringBuilder();

                    while (char.IsDigit(ch))
                    {
                        accum.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            break;
                        }
                        else
                        {
                            ch = (char)input.Peek();
                        }
                    }

                    this.result.Add(int.Parse(accum.ToString()));
                }
                else switch (ch)
                    {
                        case '+':
                            input.Read();
                            this.result.Add(Scanner.Add);
                            break;

                        case '-':
                            input.Read();
                            this.result.Add(Scanner.Sub);
                            break;

                        case '*':
                            input.Read();
                            this.result.Add(Scanner.Mul);
                            break;

                        case '/':
                            input.Read();
                            this.result.Add(Scanner.Div);
                            break;

                        case '=':
                            input.Read();
                            this.result.Add(Scanner.Equal);
                            break;

                        case ';':
                            input.Read();
                            this.result.Add(Scanner.Semi);
                            break;

                        default:
                            throw new System.Exception("Scanner encountered unrecognized character '" + ch + "'");
                    }

            }
        }
    }

    public class Parser
    {
        private int index;
        private IList<object> tokens;
        private readonly Stmt result;

        public Parser(IList<object> tokens)
        {
            this.tokens = tokens;
            this.index = 0;
            this.result = this.ParseStmt();

            if (index != this.tokens.Count)
                throw new System.Exception("expected EOF");
        }

        public Stmt Result
        {
            get { return result; }
        }

        private Stmt ParseStmt()
        {
            Stmt result;

            if (this.index == this.tokens.Count)
            {
                throw new System.Exception("expected statement, got EOF");
            }

            // <stmt> := print <expr> 

            // <expr> := <string>
            // | <int>
            // | <arith_expr>
            // | <ident>
            if (this.tokens[this.index].Equals("print"))
            {
                this.index++;
                Print print = new Print();
                print.Expr = this.ParseExpr();
                result = print;
            }
            else if (this.tokens[this.index].Equals("var"))
            {
                this.index++;
                DeclareVar declareVar = new DeclareVar();

                if (index < tokens.Count &&
                    this.tokens[this.index] is string)
                {
                    declareVar.Ident = (string)this.tokens[this.index];
                }
                else
                {
                    throw new System.Exception("expected variable name after 'var'");
                }

                this.index++;

                if (index == tokens.Count ||
                    this.tokens[this.index] != Scanner.Equal)
                {
                    throw new System.Exception("expected = after 'var ident'");
                }

                this.index++;

                declareVar.Expr = this.ParseExpr();
                result = declareVar;
            }
            else if (this.tokens[this.index].Equals("read_int"))
            {
                this.index++;
                ReadInt readInt = new ReadInt();

                if (this.index < tokens.Count &&
                    tokens[this.index] is string)
                {
                    readInt.Ident = (string)this.tokens[this.index++];
                    result = readInt;
                }
                else
                {
                    throw new System.Exception("expected variable name after 'read_int'");
                }
            }
            else if (this.tokens[this.index].Equals("for"))
            {
                this.index++;
                ForLoop forLoop = new ForLoop();

                if (this.index < this.tokens.Count &&
                    this.tokens[this.index] is string)
                {
                    forLoop.Ident = (string)this.tokens[this.index];
                }
                else
                {
                    throw new System.Exception("expected identifier after 'for'");
                }

                this.index++;

                if (index == tokens.Count ||
                    this.tokens[this.index] != Scanner.Equal)
                {
                    throw new System.Exception("for missing '='");
                }

                this.index++;

                forLoop.From = this.ParseExpr();

                if (this.index == this.tokens.Count ||
                    !this.tokens[this.index].Equals("to"))
                {
                    throw new System.Exception("expected 'to' after for");
                }

                this.index++;

                forLoop.To = this.ParseExpr();

                if (this.index == this.tokens.Count ||
                    !this.tokens[this.index].Equals("do"))
                {
                    throw new System.Exception("expected 'do' after from expression in for loop");
                }

                this.index++;

                forLoop.Body = this.ParseStmt();
                result = forLoop;

                if (this.index == this.tokens.Count ||
                    !this.tokens[this.index].Equals("end"))
                {
                    throw new System.Exception("unterminated 'for' loop body");
                }

                this.index++;
            }
            else if (this.tokens[this.index] is string)
            {
                // assignment

                Assign assign = new Assign();
                assign.Ident = (string)this.tokens[this.index++];

                if (this.index == this.tokens.Count ||
                    this.tokens[this.index] != Scanner.Equal)
                {
                    throw new System.Exception("expected '='");
                }

                this.index++;

                assign.Expr = this.ParseExpr();
                result = assign;
            }
            else
            {
                throw new System.Exception("parse error at token " + this.index + ": " + this.tokens[this.index]);
            }

            if (this.index < this.tokens.Count && this.tokens[this.index] == Scanner.Semi)
            {
                this.index++;

                if (this.index < this.tokens.Count &&
                    !this.tokens[this.index].Equals("end"))
                {
                    Sequence sequence = new Sequence();
                    sequence.First = result;
                    sequence.Second = this.ParseStmt();
                    result = sequence;
                }
            }

            return result;
        }

        private Expr ParseExpr()
        {
            if (this.index == this.tokens.Count)
            {
                throw new System.Exception("expected expression, got EOF");
            }

            if (this.tokens[this.index] is StringBuilder)
            {
                string value = ((StringBuilder)this.tokens[this.index++]).ToString();
                StringLiteral stringLiteral = new StringLiteral();
                stringLiteral.Value = value;
                return stringLiteral;
            }
            else if (this.tokens[this.index] is int)
            {
                int intValue = (int)this.tokens[this.index++];
                IntLiteral intLiteral = new IntLiteral();
                intLiteral.Value = intValue;
                return intLiteral;
            }
            else if (this.tokens[this.index] is string)
            {
                string ident = (string)this.tokens[this.index++];
                Variable var = new Variable();
                var.Ident = ident;
                return var;
            }
            else
            {
                throw new System.Exception("expected string literal, int literal, or variable");
            }
        }

    }


}
