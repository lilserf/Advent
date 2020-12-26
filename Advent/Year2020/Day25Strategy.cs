using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day25Strategy : DayLineLoaderBasic
	{
		List<long> m_data = new List<long>();

		public override void ParseInputLine(string line)
		{
			m_data.Add(long.Parse(line));
		}

		long Transform(long subjectNumber, long loopSize)
		{
			long value = 1;
			for (long i = 0; i < loopSize; i++)
			{
				value *= subjectNumber;
				value %= 20201227;
			}

			return value;
		}

		long FindLoopSize(long subjectNumber, long key)
		{
			long i = 1;
			long value = 1;
			while(true)
			{
				value *= subjectNumber;
				value %= 20201227;

				if(value == key)
				{
					return i;
				}

				i++;
			}
		}


		public override string Part1()
		{
			long cardPubKey = m_data.ElementAt(0);
			long doorPubKey = m_data.ElementAt(1);

			long cardLoopSize = FindLoopSize(7, cardPubKey);
			long doorLoopSize = FindLoopSize(7, doorPubKey);

			Console.WriteLine($"Card loop size is {cardLoopSize}; door loop size is {doorLoopSize}");

			long cardEncKey = Transform(doorPubKey, cardLoopSize);
			long doorEncKey = Transform(cardPubKey, doorLoopSize);

			return $"{cardEncKey} == {doorEncKey}";

		}

		public override string Part2()
		{
			return "";
		}
	}
}
