using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day02Strategy : DayInputFileStrategy
	{
		int[] m_data;

		public Day02Strategy(string file) : base(file)
		{}

		private void Run(int[] data)
		{
			int pc = 0;

			while (true)
			{
				// 99 is End Program
				if (data[pc] == 99)
					break;

				switch (data[pc])
				{
					// 1 is Add
					case 1:
						{
							int loc1 = data[pc + 1];
							int loc2 = data[pc + 2];
							int dest = data[pc + 3];

							data[dest] = data[loc1] + data[loc2];
							break;
						}
					// 2 is Multiply
					case 2:
						{
							int loc1 = data[pc + 1];
							int loc2 = data[pc + 2];
							int dest = data[pc + 3];

							data[dest] = data[loc1] * data[loc2];
							break;
						}
				}

				pc += 4;
			}
		}

		public override string Part1()
		{
			int[] data =(int[]) m_data.Clone();

			data[1] = 12;
			data[2] = 2;

			Run(data);

			return data[0].ToString();
		}

		public override string Part2()
		{
			for(int noun = 0; noun < 100; noun++)
			{
				for(int verb = 0; verb < 100; verb++)
				{
					int[] data = (int[])m_data.Clone();

					data[1] = noun;
					data[2] = verb;

					Run(data);

					if (data[0] == 19690720)
						return (100 * noun + verb).ToString();
				}
			}

			return "Error";
		}

		protected override void ParseInputLine(string line)
		{
			m_data = line.Split(',').Select(s => int.Parse(s)).ToArray();
		}
	}
}
