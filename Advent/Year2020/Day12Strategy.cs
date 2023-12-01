using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advent;

namespace Advent.Year2020
{
	class Day12Strategy : DayLineLoaderBasic
	{
		List<string> m_data = new List<string>();

		public override void ParseInputLine(string line)
		{
			m_data.Add(line);
		}

		public override string Part1()
		{
			Vec2 position = new Vec2(0, 0);
			Vec2 facing = Vec2.Right;
			int facingDeg = 0;

			Queue<string> dirs = new Queue<string>(m_data);

			while(dirs.Count > 0)
			{
				string d = dirs.Dequeue();

				int val = int.Parse(d.Substring(1));
				switch(d[0])
				{
					case 'N': position += (Vec2.Up * val); break;
					case 'E': position += (Vec2.Right * val); break;
					case 'S': position += (Vec2.Down * val); break;
					case 'W': position += (Vec2.Left * val); break;
					case 'F': position += (facing * val); break;
					case 'L':
						{
							facingDeg = facingDeg + 360 - val;
							facingDeg %= 360;
						}
						break;
					case 'R':
						{
							facingDeg += val;
							facingDeg %= 360;
						}
						break;

				}

				switch(facingDeg)
				{
					case 0: facing = Vec2.Right; break;
					case 90: facing = Vec2.Down; break;
					case 180: facing = Vec2.Left; break;
					case 270: facing = Vec2.Up; break;
				}

				Console.WriteLine($"{d} -- Now at {position} facing {facing} {facingDeg}");
			}

			return Vec2.ManhattanDistance(Vec2.Zero, position).ToString();
		}

		public override string Part2()
		{
			Vec2 shipPos = new Vec2(0, 0);
			Vec2 waypoint = new Vec2(10, -1);

			Queue<string> dirs = new Queue<string>(m_data);

			while (dirs.Count > 0)
			{
				string d = dirs.Dequeue();

				int val = int.Parse(d.Substring(1));
				switch (d[0])
				{
					case 'N': waypoint += (Vec2.Up * val); break;
					case 'E': waypoint += (Vec2.Right * val); break;
					case 'S': waypoint += (Vec2.Down * val); break;
					case 'W': waypoint += (Vec2.Left * val); break;
					case 'F':
						{
							Vec2 move = waypoint - shipPos;
							for(int i=0; i<val;i++)
							{
								shipPos += move;
								waypoint += move;
							}
						}
						break;
					case 'L':
						{
							Vec2 vec = waypoint - shipPos;
							int numRots = val / 90;
							for(int i=0; i < numRots; i++)
							{
								vec.Rotate90CounterClockwise();
							}
							waypoint = shipPos + vec;
						}
						break;
					case 'R':
						{
							Vec2 vec = waypoint - shipPos;
							int numRots = val / 90;
							int ccw = 4 - numRots;
							for (int i = 0; i < ccw; i++)
							{
								vec.Rotate90CounterClockwise();
							}
							waypoint = shipPos + vec;
						}
						break;

				}

				Console.WriteLine($"{d} -- Ship at {shipPos}, Waypoint at {waypoint}");
			}

			return Vec2.ManhattanDistance(Vec2.Zero, shipPos).ToString();
		}
	}
}
