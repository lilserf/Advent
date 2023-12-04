using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	enum ShuffleOp
	{
		NewStack,
		Cut,
		Increment
	}

	struct ShuffleStep
	{
		public ShuffleOp Op;
		public int Quantity;

		public ShuffleStep(ShuffleOp op, int q)
		{
			Op = op;
			Quantity = q;
		}

		public override string ToString()
		{
			switch(Op)
			{
				case ShuffleOp.Cut:
					return $"cut {Quantity}";
				case ShuffleOp.NewStack:
					return $"deal into new stack";
				case ShuffleOp.Increment:
					return $"deal with increment {Quantity}";
			}

			return "Error?!";
		}
	}



	// Of pattern x' = (Ax + B) mod M
	class Transform
	{
		public long A;
		public long B;
		public long M;

		public Transform(long a, long b, long m)
		{
			A = a;
			B = b;
			M = m;
		}

		public Transform(Transform other)
		{
			A = other.A;
			B = other.B;
			M = other.M;
		}

		public void Fold(Transform other)
		{
			long newA = (long)BigInteger.Remainder(M + BigInteger.Multiply(A, other.A), M);
			long newB = (long)BigInteger.Remainder(M + BigInteger.Multiply(other.A, B) + other.B, M);

			A = newA;
			B = newB;
		}

		public void FoldSelf(long count)
		{
			long newA = (long)BigInteger.ModPow(A, count, M);

			long divisor = Inverse(1 - A);

			var dividend = BigInteger.Multiply(B, (1 - newA));
			var divided = BigInteger.Multiply(dividend, divisor);

			long newB = (long)BigInteger.Remainder(M + divided, M);

			A = newA;
			B = newB;
		}

		private long Inverse(long x)
		{
			return (long)BigInteger.ModPow(x, M - 2, M);
		}

		public long TransformIndex(long x)
		{
			return ((A * x + B) + M) % M;
		}

		public long InverseTransformIndex(long x)
		{
			long inverseA = Inverse(A);
			long xMinusB = M + (x - B) % M;
			return (long)BigInteger.Remainder(M + BigInteger.Multiply(xMinusB, inverseA), M);
		}

		public static Transform FromStep(ShuffleStep s, long deckSize)
		{
			switch(s.Op)
			{
				case ShuffleOp.NewStack:
					return new Transform(-1, -1, deckSize);
				case ShuffleOp.Cut:
					return new Transform(1, -s.Quantity, deckSize);
				case ShuffleOp.Increment:
					return new Transform(s.Quantity, 0, deckSize);
				default:
					throw new InvalidOperationException();
			}
		}
	}

	class Day22Strategy : DayInputFileStrategy
	{
		Queue<ShuffleStep> m_steps;
		static int NUM_CARDS = 10007;

		public Day22Strategy(string inputFile) : base(inputFile)
		{
			m_steps = new Queue<ShuffleStep>();
		}

		private void Print(int[] test)
		{
			Console.WriteLine(test.Aggregate("", (s, x) => s + x + " "));
		}

		public override string Part1()
		{
			Transform folded = new Transform(1, 0, NUM_CARDS);

			foreach(var step in m_steps)
			{
				Transform thisStep = Transform.FromStep(step, NUM_CARDS);
				folded.Fold(thisStep);
			}

			long finalIndex = folded.TransformIndex(2019);

			long test = folded.InverseTransformIndex(finalIndex);
			Console.WriteLine($"Inverted: {test}");
			return finalIndex.ToString();
		}

		public override string Part2()
		{
			long NUM_CARDS_PART2 = 119315717514047L;
			long NUM_SHUFFLES = 101741582076661L;

			Transform part2 = new Transform(1, 0, NUM_CARDS_PART2);
			foreach (var step in m_steps)
			{
				Transform thisStep = Transform.FromStep(step, NUM_CARDS_PART2);
				part2.Fold(thisStep);
			}

			Transform folded = new Transform(part2);
			folded.FoldSelf(NUM_SHUFFLES);

			return folded.InverseTransformIndex(2020).ToString();
		}

		protected override void ParseInputLine(string line, int lineNum)
		{
			ShuffleStep s = new ShuffleStep();
			if(line.StartsWith("cut")) // cut X
			{
				s.Op = ShuffleOp.Cut;
				s.Quantity = int.Parse(line.Substring(4));
			}
			else if(line.StartsWith("deal into new")) // deal into new stack
			{
				s.Op = ShuffleOp.NewStack;
				s.Quantity = 0;
			}
			else if(line.StartsWith("deal with increment")) // deal with increment
			{
				s.Op = ShuffleOp.Increment;
				s.Quantity = int.Parse(line.Substring(20));
			}

			if (s.ToString() != line.Trim())
				throw new Exception();

			m_steps.Enqueue(s);
		}
	}
}
