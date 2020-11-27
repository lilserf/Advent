using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day04Strategy : IDayStrategy
	{
		int m_min;
		int m_max;

		List<int> m_possible = new List<int>();

		public Day04Strategy(int min, int max)
		{
			m_min = min;
			m_max = max;
		}

		public void Initialize()
		{
		}

		private static int GetDigit(int num, int digit)
		{
			digit = 6 - digit;
			return (num % (int)(Math.Pow(10,digit)) / (int)Math.Pow(10,digit-1));
		}

		public string Part1()
		{
			for(int check = m_min; check <= m_max; check++)
			{
				int prev = GetDigit(check, 0);

				bool doubled = false;
				bool increasing = true;

				for(int i=1; i < 6; i++)
				{
					int curr = GetDigit(check, i);

					if (prev == curr)
						doubled = true;

					if (curr < prev)
						increasing = false;

					prev = curr;
				}

				if (doubled && increasing)
					m_possible.Add(check);
			}

			return m_possible.Count.ToString();
		}

		public string Part2()
		{
			int count = 0;

			foreach(int check in m_possible)
			{
				int prev = GetDigit(check, 0);

				List<int> runs = new List<int>();

				int run = 1;

				for(int i=1; i<6; i++)
				{
					int curr = GetDigit(check, i);

					if (prev == curr)
					{
						run++;
					}
					else
					{
						runs.Add(run);
						run = 1;
					}

					prev = curr;
				}
				runs.Add(run);

				if (runs.Contains(2))
				{
					count++;
				}
				else
				{
					Console.WriteLine(check.ToString());
				}
			}

			return count.ToString();
		}

		public void Reset()
		{
		}
	}
}
