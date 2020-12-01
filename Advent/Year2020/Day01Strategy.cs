using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day01Strategy : DayInputFileStrategy
	{
		public Day01Strategy(string file)
			:base(file)
		{
			m_numbers = new List<int>();
		}

		List<int> m_numbers;

		public override void Reset()
		{
		}

		protected override void ParseInputLine(string line)
		{
			m_numbers.Add(int.Parse(line));
		}

		public override string Part1()
		{
			for(int i = 0; i < m_numbers.Count; i++)
			{
				for(int j=0; j < m_numbers.Count; j++)
				{
					if(i != j && m_numbers[i] + m_numbers[j] == 2020)
					{
						return $"{m_numbers[i] * m_numbers[j]}";
					}
					
				}
			}

			return "Not found";
		}

		public override string Part2()
		{
			for (int i = 0; i < m_numbers.Count; i++)
			{
				for (int j = 0; j < m_numbers.Count; j++)
				{
					for (int k = 0; k < m_numbers.Count; k++)
					{
						if (i == j || j == k || k == i) continue;

						var a = m_numbers[i];
						var b = m_numbers[j];
						var c = m_numbers[k];


						if(a+b+c == 2020)
						{
							return $"{a * b * c}";
						}
					}
				}
			}
			return "Not found";
		}

	}
}
