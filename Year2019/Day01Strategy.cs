using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day01Strategy : DayInputFileStrategy
	{
		List<int> m_masses = new List<int>();

		int m_massFuel = 0;

		public Day01Strategy(string file)
			: base(file)
		{ }

		private int computeFuel(int mass)
		{
			return (mass / 3) - 2;
		}

		public override string Part1()
		{
			m_massFuel = 0;
			foreach(var m in m_masses)
			{
				m_massFuel += computeFuel(m);
			}
			return m_massFuel.ToString();
		}

		public override string Part2()
		{
			int totalFuel = 0;

			foreach(var m in m_masses)
			{
				int moduleFuel = computeFuel(m);

				int overage = moduleFuel;
				while (overage > 0)
				{
					int newFuel = computeFuel(overage);
					if (newFuel > 0)
						moduleFuel += newFuel;
					overage = newFuel;
				}

				totalFuel += moduleFuel;
			}

			return totalFuel.ToString();
		}

		protected override void ParseInputLine(string line)
		{
			m_masses.Add(int.Parse(line));
		}
	}
}
