using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Day15Strategy : DayLineLoaderBasic
	{
		public override void ParseInputLine(string line)
		{
			
		}

		long m_initialA = 591;
		long m_initialB = 393;

		long m_factorA = 16807;
		long m_factorB = 48271;

		long m_mod = 2147483647;
		public override string Part1()
		{

			long aVal = m_initialA;
			long bVal = m_initialB;

			int count = 0;
			for(long i=0; i < 40000000; i++)
			{
				aVal = (aVal * m_factorA) % m_mod;
				bVal = (bVal * m_factorB) % m_mod;

				if((aVal & 0xFFFF) == (bVal & 0xFFFF))
				{
					count++;
				}
			}

			return count.ToString();
		}

		public override string Part2()
		{
			Queue<long> aQueue = new Queue<long>();
			Queue<long> bQueue = new Queue<long>();

			long aVal = m_initialA;
			long bVal = m_initialB;

			int pairCount = 0;
			int count = 0;
			while(pairCount < 5000000)
			{
				aVal = (aVal * m_factorA) % m_mod;
				bVal = (bVal * m_factorB) % m_mod;

				if(aVal % 4 == 0)
				{
					aQueue.Enqueue(aVal);
				}
				if(bVal % 8 == 0)
				{
					bQueue.Enqueue(bVal);
				}

				if (aQueue.Count >= 1 && bQueue.Count >= 1)
				{
					pairCount++;
					long aPending = aQueue.Dequeue();
					long bPending = bQueue.Dequeue();
					if ((aPending & 0xFFFF) == (bPending & 0xFFFF))
					{
						count++;
					}
				}
			}

			return count.ToString();
		}
	}
}
