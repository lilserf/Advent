using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Day15Strategy : DayLineLoaderBasic
	{
		List<int> m_data = new List<int>();
		public override void ParseInputLine(string line)
		{
			var splits = line.Split(',');

			foreach(var s in splits)
			{
				m_data.Add(int.Parse(s));
			}
		}


		public override string Part1()
		{
			//m_data = new List<int>(new int[] { 0, 3, 6 });
			return Run(2020);
		}

		private string Run(int iter)
		{ 
			Dictionary<int, (int, int)> lookup = new Dictionary<int, (int, int)>();
			int last = 0;

			int turn = 0;
			foreach(var x in m_data)
			{
				lookup[x] = (-1, turn);
				turn++;
				last = x;
			}

			while(turn < iter)
			{
				int thisX;
				var lastTime = lookup[last];
				if(lastTime.Item1 < 0)
				{
					thisX = 0;
				}
				else
				{
					thisX = lastTime.Item2 - lastTime.Item1;
				}

				int prev = lookup.ContainsKey(thisX) ? lookup[thisX].Item2 : -1;
				lookup[thisX] = (prev, turn);
				turn++;
				last = thisX;
				//Console.WriteLine(thisX);
			}

			return last.ToString();
		}

		public override string Part2()
		{
			//m_data = new List<int>(new int[] { 0, 3, 6 });
			return Run(30000000);
		}
	}
}
