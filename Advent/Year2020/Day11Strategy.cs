using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Day11Strategy : DayMapInputFileStrategy
	{
		char[][] m_map;
		const int MAP_WIDTH = 99;
		const int MAP_HEIGHT = 99;

		public Day11Strategy(string file) : base(file)
		{
			ArrayUtil.Build2DArray(ref m_map, 99, 99, ' ');
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_map[position.X][position.Y] = c;
		}


		private int NewGeneration()
		{
			char[][] newMap = null;
			ArrayUtil.Build2DArray(ref newMap, MAP_WIDTH, MAP_HEIGHT, ' ');

			int stateChanges = 0;

			for(int y = 0; y < MAP_HEIGHT; y++)
			{
				for (int x = 0; x < MAP_WIDTH; x++)
				{
					var curr = new Vec2(x, y);
					var c = m_map[curr.X][curr.Y];
					int numAdj = 0;
					foreach(var dir in Vec2.Adjacent)
					{
						var adj = curr + dir;

						if (adj.X >= 0 && adj.X < MAP_WIDTH && adj.Y >= 0 && adj.Y < MAP_HEIGHT)
						{
							if (m_map[adj.X][adj.Y] == '#')
							{
								numAdj++;
							}
						}
					}

					char newChar = c;
					if(c == 'L' && numAdj == 0)
					{
						newChar = '#';
						stateChanges++;
					}
					else if(c == '#' && numAdj >= 4)
					{
						newChar = 'L';
						stateChanges++;
					}

					newMap[curr.X][curr.Y] = newChar; ;
				}
			}

			m_map = newMap;
			return stateChanges;
		}

		public override string Part1()
		{
			//ArrayUtil.PrintArray(m_map, 10, 10);
			//Console.ReadKey();

			int gen = 0;
			while(NewGeneration() > 0)
			{
				gen++;
				//Console.WriteLine($"#### GENERATION {gen}");
				//ArrayUtil.PrintArray(m_map, 10, 10);
				//Console.ReadKey();
			}

			var occupied = m_map.Sum(y => y.Count(x => x == '#'));
			return occupied.ToString();
		}



		private int NewGeneration2()
		{
			char[][] newMap = null;
			ArrayUtil.Build2DArray(ref newMap, MAP_WIDTH, MAP_HEIGHT, ' ');

			int stateChanges = 0;

			for (int y = 0; y < MAP_HEIGHT; y++)
			{
				for (int x = 0; x < MAP_WIDTH; x++)
				{
					var curr = new Vec2(x, y);
					var c = m_map[curr.X][curr.Y];
					int numAdj = 0;
					foreach (var dir in Vec2.Adjacent)
					{
						var adj = curr + dir;

						while(adj.X >= 0 && adj.X < MAP_WIDTH && adj.Y >= 0 && adj.Y < MAP_HEIGHT && m_map[adj.X][adj.Y] == '.')
						{
							adj = adj + dir;
						}

						if (adj.X >= 0 && adj.X < MAP_WIDTH && adj.Y >= 0 && adj.Y < MAP_HEIGHT)
						{
							if (m_map[adj.X][adj.Y] == '#')
							{
								numAdj++;
							}
						}
					}

					char newChar = c;
					if (c == 'L' && numAdj == 0)
					{
						newChar = '#';
						stateChanges++;
					}
					else if (c == '#' && numAdj >= 5)
					{
						newChar = 'L';
						stateChanges++;
					}

					newMap[curr.X][curr.Y] = newChar; ;
				}
			}

			m_map = newMap;
			return stateChanges;
		}

		public override string Part2()
		{
			Initialize();

			int gen = 0;
			while (NewGeneration2() > 0)
			{
				gen++;
				//Console.WriteLine($"#### GENERATION {gen}");
				//ArrayUtil.PrintArray(m_map, 10, 10);
				//Console.ReadKey();
			}

			var occupied = m_map.Sum(y => y.Count(x => x == '#'));
			return occupied.ToString();
		}
	}
}
