using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Day05Strategy : DayInputFileStrategy
	{
		int NUM_ROWS = 128;
		public Day05Strategy(string file) : base(file)
		{

		}

		int ComputeSpaceNum(string data, int numChars, int min, int max, char minChar, char maxChar)
		{
			Queue<char> steps = new Queue<char>(data.Take(numChars));

			while(steps.Count > 0)
			{
				var c = steps.Dequeue();
				int range = (max - min + 1);
				if (c == minChar)
				{
					max = min + range / 2;
				}
				else if (c == maxChar)
				{
					min = min + range / 2;
				}
				else throw new Exception("wha?");
			}

			return min;
		}

		Dictionary<string, int> m_passes = new Dictionary<string, int>();

		protected override void ParseInputLine(string line, int lineNum)
		{
			int row = ComputeSpaceNum(line, 7, 0, NUM_ROWS-1, 'F', 'B');
			int col = ComputeSpaceNum(line.Substring(7), 3, 0, 7, 'L', 'R');

			m_passes[line] = row * 8 + col;
		}

		public override string Part1()
		{
			return m_passes.Values.Max().ToString();
		}

		bool[][] m_map;

		public override string Part2()
		{
			var sorted = m_passes.Values.OrderBy(x => x).ToList();

			int prev = -1;
			foreach (var seat in sorted)
			{
				if(prev != -1)
				{
					var diff = seat - prev;
					if(diff > 1)
						return ($"Empty seat at {prev+1}");
				}
				prev = seat;
			}

			return "No seat found";
		}
	}
}
