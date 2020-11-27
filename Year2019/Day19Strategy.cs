using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day19Strategy : DayIntCodeStrategy
	{
		Queue<Func<long>> m_inputFuncs;

		Vec2 m_query = new Vec2(0, 0);
		int MAX_WIDTH = 120;
		int MAX_HEIGHT = 120;

		int m_numAffected = 0;


		public Day19Strategy(string file) : base(file)
		{
		}

		private long GetY()
		{
			return m_query.Y;
		}

		private long GetX()
		{
			return m_query.X;
		}

		public override string Part1()
		{
			m_inputFuncs = new Queue<Func<long>>();
			m_inputFuncs.Enqueue(GetX);
			m_inputFuncs.Enqueue(GetY);

			while (m_query.Y < MAX_HEIGHT)
			{
				var program = CreateProgram(() => true, GetInput, ReportResult);
				program.Run();
			}

			return m_numAffected.ToString();
		}

		private void DrawResult(Vec2 pos, long obj)
		{
			Console.SetCursorPosition(pos.X, pos.Y);
			if (obj == 0)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write('.');
			}
			else if (obj == 1)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write('#');
			}
		}

		private void ReportResult(long obj)
		{
			DrawResult(m_query, obj);

			if(obj == 1)
			{
				m_numAffected++;
			}

			m_query.X++;
			if (m_query.X >= MAX_WIDTH)
			{
				m_query.X = 0;
				m_query.Y++;
			}
		}

		private long GetInput()
		{
			var func = m_inputFuncs.Dequeue();
			m_inputFuncs.Enqueue(func);
			long val = func();
			return val;
		}

		static int SQUARE_SIZE = 99;
		List<Vec2> m_cornerOffsetsTopRight = new List<Vec2>() { new Vec2(-SQUARE_SIZE, 0), new Vec2(-SQUARE_SIZE, SQUARE_SIZE), new Vec2(0, SQUARE_SIZE) };
		List<Vec2> m_cornerOffsetsBotLeft = new List<Vec2>() { new Vec2(0, -SQUARE_SIZE), new Vec2(SQUARE_SIZE, -SQUARE_SIZE), new Vec2(SQUARE_SIZE, 0) };

		private bool IsInBeam(Vec2 pos)
		{
			long output = 0;
			Queue<Func<long>> inputs = new Queue<Func<long>>( new Func<long> [] { () => pos.X, () => pos.Y } );
			CreateProgram(() => true, () => inputs.Dequeue()(), (x) => output = x).Run();
			return output == 1;
		}

		private bool CheckSquare(Vec2 pos, IEnumerable<Vec2> offsets)
		{
			foreach (var offset in offsets)
			{
				if (!IsInBeam(pos + offset))
				{
					return false;
				}
			}
			return true;
		}

		public override string Part2()
		{
			Console.Clear();

			Vec2 topRight = new Vec2(SQUARE_SIZE-1, SQUARE_SIZE);
			while(true)
			{
				if(IsInBeam(topRight))
				{
					if (CheckSquare(topRight, m_cornerOffsetsTopRight))
					{
						break;
					}
					topRight += Vec2.DownRight;
				}
				else
				{
					topRight += Vec2.Left;
				}
			}

			var topLeftFromRight = topRight + new Vec2(-SQUARE_SIZE, 0);
			Console.WriteLine(topLeftFromRight);

			Vec2 botLeft = new Vec2(0, SQUARE_SIZE);
			while (true)
			{
				if (IsInBeam(botLeft))
				{
					if (CheckSquare(botLeft, m_cornerOffsetsBotLeft))
					{
						break;
					}
					botLeft += Vec2.Down;
				}
				else
				{
					botLeft += Vec2.Right;
				}
			}

			var topLeftFromBot = botLeft + new Vec2(0, -SQUARE_SIZE);
			Console.WriteLine(topLeftFromBot);

			return "";

		}


	}
}
