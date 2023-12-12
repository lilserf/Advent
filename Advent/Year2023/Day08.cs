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

		public override string Part2()
		{
			Queue<string> curr = new Queue<string>(m_links.Keys.Where(x => x.EndsWith('A')));

			int steps = 0;
			int currDir = 0;
			while (curr.Any(x => !x.EndsWith('Z')))
			{
				var c = curr.Dequeue();

				//if(!c.EndsWith("Z"))
				{
					var map = m_links[c];
					if (m_directions[currDir] == 'L')
						c = map.Item1;
					else
						c = map.Item2;
					curr.Enqueue(c);
				}

				currDir++;
				currDir = currDir % m_directions.Count();
				steps++;
			}

			return steps.ToString();
		}
	}
}
