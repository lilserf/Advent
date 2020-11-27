using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day17Strategy : DayIntCodeStrategy
	{
		char[][] m_grid;

		static int GRID_WIDTH = 80;
		static int GRID_HEIGHT = 40;

		Vec2 m_cursorPos = new Vec2();

		public Day17Strategy(string file) : base(file)
		{
			m_grid = new char[GRID_WIDTH][];
			for (int i = 0; i < GRID_WIDTH; i++)
			{
				m_grid[i] = new char[GRID_HEIGHT];
			}
		}

		public override string Part1()
		{
			var program = CreateProgram(InputReady, GetInput, Output);
			program.Run();

			List<Vec2> intersections = new List<Vec2>();

			for (int j = 0; j < GRID_HEIGHT; j++)
			{
				for (int i = 0; i < GRID_WIDTH; i++)
				{
					Vec2 curr = new Vec2(i, j);
					if (m_grid[curr.X][curr.Y] == '#')
					{
						int count = 0;
						foreach (var offset in Vec2.CardinalAdjacent)
						{
							var check = curr + offset;

							if (check.X < 0 || check.X > GRID_WIDTH || check.Y < 0 || check.Y > GRID_HEIGHT)
								continue;
							if (m_grid[check.X][check.Y] == '#')
								count++;
						}

						if(count == 4)
						{
							intersections.Add(curr);
						}
					}
				}
			}

			return intersections.Select(v => v.X * v.Y).Sum().ToString();

		}

		private void Output(long obj)
		{
			char c = (char)obj;
			m_grid[m_cursorPos.X][m_cursorPos.Y] = c;
			if(c == '\n')
			{
				m_cursorPos.X = 0;
				m_cursorPos.Y++;
			}
			else
			{
				m_cursorPos.X++;
			}
			Console.Write(c);
		}

		Queue<char> m_inputQueue = new Queue<char>();

		private long GetInput()
		{
			char x = m_inputQueue.Dequeue();
			Console.Write(x);
			return (long)x;
		}

		private bool InputReady()
		{
			return true;
		}

		long m_answer = 0;

		public override string Part2()
		{
			//string loop1 = "L,8,R,12,R,12,R,10,R,10,R,12,R,10";
			//string loop2 = "L,8,R,12,R,12,R,10,R,10,R,12,R,10";
			//string loop3 = "L,10,R,10,L,6";
			//string loop35 ="L,10,R,10,L,6";
			//string loop4 = "R,10,R,12,R,10";
			//string loop5 = "L,8,R,12,R,12,R,10,R,10,R,12,R,10";
			//string loop6 = "L,10,R,10,L,6";

			string progA = "L,8,R,12,R,12,R,10\n";
			string progB = "L,10,R,10,L,6\n";
			string progC = "R,10,R,12,R,10\n";

			string route = "A,C,A,C,B,B,C,A,C,B\n";

			string fullProg = route + progA + progB + progC + "n\n";

			foreach(char c in fullProg)
			{
				m_inputQueue.Enqueue(c);
			}

			var program = CreateProgram(InputReady, GetInput, Output2);
			program.WriteMemory(0, 2);
			program.Run();

			return m_answer.ToString();
		}

		private void Output2(long obj)
		{
			if (obj < 255)
			{
				char c = (char)obj;
				Console.Write(c);
			}
			else
			{
				m_answer = obj;
			}
		}
	}
}
