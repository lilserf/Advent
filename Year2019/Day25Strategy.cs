using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day25Strategy : DayIntCodeStrategy
	{
		public Day25Strategy(string inputFile) : base(inputFile)
		{
		}

		public override string Part1()
		{
			var program = CreateProgram(() => true, GetInput, (x) => Console.Write((char)x));
			program.Run();
			return "";
		}

		Queue<char> m_inputs = new Queue<char>();

		private long GetInput()
		{
			if(m_inputs.Count == 0)
			{
				string command = Console.ReadLine();
				foreach (var c in command)
				{
					m_inputs.Enqueue(c);
				}
				m_inputs.Enqueue('\n');
			}

			return m_inputs.Dequeue();
		}

		public override string Part2()
		{
			return "";
		}
	}
}
