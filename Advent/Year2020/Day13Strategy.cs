using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Advent;

namespace Advent.Year2020
{
	class Day13Strategy : DayLineLoaderBasic
	{
		int m_earliest = 0;

		List<int> m_buses = new List<int>();

		List<int> m_buses2 = new List<int>();

		public override void ParseInputLine(string line)
		{
			if (m_earliest == 0)
				m_earliest = int.Parse(line);
			else
			{
				var splits = line.Split(',');
				foreach(var s in splits)
				{
					int o = 0;
					bool success = int.TryParse(s, out o);
					if (success)
					{
						m_buses.Add(o);
						m_buses2.Add(o);
					}
					else
					{
						m_buses2.Add(-1);
					}
				}
			}
		}

		public override string Part1()
		{
			var offsets = m_buses.Select(x => m_earliest % x).ToList();

			int wait = 0;
			while(!offsets.Any(x => x == 0))
			{
				offsets = offsets.Select(x => x + 1).ToList();
				for (int i = 0; i < offsets.Count(); i++)
				{
					offsets[i] = offsets[i] % m_buses[i];
				}
				wait++;
			}

			int id = 0;
			for(int i=0; i < offsets.Count(); i++)
			{
				if (offsets.ElementAt(i) == 0)
					id = m_buses[i];
			}

			return (id * wait).ToString();
		}

		private BigInteger invMod(BigInteger x, BigInteger M)
		{
			return BigInteger.ModPow(x, M - 2, M);
		}


		private BigInteger ChineseRemainderGauss(List<int> mods, BigInteger prodMods, List<int> remainders)
		{
			BigInteger result = 0;

			for(int i=0; i < mods.Count(); i++)
			{
				var ai = remainders[i];
				var ni = mods[i];
				var bi = prodMods / ni;

				result += ai * bi * invMod(bi, ni);
			}

			return result % prodMods;
		}

		public override string Part2()
		{

			// TEST
			//m_buses = new List<int>(new int[] { 17, 13, 19 });
			//m_buses2 = new List<int>(new int[] { 17, -1, 13, 19 });
			//m_buses = new List<int>(new int[] { 1789, 37, 47, 1889 });
			//m_buses2 = new List<int>(new int[] { 1789, 37, 47, 1889 });
			// END TEST

			var remainders = new List<int>();

			int j = 0;
			foreach (var b in m_buses)
			{
				for (int i = 0; i < m_buses2.Count; i++)
				{
					if (b == m_buses2[i])
					{
						remainders.Add(b - i);
						break;
					}
				}
				j++;
			}

			BigInteger prod = m_buses.Aggregate(new BigInteger(1), (s, x) => s * x);

			BigInteger answer = ChineseRemainderGauss(m_buses, prod, remainders);

			return answer.ToString();
		}
	}
}
