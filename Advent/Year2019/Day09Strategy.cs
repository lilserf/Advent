using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day09Strategy : DayInputFileStrategy
	{
		long[] m_program;
		static bool s_test = false;

		public Day09Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			if (s_test)
			{
				long[] test1 = new long[] { 109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 };

				long[] test2 = new long[] { 1102, 34915192, 34915192, 7, 4, 7, 99, 0 };

				long[] test3 = new long[] { 104, 1125899906842624, 99 };


				Console.Write($"test1: ");
				var p1 = new IntCode(test1, () => true, () => 0, (x) => Console.Write(x + " "));
				p1.Run();
				Console.WriteLine();

				var p2 = new IntCode(test2, () => true, () => 0, (x) => Console.WriteLine($"test2: {x}"));
				p2.Run();

				var p3 = new IntCode(test3, () => true, () => 0, (x) => Console.WriteLine($"test3: {x}"));
				p3.Run();
			}

			string output = "";
			var program = new IntCode(m_program, () => true, () => 1, (x) => output += x.ToString() + " ");
			program.Run();
			return output;
		}

		public override string Part2()
		{
			string output = "";
			var program = new IntCode(m_program, () => true, () => 2, (x) => output += x.ToString() + " ");
			program.Run();
			return output;
		}

		protected override void ParseInputLine(string line)
		{
			m_program = line.Split(',').Select(s => long.Parse(s)).ToArray();
		}
	}
}
