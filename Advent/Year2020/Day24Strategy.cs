using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{

	class Day24Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		bool[][] m_map;

		public override void ParseInputLine(string line)
		{
			m_data.Add(line);
		}

		const int MAP_WIDTH = 2000;

		public override string Part1()
		{

			ArrayUtil.Build2DArray(ref m_map, MAP_WIDTH, MAP_WIDTH, false);

			Vec2 refTile = new Vec2(MAP_WIDTH / 2, MAP_WIDTH / 2);

			foreach(var line in m_data)
			{
				Vec2 curr = refTile;
				var temp = new string(line.ToCharArray());
				while (temp.Length > 0)
				{
					string dir = "";

					if (temp.StartsWith("ne"))
					{
						dir = "ne";
						temp = temp.Substring(2);
					}
					else if (temp.StartsWith("se"))
					{
						dir = "se";
						temp = temp.Substring(2);
					}
					else if (temp.StartsWith("nw"))
					{
						dir = "nw";
						temp = temp.Substring(2);
					}
					else if (temp.StartsWith("sw"))
					{
						dir = "sw";
						temp = temp.Substring(2);
					}
					else if (temp.StartsWith("w"))
					{
						dir = "w";
						temp = temp.Substring(1);
					}
					else if (temp.StartsWith("e"))
					{
						dir = "e";
						temp = temp.Substring(1);
					}


					switch(dir)
					{
						case "e": curr.X++; break;
						case "w": curr.X--; break;
						case "ne":curr.Y++; break;
						case "sw":curr.Y--; break;
						case "nw":curr.X--; curr.Y++; break;
						case "se":curr.X++; curr.Y--; break;
					}
				}

				m_map[curr.X][curr.Y] = !m_map[curr.X][curr.Y];
			}

			return m_map.Sum(x => x.Count(y => y)).ToString();
		}

		static Vec2[] Neighbors = new Vec2[] { new Vec2(1,0), new Vec2(0,1), new Vec2(-1,1), new Vec2(-1, 0), new Vec2(0, -1), new Vec2(1,-1)};

		public override string Part2()
		{
			int minX = MAP_WIDTH;
			int maxX = 0;
			int minY = MAP_WIDTH;
			int maxY = 0;

			int loopXMin = 0;
			int loopXMax = MAP_WIDTH-1;
			int loopYMin = 0;
			int loopYMax = MAP_WIDTH-1;

			for(int i=0; i < 100; i++)
			{
				List<Vec2> flips = new List<Vec2>();

				for(int x= loopXMin; x <= loopXMax; x++)
				{
					for(int y= loopYMin; y <= loopYMax; y++)
					{
						Vec2 pos = new Vec2(x, y);
						int sum = 0;
						foreach(var offset in Neighbors)
						{
							Vec2 neigh = pos + offset;

							if (neigh.X >= 0 && neigh.X < MAP_WIDTH && neigh.Y >= 0 && neigh.Y < MAP_WIDTH)
							{
								sum += m_map[neigh.X][neigh.Y] ? 1 : 0;
							}
						}

						bool currTile = m_map[pos.X][pos.Y];

						if(currTile)
						{
							minX = Math.Min(minX, pos.X);
							maxX = Math.Max(maxX, pos.X);
							minY = Math.Min(minY, pos.Y);
							maxY = Math.Max(maxY, pos.Y);
						}

						if (currTile && (sum == 0 || sum > 2))
							flips.Add(pos);
						else if (!currTile && sum == 2)
							flips.Add(pos);
					}
				}

				foreach(var f in flips)
				{
					if (f.X == MAP_WIDTH-1 || f.X == 0 || f.Y == MAP_WIDTH-1 || f.Y == 0)
					{
						throw new InvalidOperationException("Map too small!");
					}

					m_map[f.X][f.Y] = !m_map[f.X][f.Y];
				}

				loopXMin = minX-2;
				loopYMin = minY-2;
				loopXMax = maxX+2;
				loopYMax = maxY+2;

				Console.WriteLine($"Day {i}: { m_map.Sum(x => x.Count(y => y))}");
			}

			return m_map.Sum(x => x.Count(y => y)).ToString();
		}
	}
}
