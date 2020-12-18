using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	abstract class Expr
	{
		public abstract long Evaluate();
		public abstract void Fill(Expr ex);
		public abstract bool CanFill();
	}

	class ConstantExpr : Expr
	{
		long m_value;

		public ConstantExpr(long value)
		{
			m_value = value;
		}

		public override bool CanFill()
		{
			return false;
		}

		public override long Evaluate()
		{
			return m_value;
		}

		public override void Fill(Expr ex)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return m_value.ToString();
		}
	}

	class OperatorExpr : Expr
	{
		char m_op;

		public Expr Left { get; set; }
		public Expr Right { get; set; }

		public OperatorExpr(char op)
		{
			m_op = op;
		}

		public override long Evaluate()
		{
			switch(m_op)
			{
				case '+': return Right.Evaluate() + Left.Evaluate();
				case '*': return Right.Evaluate() * Left.Evaluate();
			}

			throw new Exception("bwuh?");
		}

		public override string ToString()
		{
			return $"({Left} {m_op} {Right})";
		}

		public override void Fill(Expr ex)
		{
			if(Right == null)
			{
				Right = ex;
			}
			else
			{
				Right.Fill(ex);
			}
		}

		public override bool CanFill()
		{
			return Right == null || Right.CanFill();
		}
	}

	class GroupExpr : Expr
	{
		public Expr Inner { get; set; }

		public override bool CanFill()
		{
			return Inner == null || Inner.CanFill();
		}

		public override long Evaluate()
		{
			return Inner.Evaluate();
		}

		public override void Fill(Expr ex)
		{
			if (Inner == null)
			{
				Inner = ex;
			}
			else
			{
				Inner.Fill(ex);
			}
		}

		public override string ToString()
		{
			return $"({Inner})";
		}
	}


	class Day18Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		List<Expr> m_exprs = new List<Expr>();

		private Expr ParseExpr(string line, bool partB)
		{
			Stack<Expr> exprStack = new Stack<Expr>();
			exprStack.Push(new GroupExpr());

			foreach (char c in line)
			{
				if (c == ' ') continue;

				int value;
				Expr newExpr = null;

				if (int.TryParse($"{c}", out value))
				{
					// It's a digit
					newExpr = new ConstantExpr(value);
				}
				else if (c == '+' || c == '*')
				{
					newExpr = new OperatorExpr(c);
					((OperatorExpr)newExpr).Left = exprStack.Peek();
				}
				else if (c == '(')
				{
					newExpr = new GroupExpr();
					exprStack.Push(newExpr);
					continue;
				}
				else if (c == ')')
				{
					var finishedGroup = exprStack.Pop();
					exprStack.Peek().Fill(finishedGroup);
					continue;
				}

				// Slot new expression into existing expression
				if (exprStack.Peek().CanFill())
				{
					exprStack.Peek().Fill(newExpr);
				}
				else
				{
					exprStack.Pop();
					exprStack.Push(newExpr);
				}
			}

			if (exprStack.Count != 1) throw new InvalidOperationException("what do i return");
			return exprStack.Pop();
		}

		public override void ParseInputLine(string line)
		{
			m_data.Add(line);
		}

		public override string Part1()
		{
			foreach(var line in m_data)
			{
				m_exprs.Add(ParseExpr(line));
			}
			//var ex1 = ParseExpr("1 + 2 * 3 + 4 * 5 + 6");
			//Console.WriteLine(ex1.Evaluate());

			//var ex2 = ParseExpr("1 + (2 * 3) + (4 * (5 + 6))");
			//Console.WriteLine(ex2.Evaluate());

			return m_exprs.Sum(x => x.Evaluate()).ToString();
		}

		public override string Part2()
		{
			return "";
		}
	}
}
