using Advent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2023
{
	internal class Day08 : DayLineLoaderBasic
	{
		string m_directions;
		Dictionary<string, (string, string)> m_links = new();

		static Regex m_linkRegex = new Regex(@"(\w\w\w).*\((\w\w\w), (\w\w\w)\)");

		public override void ParseInputLine(string line, int lineNum)
		{
			if(lineNum == 0)
			{
				m_directions = line;
			}
			else
			{
				if (string.IsNullOrEmpty(line))
					return;

				if(m_linkRegex.IsMatch(line))
				{
					var groups = m_linkRegex.Match(line).Groups;
					var key = groups[1].Value;
					m_links[key] = (groups[2].Value, groups[3].Value);
				}
			}
		}

		public override string Part1()
		{
			string curr = "AAA";
			string target = "ZZZ";

			int currDir = 0;
			int steps = 0;
			while(curr != target)
			{
				var map = m_links[curr];
				if (m_directions[currDir] == 'L')
					curr = map.Item1;
				else
					curr = map.Item2;

				currDir++;
				currDir = currDir % m_directions.Count();
				steps++;
			}

			return steps.ToString();
		}

		public (int, int) FindZ(string start)
		{
			(int first, int second) = (0, 0);
			string curr = start;
            int currDir = 0;
            int steps = 0;
            while (true)
            {
                var map = m_links[curr];
                if (m_directions[currDir] == 'L')
                    curr = map.Item1;
                else
                    curr = map.Item2;

				if(curr.EndsWith('Z'))
				{
					if (first == 0)
					{
						first = steps;
					}
					else if (second == 0)
					{
						second = steps;
						return (first, second);
					}
				}

                currDir++;
                currDir = currDir % m_directions.Count();
                steps++;
            }
        }

		public override string Part2()
		{
			var starts = (m_links.Keys.Where(x => x.EndsWith('A')));

			var steps = starts.Select(FindZ).ToList();

			for(int i=0; i < starts.Count(); i++)
			{
				Console.WriteLine($"{starts.ElementAt(i)} hits Z at {steps.ElementAt(i).Item1} and {steps.ElementAt(i).Item2}");
			}

			long mult = 1;
			foreach(var x in steps)
			{
				// some kind of off-by-one in FindZ means I need this +1
				mult = LcmGcd.lcm(mult, x.Item1+1);
			}

			return mult.ToString();
		}
	}
}
