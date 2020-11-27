using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day13Strategy : DayIntCodeStrategy
	{
		Queue<long> m_outputBuffer = new Queue<long>();

		static bool s_fast = false;

		long m_ballX = 0;
		long m_paddleX = 0;
		long m_score = 0;

		long m_bricks = 0;

		const char EMPTY = ' ';
		const char WALL = '█';
		const char BRICK = '▓';
		const char PADDLE = '═';
		const char BALL = '☻';

		public Day13Strategy(string file) : base(file)
		{
		}

		char GetTileChar(long id)
		{
			switch(id)
			{
				case 0: return EMPTY; 
				case 1: return WALL; 
				case 2: return BRICK;
				case 3: return PADDLE;
				case 4: return BALL;
			}
			return ' ';
		}

		void DrawTile(long x, long y, long id)
		{
			if (!s_fast)
			{
				Console.SetCursorPosition((int)x, (int)y);
				Console.Write(GetTileChar(id));
			}

			if(id == 2)
			{
				m_bricks++;
			}
			if(id == 4)
			{
				m_ballX = x;
			}
			if(id == 3)
			{
				m_paddleX = x;
			}
		}

		void DrawScore(long score)
		{
			if (!s_fast)
			{
				Console.SetCursorPosition(42, 1);
				Console.Write(score);
			}
			m_score = score;
		}

		public void QueueOutput(long num)
		{
			m_outputBuffer.Enqueue(num);

			if(m_outputBuffer.Count == 3)
			{
				var param1 = m_outputBuffer.Dequeue();
				var param2 = m_outputBuffer.Dequeue();
				if (param1 == -1 && param2 == 0)
				{
					DrawScore(m_outputBuffer.Dequeue());
				}
				else
				{
					DrawTile(param1, param2, m_outputBuffer.Dequeue());
				}
			}
		}

		public override string Part1()
		{
			IntCode game = CreateProgram(() => true, () => 0, QueueOutput);
			game.Run();

			Console.SetCursorPosition(0, 28);
			return m_bricks.ToString();
		}

		private long GetJoystick()
		{
			if (!s_fast)
			{
				Thread.Sleep(20);
			}
			// Chase that ball
			if(m_paddleX < m_ballX)
			{
				return 1;
			}
			else if(m_paddleX > m_ballX)
			{
				return -1;
			}
			return 0;
		}

		public override string Part2()
		{
			IntCode game = CreateProgram(() => true, GetJoystick, QueueOutput);
			// INSERT COINS
			game.WriteMemory(0, 2);
			game.Run();

			Console.SetCursorPosition(0, 30);

			return m_score.ToString();
		}

	}
}
