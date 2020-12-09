using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day08Strategy : DayStrategyBasic
	{

		IEnumerable<string> m_lines;

		public Day08Strategy(string file)
		{
			m_lines = File.ReadAllLines(file);
		}


		public override string Part1()
		{
			Handheld handheld = new Handheld(m_lines);

			HashSet<int> executed = new HashSet<int>();
			executed.Add(0);

			while(true)
			{
				(int newPc, long accum) = handheld.Step();
				if (executed.Contains(newPc))
				{
					return accum.ToString();
				}
				executed.Add(newPc);
			}

		}

		public override string Part2()
		{
			int index = 0;

			while(true)
			{
				var lines = m_lines.ToList();
				while(!lines[index].StartsWith("jmp"))
				{
					index++;
				}
				lines[index] = lines[index].Replace("jmp", "nop");

				Handheld handheld = new Handheld(lines);
				(int err, long acc) = handheld.Run(500);

				if (err == 0)
					return acc.ToString();
				else
					index++;
			}

		}
	}
}
