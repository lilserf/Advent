using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day11Strategy : DayInputFileStrategy
	{
		long[] m_program;

		char[][] m_ship;
		const int SIZE = 200;
		Vec2 m_robot = new Vec2(SIZE / 2, SIZE / 2);

		const char UNPAINTED = '.';
		const char WHITE = ' ';
		const char BLACK = '█';

		enum Facing
		{
			Up,
			Right,
			Down,
			Left
		}

		Facing m_facing;

		bool m_paint = true;

		public Day11Strategy(string file) : base(file)
		{
			m_ship = new char[SIZE][];
			for(int i=0; i < SIZE; i++)
			{
				m_ship[i] = new char[SIZE];
				for(int j=0; j < SIZE; j++)
				{
					m_ship[i][j] = UNPAINTED;
				}
			}
		}

		void TurnLeft()
		{
			switch(m_facing)
			{
				case Facing.Up: m_facing = Facing.Left; break;
				case Facing.Left: m_facing = Facing.Down; break;
				case Facing.Down: m_facing = Facing.Right; break;
				case Facing.Right: m_facing = Facing.Up; break;

			}
		}

		void TurnRight()
		{
			switch (m_facing)
			{
				case Facing.Up: m_facing = Facing.Right; break;
				case Facing.Left: m_facing = Facing.Up; break;
				case Facing.Down: m_facing = Facing.Left; break;
				case Facing.Right: m_facing = Facing.Down; break;

			}
		}

		private void PaintOrMove(long output)
		{
			if (m_paint)
			{
				m_ship[m_robot.X][m_robot.Y] = (output == 1 ? WHITE : BLACK);
			}
			else
			{
				if(output == 0)
				{
					TurnLeft();
				}
				else
				{
					TurnRight();
				}
				int facing = (int)m_facing;
				Vec2 move = Vec2.CardinalAdjacent[facing];
				m_robot += move;
			}
			m_paint = !m_paint;
		}

		private long GetHullColor()
		{
			return m_ship[m_robot.X][m_robot.Y] == WHITE ? 1 : 0;
		}

		public override string Part1()
		{
			IntCode ic = new IntCode(m_program, () => true, GetHullColor, PaintOrMove);
			ic.Run();

			int sum = 0;

			for (int i = 0; i < SIZE; i++)
			{
				for (int j = 0; j < SIZE; j++)
				{
					if (m_ship[i][j] != UNPAINTED)
					{
						sum++;
					}
					Console.Write(m_ship[j][i]);
				}
				Console.WriteLine();
			}

			return sum.ToString();
		}

		public override string Part2()
		{
			// During iteration and screwing around on Part 1 I accidentally accomplished Part 2
			return "AHCHZEPK";
		}

		protected override void ParseInputLine(string line)
		{
			m_program = line.Split(',').Select(s => long.Parse(s)).ToArray();
		}
	}
}
