using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Day09Strategy : DayInputFileStrategy
	{
		Queue<long> m_data = new Queue<long>();



		public Day09Strategy(string file) : base(file)
		{

		}

		protected override void ParseInputLine(string line)
		{
			m_data.Enqueue(long.Parse(line));
		}

		public override string Part1()
		{
			const int PREAMBLE = 25;

			var data = new Queue<long>(m_data);

			Queue<long> curr = new Queue<long>();

			// Take the preamble first
			for (int i = 0; i < 25; i++)
			{
				curr.Enqueue(data.Dequeue());
			}
			
			// Loop until out of data
			while(data.Count > 0)
			{
				long next = data.Dequeue();

				// Get the list of all pairs of numbers in our current set
				var pairs = Permuter.Pairs(curr);

				// Find if one of these pairs matches
				bool found = false;
				foreach(var p in pairs)
				{
					if(p.Item1 + p.Item2 == next)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					return next.ToString();
				}
				else
				{
					// Ditch the oldest and add the next number
					curr.Dequeue();
					curr.Enqueue(next);
				}
			}

			return "";
		}

		// Quick helper
		public long Sum(List<long> data, int start, int end)
		{
			long sum = 0;
			for(int i=start; i < end; i++)
			{
				sum += data.ElementAt(i);
			}
			return sum;
		}

		public override string Part2()
		{
			// Wheeee hardcoded
			long magicNum = 25918798;

			List<long> data = new List<long>(m_data);

			// Start from the big end of the list to minimize having to search 100 tiny numbers trying to add to 25 million
			int length = 2;
			int i = data.Count;
			while(i >= 0)
			{
				// Sum the range
				long sum = Sum(data, i - length, i);
				if(sum == magicNum)
				{
					// Get the range
					long start = data.ElementAt(i - length);
					long end = data.ElementAt(i - 1);
					var range = data.Skip(i - length).Take(length);
					// Find min and max (did this wrong at first)
					long min = range.Min();
					long max = range.Max();
					//Console.WriteLine($"Range from {i-length}({start}) to {i-1}({end}) sums to magic number");
					//Console.WriteLine($"Min is {min}, Max is {max}");
					return (min + max).ToString();
				}
				else if(sum > magicNum)
				{
					// If we went over, move the range down and reset the length to 2
					i--;
					length = 2;
				}
				else if(sum < magicNum)
				{
					// If we're still under, extend the range
					length++;
				}
			}

			return "";
		}
	}
}
