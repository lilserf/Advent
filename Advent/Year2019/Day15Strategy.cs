using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day15Strategy : DayIntCodeStrategy
	{
		char[][] m_map;
		int[][] m_bfs;

		static int MAP_WIDTH = 50;
		static int MAP_HEIGHT = 42;
		static int SLEEP = 1;

		static char EMPTY = ' ';
		static char DROID = '\u263A';
		static char WALL = '\u2588';
		static char UNKNOWN = '\u2591';
		static char OXYGEN = '\u256c';

		static Vec2 s_startPos = new Vec2(MAP_WIDTH / 2, MAP_HEIGHT / 2);
		Vec2 m_droidPos = s_startPos;
		Vec2 m_pendingMove = Vec2.Zero;
		Vec2 m_oxygen = Vec2.Zero;
		bool m_found = false;
		int m_timeSinceUnexplored;
		int m_bfsResult = 0;
		int m_filled = 0;

		Random m_rand = new Random();

		public Day15Strategy(string file) : base(file)
		{
			Console.OutputEncoding = Encoding.UTF8;

			m_map = new char[MAP_WIDTH][];
			m_bfs = new int[MAP_WIDTH][];
			for(int i=0; i < MAP_WIDTH; i++)
			{
				m_map[i] = new char[MAP_HEIGHT];
				m_bfs[i] = new int[MAP_HEIGHT];

				for(int j=0; j < MAP_HEIGHT; j++)
				{
					m_bfs[i][j] = int.MaxValue;
				}
			}
		}

		private int BfsSearch()
		{
			int highest = 0;

			for (int i = 0; i < MAP_WIDTH; i++)
			{
				for (int j = 0; j < MAP_HEIGHT; j++)
				{
					m_bfs[i][j] = int.MaxValue;
				}
			}

			m_bfs[m_oxygen.X][m_oxygen.Y] = 0;
			Queue<Vec2> process = new Queue<Vec2>();

			foreach (var offset in Vec2.CardinalAdjacent)
			{
				Vec2 poss = m_oxygen + offset;
				if (m_map[poss.X][poss.Y] == EMPTY)
				{
					process.Enqueue(poss);
				}
			}

			while (process.Count > 0)
			{
				bool found = false;
				Vec2 point = process.Dequeue();

				int lowest = int.MaxValue;
				foreach (var offset in Vec2.CardinalAdjacent)
				{
					Vec2 next = point + offset;
					int value = m_bfs[next.X][next.Y];
					lowest = Math.Min(value, lowest);

					// Found the start pos
					if(next == s_startPos)
					{
						found = true;
					}
					if (m_map[next.X][next.Y] == EMPTY && m_bfs[next.X][next.Y] == int.MaxValue)
					{
						process.Enqueue(next);
					}
				}

				m_bfs[point.X][point.Y] = lowest+1;

				highest = Math.Max(highest, lowest + 1);

				Console.SetCursorPosition(point.X*2, point.Y);
				Console.Write((lowest + 1) % 100);
				Thread.Sleep(SLEEP);

				if(found)
				{
					m_bfsResult = lowest + 2;
				}

			}

			return highest;
		}

		public override string Part1()
		{
			Console.ReadKey();
			DrawMap();

			var program = CreateProgram(() => true, GetMove, Response);

			while (m_timeSinceUnexplored < 100)
			{
				program.Step();
			}

			m_filled = BfsSearch();
			
			Console.SetCursorPosition(0, MAP_HEIGHT + 3);
			return m_bfsResult.ToString();
		}

		private void SetMapPos(Vec2 pos, char c, char display = (char)0)
		{
			if (display == 0) display = c;
			m_map[pos.X][pos.Y] = c;
			Console.SetCursorPosition(pos.X*2, pos.Y);
			Console.Write(display);
			Console.Write(display);
		}

		private void Response(long code)
		{
			Vec2 prevDroidPos = m_droidPos;
			Console.SetCursorPosition(40, MAP_HEIGHT + 2);
			Console.WriteLine($"Response is {code}.");
			switch(code)
			{
				case 0: // HIT A WALL
					Vec2 wallPos = m_droidPos + m_pendingMove;
					SetMapPos(wallPos, WALL);
					break;
				case 1: // MOVED
					SetMapPos(m_droidPos, EMPTY);
					m_droidPos += m_pendingMove;
					SetMapPos(m_droidPos, EMPTY, DROID);
					break;
				case 2: // FOUND OXYGEN SYSTEM
					SetMapPos(m_droidPos, EMPTY);
					m_droidPos += m_pendingMove;
					SetMapPos(m_droidPos, OXYGEN, DROID);
					m_found = true;
					m_oxygen = m_droidPos;
					break;
			}

			//Console.SetCursorPosition(m_droidPos.X, m_droidPos.Y);
			//Console.Write(DROID);

			if (m_found && prevDroidPos == m_oxygen)
			{
				SetMapPos(m_oxygen, OXYGEN);
			}
			if (prevDroidPos == s_startPos)
			{
				Console.SetCursorPosition(s_startPos.X, s_startPos.Y);
				Console.Write('\u263B');
			}
		}

		private int ResponseFromVec2(Vec2 offset)
		{
			if (offset == Vec2.Up) return 1;
			if (offset == Vec2.Down) return 2;
			if (offset == Vec2.Left) return 3;
			if (offset == Vec2.Right) return 4;
			throw new InvalidOperationException();
		}

		private Vec2 Vec2FromResponse(long response)
		{
			switch(response)
			{
				case 1: return Vec2.Up;
				case 2: return Vec2.Down;
				case 3: return Vec2.Left;
				case 4: return Vec2.Right;
				default: throw new InvalidOperationException();
			}
		}

		private void DrawMap()
		{
			for(int i=0; i < MAP_HEIGHT; i++)
			{
				for (int j = 0; j < MAP_WIDTH; j++)
				{
					char c = m_map[j][i] == 0 ? UNKNOWN : m_map[j][i];

					if(m_droidPos.X == j && m_droidPos.Y == i)
					{
						c = DROID;
					}
					Console.SetCursorPosition(j*2, i);
					Console.Write(c);
					Console.Write(c);
				}
			}
		}

		private long GetMove()
		{
			m_timeSinceUnexplored++;

			long move = 0;
			Thread.Sleep(SLEEP);
			foreach(var offset in Vec2.CardinalAdjacent)
			{
				Vec2 pos = m_droidPos + offset;
				if(m_map[pos.X][pos.Y] == 0)
				{
					m_timeSinceUnexplored = 0;

					// Update DFS
					m_bfs[m_droidPos.X][m_droidPos.Y] = 0;

					// Just move to the first unexplored
					m_pendingMove = offset;
					move = ResponseFromVec2(offset);
				}
			}

			if (move == 0)
			{
				int best = int.MaxValue;
				Vec2 towardsUnexplored = Vec2.Zero;

				foreach (var offset in Vec2.CardinalAdjacent)
				{
					Vec2 pos = m_droidPos + offset;
					if(m_bfs[pos.X][pos.Y] < best)
					{
						towardsUnexplored = offset;
						best = m_bfs[pos.X][pos.Y];
					}
				}

				if (best == int.MaxValue)
					throw new InvalidOperationException();

				if (best != int.MaxValue)
				{
					m_bfs[m_droidPos.X][m_droidPos.Y] = best + 1;
				}
				
				m_pendingMove = towardsUnexplored;
				move = ResponseFromVec2(m_pendingMove);
			}

			Console.SetCursorPosition(0, MAP_HEIGHT + 2);
			Console.WriteLine($"Move request is {move}...");
			return move;
		}

		public override string Part2()
		{
			return m_filled.ToString();
		}
	}
}
