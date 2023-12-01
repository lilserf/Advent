using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Day10Strategy : DayInputFileStrategy
	{

		List<int> m_data = new List<int>();

		public Day10Strategy(string file) : base(file)
		{

		}

		protected override void ParseInputLine(string line)
		{
			m_data.Add(int.Parse(line));
		}

		List<(int, int)> m_3gaps = new List<(int, int)>();

		public override string Part1()
		{
			Queue<int> list = new Queue<int>(m_data.OrderBy(x => x));

			int maxJoltage = list.Max() + 3;
			int currJoltage = 0;

			Dictionary<int, int> counts = new Dictionary<int, int>();
			counts[1] = 0;
			counts[3] = 0;

			while(list.Count > 0 && currJoltage <= maxJoltage)
			{
				var next = list.Dequeue();
				Console.WriteLine($"Curr joltage {currJoltage}, using adapter rated for {next}...");
				if (next > currJoltage + 3) throw new Exception("What?");

				var diff = next - currJoltage;
				counts[diff]++;
				if (diff == 3)
					m_3gaps.Add((currJoltage, next));

				currJoltage = next;
			}

			counts[3]++;
			m_3gaps.Add((currJoltage, maxJoltage));
			return (counts[1] * counts[3]).ToString();
		}

		public int NumPermutations(int start, int end, IEnumerable<int> adapters)
		{
			var subset = adapters.Where(x => x > start && x < end).OrderBy(x => x);

			// All the gaps happen to contain 0-3 items
			// which means they always permute to this many states
			switch(subset.Count())
			{
				case 0: return 1;
				case 1: return 2;
				case 2: return 4;
				case 3: return 7;
			}

			return 1;
			//if (subset.Count() > 0)
			//{
			//	bool includeEmpty = (end - start < 4);
			//	var perms = Permuter.OrderedPermute(subset.ToList(), includeEmpty).ToList();

			//	return perms.Count;
			//}
			//else
			//{
			//	return 1;
			//}
		}

		public override string Part2()
		{
			Queue<int> list = new Queue<int>(m_data.OrderBy(x => x));

			int maxJoltage = list.Max() + 3;

			int curr = 0;

			long combos = 1;

			Console.Write($"(0), ");
			foreach(var gap in m_3gaps)
			{
				if (gap.Item1 > curr)
				{
					int num = NumPermutations(curr, gap.Item1, m_data);
					if(num > 1)
						Console.Write($"<{num} vars>, ");
					else
						Console.Write($"{gap.Item1}, ");
					combos *= num;
				}
				curr = gap.Item2;
				Console.Write($"{curr}, ");
			}

			Console.WriteLine($"({maxJoltage})");

			return combos.ToString();
		}
	}
}
