using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Day16Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		public override void ParseInputLine(string line)
		{
			var splits = line.Split(',');
			m_data.AddRange(splits);
		}

		char[] m_line = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };
		int m_front = 0;

		private void Swap(int i1, int i2)
		{
			int r1 = (i1 + m_front) % 16;
			int r2 = (i2 + m_front) % 16;

			char temp = m_line[r1];
			m_line[r1] = m_line[r2];
			m_line[r2] = temp;
		}

		private int Find(char c)
		{
			for(int i=0; i < 16; i++)
			{
				if(m_line[i] == c)
				{
					return (i + 16 - m_front) % 16;
				}
			}

			return -1;
		}

		private string GetLine()
		{
			string s = "";
			for (int i = 0; i < 16; i++)
			{
				int idx = (m_front + i) % 16;
				s += m_line[idx];
			}
			return s;
		}

		private void DoDance()
		{
			foreach (var instr in m_data)
			{
				if (instr[0] == 's')
				{
					m_front = m_front + 16 - int.Parse(instr.Substring(1));
					m_front %= 16;
				}
				else if (instr[0] == 'x')
				{
					var matches = Regex.Match(instr, @"x(\d+)/(\d+)");
					if (matches.Success)
					{
						int i1 = int.Parse(matches.Groups[1].Value);
						int i2 = int.Parse(matches.Groups[2].Value);

						Swap(i1, i2);
					}
				}
				else if (instr[0] == 'p')
				{
					char c1 = instr[1];
					char c2 = instr[3];

					Swap(Find(c1), Find(c2));
				}
				//Console.WriteLine($"{instr}:\t{GetLine()}");
				//Console.ReadKey();
			}
		}

		public override string Part1()
		{
			//Console.WriteLine($"Start:\t{GetLine()}");

			DoDance();

			return GetLine();
		}


		public override string Part2()
		{
			List<string> set = new List<string>();
			set.Add("abcdefghijklmnop");

			for(int i=0; i < 100000; i++)
			{
				var s = GetLine();
				if(!set.Contains(s))
				{
					set.Add(s);
				}
				else
				{
					break;
				}
				DoDance();
			}

			return set.ElementAt(1000000000 % set.Count());
		}
	}
}
