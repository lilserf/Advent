using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Advent.Year2019
{

	class Day20Strategy : DayMapInputFileStrategy
	{
		static int MAP_WIDTH = 115;
		static int MAP_HEIGHT = 117;
		static int LAYERS = 100;
		char[][] m_map;
		int[][][] m_bfs;
		Dictionary<Vec2, string> m_portals;
		Dictionary<Vec2, bool> m_insidePortal;
		Vec2 m_start;
		Vec2 m_end;


		public Day20Strategy(string inputFile) : base(inputFile)
		{
			ArrayUtil.Build2DArray(ref m_map, MAP_WIDTH, MAP_HEIGHT, '?');
			m_bfs = new int[LAYERS][][];
			for (int i = 0; i < LAYERS; i++)
			{
				ArrayUtil.Build2DArray(ref m_bfs[i], MAP_WIDTH, MAP_HEIGHT, int.MaxValue);
			}
			m_portals = new Dictionary<Vec2, string>();
			m_insidePortal = new Dictionary<Vec2, bool>();
		}

		private void PrintMap(bool bfs, int startRow = 0)
		{
			for(int j=startRow; j < MAP_HEIGHT; j++)
			{
				for (int i=0; i < MAP_WIDTH; i++)
				{
					PrintMapChar(bfs, i, j, startRow);
				}
				Console.WriteLine();
			}
		}


		private void PrintMapChar(bool bfs, int i, int j, int layer = 0, int startRow = 0)
		{
			if (j < startRow)
				return;
			Console.SetCursorPosition(i, j - startRow);
			char c = m_map[i][j];
			if (c != '.')
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write(c);
			}
			else
			{
				if (bfs)
				{
					if (m_bfs[layer][i][j] == int.MaxValue)
					{
						Console.Write(' ');
					}
					else
					{
						int digit = m_bfs[layer][i][j] % 10;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.Write(digit);
					}
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write(c);
				}
			}
		}

		public override string Part1()
		{
			BuildBfs();

			return m_bfs[0][m_start.X][m_start.Y].ToString();
		}

		public override string Part2()
		{
			for(int l = 0; l < LAYERS; l++)
			{
				for(int i = 0; i < MAP_WIDTH; i++)
				{
					for (int j = 0; j <MAP_HEIGHT; j++)
					{
						m_bfs[l][i][j] = int.MaxValue;
					}
				}
			}

			BuildBfs(true);
			return m_bfs[0][m_start.X][m_start.Y].ToString();
		}

		static int START_ROW = 60;
		static bool s_debug = false;

		private void BuildBfs(bool allowRecurse = false)
		{
			if(s_debug) PrintMap(true, START_ROW);

			int layer = 0;

			m_bfs[layer][m_end.X][m_end.Y] = 0;

			Queue<Tuple<int, Vec2>> process = new Queue<Tuple<int, Vec2>>();
			process.Enqueue(new Tuple<int, Vec2>(layer, m_end));

			while (process.Count > 0)
			{
				var next = process.Dequeue();
				layer = next.Item1;
				Vec2 curr = next.Item2;
				int currDist = m_bfs[layer][curr.X][curr.Y];

				bool didSomething = false;

				foreach (var offset in Vec2.CardinalAdjacent)
				{
					Vec2 v = curr + offset;
					if (v.X >= 0 && v.X < MAP_WIDTH && v.Y > 0 && v.Y < MAP_HEIGHT)
					{
						if (m_map[v.X][v.Y] == '.' || m_map[v.X][v.Y] == '!')
						{
							if (m_bfs[layer][v.X][v.Y] > currDist + 1)
							{
								didSomething = true;
								m_bfs[layer][v.X][v.Y] = currDist + 1;
								if (s_debug) PrintMapChar(true, v.X, v.Y, START_ROW);
								process.Enqueue(new Tuple<int, Vec2>(layer, v));
							}
						}
					}
				}

				if(m_map[curr.X][curr.Y] == '!')
				{
					Vec2 end = NavigatePortal(curr);
					int newLayer = layer;

					if (allowRecurse)
					{
						newLayer = m_insidePortal[curr] ? layer + 1 : layer - 1;

						if(newLayer < 0 || newLayer >= LAYERS)
						{
							continue;
						}
					}

					if (m_bfs[newLayer][end.X][end.Y] > currDist + 1)
					{
						didSomething = true;
						m_bfs[newLayer][end.X][end.Y] = currDist + 1;
						if (s_debug) PrintMapChar(true, end.X, end.Y, START_ROW);
						process.Enqueue(new Tuple<int, Vec2>(newLayer, end));
					}
				}

				if(s_debug && didSomething)
				{
					Thread.Sleep(10);
					//Console.ReadKey();
				}
			}

		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_map[position.X][position.Y] = c;
		}

		private bool IsLetter(char c)
		{
			return (c >= 'A' && c <= 'Z');
		}

		private Vec2 NavigatePortal(Vec2 start)
		{
			var label = m_portals[start];

			return m_portals.Where(kvp => kvp.Value == label && kvp.Key != start).Select(kvp => kvp.Key).First();
		}

		protected override void ReadMapEnd()
		{
			for(int i=1; i < MAP_WIDTH-1; i++)
			{
				for(int j=1; j < MAP_HEIGHT-1; j++)
				{
					char c = m_map[i][j];

					if(IsLetter(c))
					{
						Vec2 pos = new Vec2(i, j);

						Vec2 left = pos + Vec2.Left;
						Vec2 right = pos + Vec2.Right;
						Vec2 up = pos + Vec2.Up;
						Vec2 down = pos + Vec2.Down;

						char cLeft = m_map[left.X][left.Y];
						char cRight = m_map[right.X][right.Y];
						char cUp = m_map[up.X][up.Y];
						char cDown = m_map[down.X][down.Y];

						bool isInside = false;
						string label = "";
						Vec2 portal = Vec2.Zero;
						if(IsLetter(cUp) && cDown == '.')
						{
							// Vertical label with portal down
							label += "" + cUp + c;
							portal = down;
							isInside = !(portal.Y == 2);
						}
						else if(IsLetter(cDown) && cUp == '.')
						{
							// Vertical label with portal up
							label += "" + c + cDown;
							portal = up;
							isInside = !(portal.Y == MAP_HEIGHT - 3);
						}
						else if(IsLetter(cLeft) && cRight == '.')
						{
							// Horizontal label with portal right
							label += "" + cLeft + c;
							portal = right;
							isInside = !(portal.X == 2);
						}
						else if(IsLetter(cRight) && cLeft == '.')
						{
							// Horizontal label with portal left
							label += "" + c + cRight;
							portal = left;
							isInside = !(portal.X == MAP_WIDTH - 3);
						}

						if(portal != Vec2.Zero)
						{
							string inOut = isInside ? "inside" : "outside";
							Console.WriteLine($"Found portal [{label}] at {portal}! It leads {inOut}");
							if (label == "AA")
							{
								m_start = portal;
							}
							else if (label == "ZZ")
							{
								m_end = portal;
							}
							else
							{
								m_map[portal.X][portal.Y] = '!';
								m_portals[portal] = label;
								m_insidePortal[portal] = isInside;
							}
						}
					}
				}
			}
		}
	}
}
