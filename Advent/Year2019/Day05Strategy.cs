using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day05Strategy : DayInputFileStrategy
	{
		long[] m_data;

		public Day05Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			long[] data = (long[])m_data.Clone();
			List<long> outputs = new List<long>();

			IntCode ic = new IntCode(data, () => true, () => 1, (x) => outputs.Add(x));

			ic.Run();

			return outputs.Last().ToString();
		}

		public override string Part2()
		{
			long[] data = (long[])m_data.Clone();

			//int[] data = { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 }; // is input equal to 8
			//int[] data = { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 }; // is input less than 8
			//int[] data = { 3, 3, 1108, -1, 8, 3, 4, 3, 99 }; // is input equal to 8
			//int[] data = { 3, 3, 1107, -1, 8, 3, 4, 3, 99 }; // is input less than 8
			//int[] data = { 3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9 }; // is input zero
			//int[] data = { 3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1 }; // is input zero

			List<long> outputs = new List<long>();
			IntCode ic = new IntCode(data, () => true, () => 5, (x) => outputs.Add(x));

			ic.Run();

			return outputs.Last().ToString();
		}

		protected override void ParseInputLine(string line, int lineNum)
		{
			m_data = line.Split(',').Select(s => long.Parse(s)).ToArray();
		}
	}
}
