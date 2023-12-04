using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	abstract class Expr
	{
		public abstract long Evaluate();
		public abstract void Fill(Expr ex);
		public abstract bool CanFill();
	}

	class PendingExpr : Expr
	{
		public string Text { get; set; }
		public PendingExpr(string text)
		{
			Text = text;
		}

		public override long Evaluate()
		{
			throw new NotImplementedException();
		}

		public override void Fill(Expr ex)
		{
			throw new NotImplementedException();
		}

		public override bool CanFill()
		{
			throw new NotImplementedException();
		}
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

		private Expr ParseExpr(string line)
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

		public override void ParseInputLine(string line, int lineNum)
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

		private Expr ParseAddExpr(string line)
		{
			OperatorExpr add = new OperatorExpr('+');
			add.Left = new ConstantExpr(0);
			var splits = line.Split('+');
			foreach(var num in splits)
			{
				int val = int.Parse(num);
				add.Fill(new ConstantExpr(val));
				OperatorExpr newAdd = new OperatorExpr('+');
				newAdd.Left = add;
				add = newAdd;
			}

			add.Right = new ConstantExpr(0);
			return add;
		}

		private Expr ParseSubExpr(string line)
		{
			var splits = line.Split('*');
			OperatorExpr mult = new OperatorExpr('*');
			mult.Left = new ConstantExpr(1);

			foreach(var ex in splits)
			{
				mult.Fill(ParseAddExpr(ex));
				var newMult = new OperatorExpr('*');
				newMult.Left = mult;
				mult = newMult;
			}

			mult.Right = new ConstantExpr(1);
			return mult;
		}

		private long SolvePartB(string line)
		{
			Stack<string> exprStack = new Stack<string>();
			exprStack.Push("");

			foreach(char c in line)
			{
				if (c == ' ') continue;

				if(c == '(')
				{
					exprStack.Push("");
				}
				else if(c == ')')
				{
					var top = exprStack.Pop();
					Expr sub = ParseSubExpr(top);
					long answer = sub.Evaluate();
					top = exprStack.Pop();
					top += answer.ToString();
					exprStack.Push(top);
				}
				else
				{
					var top = exprStack.Pop();
					top += c;
					exprStack.Push(top);
				}
			}

			var final = exprStack.Pop();
			return ParseSubExpr(final).Evaluate();
		}


		public override string Part2()
		{
			Console.WriteLine(SolvePartB("1 + 2 * 3 + 4 * 5 + 6"));
			Console.WriteLine(SolvePartB("1 + (2 * 3) + (4 * (5 + 6))"));
			Console.WriteLine(SolvePartB("5 + (8 * 3 + 9 + 3 * 4 * 3)")); 
			Console.WriteLine(SolvePartB("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))")); 
			Console.WriteLine(SolvePartB("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"));

			return m_data.Sum(x => SolvePartB(x)).ToString();
		}
	}
}
