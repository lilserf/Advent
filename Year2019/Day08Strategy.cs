using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day08Strategy : DayInputFileStrategy
	{
		int WIDTH = 25;
		int HEIGHT = 6;

		List<string[]> m_layers = new List<string[]>();

		public Day08Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			int smallest = int.MaxValue;
			int value = 0;

			foreach(var l in m_layers)
			{
				int count = l.Sum(x => x.Count( c => c == '0'));
				if(count < smallest)
				{
					int ones = l.Sum(x => x.Count(c => c == '1'));
					int twos = l.Sum(x => x.Count(c => c == '2'));
					value = ones * twos;
					smallest = count;
				}
			}

			return value.ToString();
		}

		public override string Part2()
		{
			StringBuilder[] image = new StringBuilder[HEIGHT];

			for (int row = 0; row < HEIGHT; row++)
			{
				image[row] = new StringBuilder();
				for(int col = 0; col < WIDTH; col++)
				{
					image[row].Append('?');
					for(int layer = 0; layer < m_layers.Count; layer++)
					{
						bool b = false;
						switch(m_layers[layer][row][col])
						{
							case '0':
								image[row][col] = ' ';
								b = true;
								break;
							case '1':
								image[row][col] = '#';
								b = true;
								break;
						}
						if (b)
							break;
					}
				}
			}

			return image.Aggregate("", (s, x) => s += x + "\n");
		}

		protected override void ParseInputLine(string line)
		{
			int curr = 0;

			while (curr < line.Count())
			{
				string[] image = new string[HEIGHT];
				for (int row = 0; row < HEIGHT; row++)
				{
					image[row] = line.Substring(curr, WIDTH);
					curr += WIDTH;
				}

				m_layers.Add(image);
			}
		}
	}
}
