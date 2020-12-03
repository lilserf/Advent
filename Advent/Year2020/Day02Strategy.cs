using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class PasswordPolicy
	{
		char Letter { get;  }
		int Min { get;  }
		int Max { get; }

		public PasswordPolicy(char letter, int min, int max)
		{
			Letter = letter;
			Min = min;
			Max = max;
		}

		public bool Allowed(string password)
		{
			var count = password.Count(c => c == Letter);
			if (count >= Min && count <= Max)
				return true;
			else
				return false;
		}

		public bool Allowed2(string password)
		{
			bool first = password[Min - 1] == Letter;
			bool second = password[Max - 1] == Letter;

			return first ^ second;
		}
	}

	class Day02Strategy : DayInputFileStrategy
	{
		List<(PasswordPolicy, string)> m_data;

		public Day02Strategy(string file)
			: base(file)
		{
			m_data = new List<(PasswordPolicy, string)>();
		}

		protected override void ParseInputLine(string line)
		{
			var match = Regex.Match(line, @"(\d+)\-(\d+) ([a-z]): (.+)");

			var policy = new PasswordPolicy(match.Groups[3].Value[0], int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
			m_data.Add((policy, match.Groups[4].Value));
		}

		public override string Part1()
		{
			int count = 0;
			foreach(var x in m_data)
			{
				if (x.Item1.Allowed(x.Item2))
					count++;
			}
			return $"{count}";
		}

		public override string Part2()
		{
			int count = 0;
			foreach(var x in m_data)
			{
				if (x.Item1.Allowed2(x.Item2))
					count++;
			}
			return $"{count}";
		}
	}
}
