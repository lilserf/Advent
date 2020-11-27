using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Grid
	{
		int[][] m_map;
		static int MAP_WIDTH = 5;
		static int MAP_HEIGHT = 5;

		Grid m_parent = null;
		Grid m_child = null;
		bool m_recursive = false;
		static Vec2 CENTER = new Vec2(2, 2);

		int m_currentBugs = 0;
		int m_id = 0;

		public Grid(bool recursive = false, int id = 0)
		{
			m_id = id;
			m_recursive = recursive;
			ArrayUtil.Build2DArray(ref m_map, MAP_WIDTH, MAP_HEIGHT, 0);
		}
		
		public void Set(int x, int y, int value)
		{
			m_map[x][y] = value;
		}

		private int GetCurrent(Vec2 pos)
		{
			return m_map[pos.X][pos.Y] & 0x1;
		}

		const int BIT_CURR = 0;
		const int BIT_NEXT = 1;
		const int BIT_NEXT_NEW = 2;
		const int BIT_NEXT_DIED = 3;
		const int BIT_CURR_NEW = BIT_NEXT_NEW - 1;
		const int BIT_CURR_DIED = BIT_NEXT_DIED - 1;


		private void SetBit(Vec2 pos, int bit)
		{
			m_map[pos.X][pos.Y] |= (1 << bit);
		}

		private void ClearBit(Vec2 pos, int bit)
		{
			m_map[pos.X][pos.Y] &= ~(1 << bit);
		}

		private void SetNext(Vec2 pos)
		{
			int curr = GetCurrent(pos);

			SetBit(pos, BIT_NEXT);

			if (curr == 0)
			{
				SetBit(pos, BIT_NEXT_NEW);
			}
			else
			{
				ClearBit(pos, BIT_NEXT_NEW);
			}
		}

		private void ClearNext(Vec2 pos)
		{
			int curr = GetCurrent(pos);

			ClearBit(pos, BIT_NEXT);

			if (curr == 1)
			{
				SetBit(pos, BIT_NEXT_DIED);
			}
			else
			{
				ClearBit(pos, BIT_NEXT_DIED);
			}

		}

		private IEnumerable<int> GetNeighbors(Vec2 position)
		{
			foreach (var offset in Vec2.CardinalAdjacent)
			{
				var neighbor = position + offset;

				// The center is our neighbor, and we're in recursive mode
				if(m_recursive && neighbor.Equals(CENTER))
				{
					if(m_child != null)
					{
						if(position.X == 1)
						{
							yield return m_child.GetCurrent(new Vec2(0, 0));
							yield return m_child.GetCurrent(new Vec2(0, 1));
							yield return m_child.GetCurrent(new Vec2(0, 2));
							yield return m_child.GetCurrent(new Vec2(0, 3));
							yield return m_child.GetCurrent(new Vec2(0, 4));
						}
						else if(position.X == 3)
						{
							yield return m_child.GetCurrent(new Vec2(4, 0));
							yield return m_child.GetCurrent(new Vec2(4, 1));
							yield return m_child.GetCurrent(new Vec2(4, 2));
							yield return m_child.GetCurrent(new Vec2(4, 3));
							yield return m_child.GetCurrent(new Vec2(4, 4));
						}

						if(position.Y == 1)
						{
							yield return m_child.GetCurrent(new Vec2(0, 0));
							yield return m_child.GetCurrent(new Vec2(1, 0));
							yield return m_child.GetCurrent(new Vec2(2, 0));
							yield return m_child.GetCurrent(new Vec2(3, 0));
							yield return m_child.GetCurrent(new Vec2(4, 0));
						}
						else if(position.Y == 3)
						{
							yield return m_child.GetCurrent(new Vec2(0, 4));
							yield return m_child.GetCurrent(new Vec2(1, 4));
							yield return m_child.GetCurrent(new Vec2(2, 4));
							yield return m_child.GetCurrent(new Vec2(3, 4));
							yield return m_child.GetCurrent(new Vec2(4, 4));
						}
					}
				}
				else if (neighbor.X >= 0 && neighbor.X < MAP_WIDTH && neighbor.Y >= 0 && neighbor.Y < MAP_HEIGHT)
				{
					yield return GetCurrent(neighbor);
				}
			}

			// Handle the outer edges
			if(m_recursive && m_parent != null)
			{
				if(position.X == 0)
				{
					// left edge neighbors 1,2 in the parent
					yield return m_parent.GetCurrent(new Vec2(1, 2));
				}
				else if(position.X == 4)
				{
					yield return m_parent.GetCurrent(new Vec2(3, 2));
				}

				if(position.Y == 0)
				{
					// top edge neighbors 2,1 in the parent
					yield return m_parent.GetCurrent(new Vec2(2, 1));
				}
				else if(position.Y == 4)
				{
					yield return m_parent.GetCurrent(new Vec2(2, 3));
				}

			}
		}

		private void CreateParentIfNeeded()
		{
			if (m_parent == null && m_currentBugs > 0)
			{
				m_parent = new Grid(true, m_id - 1);
				m_parent.m_child = this;
			}
		}

		private void CreateChildIfNeeded()
		{
			if (m_child == null && m_currentBugs > 0)
			{
				m_child = new Grid(true, m_id + 1);
				m_child.m_parent = this;
			}
		}

		public void Evolve()
		{
			EvolveLocal();
			if (m_recursive)
			{
				CreateChildIfNeeded();
				m_child?.EvolveDown();
				CreateParentIfNeeded();
				m_parent?.EvolveUp();
			}
			FinishEvolve();
		}

		private void EvolveUp()
		{
			//Console.WriteLine($"[{m_id}] EvolveUp");
			EvolveLocal();
			CreateParentIfNeeded();
			m_parent?.EvolveUp();
			FinishEvolve();
		}

		private void EvolveDown()
		{
			//Console.WriteLine($"[{m_id}] EvolveDown");
			EvolveLocal();
			CreateChildIfNeeded();
			m_child?.EvolveDown();
			FinishEvolve();
		}

		private void EvolveLocal()
		{
			//Console.WriteLine($"[{m_id}] EvolveLocal");
			m_currentBugs = 0;
			for (int i = 0; i < MAP_WIDTH; i++)
			{
				for (int j = 0; j < MAP_HEIGHT; j++)
				{
					Vec2 pos = new Vec2(i, j);
					// Clear the "next" bit for starters
					ClearBit(pos, BIT_NEXT);

					// Don't process the center square
					if (m_recursive && pos.Equals(CENTER))
						continue;

					int current = GetCurrent(pos);
					m_currentBugs += current;

					var neighbors = GetNeighbors(pos);
					int numAdjacent = neighbors.Count(x => x == 1);

					if (current == 1)
					{
						if (numAdjacent == 1)
						{
							SetNext(pos);
						}
						else
						{
							ClearNext(pos);
						}
					}
					else if (current == 0)
					{
						if (numAdjacent == 1 || numAdjacent == 2)
						{
							SetNext(pos);
						}
						else
						{
							ClearNext(pos);
						}
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
			}

		}

		private void FinishEvolve()
		{
			//Console.WriteLine($"[{m_id}] FinishEvolve");
			m_currentBugs = 0;
			//Bitshift over the next generation
			for (int i = 0; i < MAP_WIDTH; i++)
			{
				for (int j = 0; j < MAP_HEIGHT; j++)
				{
					// Shift the 2 bit over to 1
					m_map[i][j] = m_map[i][j] >> 1;

					if(GetCurrent(new Vec2(i,j)) == 1)
					{
						m_currentBugs++;
					}
				}
			}
		}

		public int Count()
		{
			return m_currentBugs + (m_child?.CountDown() ?? 0) + (m_parent?.CountUp() ?? 0);
		}

		private int CountDown()
		{
			return m_currentBugs + (m_child?.CountDown() ?? 0);
		}

		private int CountUp()
		{
			return m_currentBugs + (m_parent?.CountUp() ?? 0);
		}

		private int GetHighestParent()
		{
			if(m_parent == null)
			{
				return m_id;
			}
			else
			{
				return m_parent.GetHighestParent();
			}
		}

		private void PrintDown(int columns)
		{
			PrintLocal(columns);

			m_child?.PrintDown(columns);
		}

		private void PrintUp(int columns)
		{
			m_parent?.PrintUp(columns);

			PrintLocal(columns);
		}

		public void Print(int columns = 1)
		{
			if(columns > 1)
				Console.Clear();

			if(m_recursive)
			{
				m_parent?.PrintUp(columns);
			}

			PrintLocal(columns);

			if(m_recursive)
			{
				m_child?.PrintDown(columns);
			}
		}

		static int GRID_PRINT_WIDTH = 7;
		static int GRID_PRINT_HEIGHT = 7;

		private void PrintLocal(int columns)
		{
			// If we're empty and on the end, don't print
			if (m_currentBugs == 0 && (m_child == null || m_parent == null))
				return;

			int firstId = GetHighestParent();
			int nth = m_id - firstId;

			int row = nth / columns;
			int col = nth % columns;

			Vec2 startPos = new Vec2(col * GRID_PRINT_WIDTH, row * GRID_PRINT_HEIGHT);

			if(columns > 1)
				Console.SetCursorPosition(startPos.X, startPos.Y);

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"Depth {m_id}:");

			for (int j = 0; j < MAP_HEIGHT; j++)
			{
				if (columns > 1)
					Console.SetCursorPosition(startPos.X, startPos.Y + j + 1);
				Console.Write('\t');
				for (int i = 0; i < MAP_WIDTH; i++)
				{
					Console.ForegroundColor = ConsoleColor.Gray;
					Vec2 pos = new Vec2(i, j);
					int curr = GetCurrent(pos);
					char c = (curr == 1) ? '#' : '.';

					bool newBug = (m_map[i][j] & (1 << BIT_CURR_NEW)) > 0;
					bool deadBug = (m_map[i][j] & (1 << BIT_CURR_DIED)) > 0;

					if (curr == 1 && deadBug == true)
					{
						throw new InvalidOperationException();
					}

					if (m_recursive && new Vec2(i, j).Equals(CENTER))
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						c = '?';
					}

					if (newBug) Console.ForegroundColor = ConsoleColor.Green;
					if (deadBug) Console.ForegroundColor = ConsoleColor.Red;
					Console.Write($"{c}");
				}
				Console.WriteLine();
			}

			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public int CalculateRating()
		{
			int result = 0;
			int digit = 0;
			for (int j = 0; j < MAP_HEIGHT; j++)
			{
				for (int i = 0; i < MAP_WIDTH; i++)
				{
					if ((m_map[i][j] & 0x1) == 0x1)
					{
						result += (1 << digit);
					}

					digit++;
				}
			}

			return result;
		}
	}

	class Day24Strategy : DayMapInputFileStrategy
	{
		Grid m_grid;
		Grid m_part2Grid;

		HashSet<int> m_ratings;

		public Day24Strategy(string inputFile) : base(inputFile)
		{
			m_grid = new Grid();
			m_part2Grid = new Grid(true);
			m_ratings = new HashSet<int>();
		}


		public override string Part1()
		{
			m_ratings.Add(m_grid.CalculateRating());

			while(true)
			{
				m_grid.Evolve();
				var rating = m_grid.CalculateRating();
				if (m_ratings.Contains(rating))
				{
					return rating.ToString();
				}
				else
				{
					m_ratings.Add(rating);
				}
			}
		}


		static bool toFile = false;

		public override string Part2()
		{
			//m_part2Grid.Print();
			TextWriter oldOut = Console.Out;
			if (toFile)
			{
				FileStream ostream;
				StreamWriter writer;
				ostream = new FileStream("./output.txt", FileMode.OpenOrCreate, FileAccess.Write);
				writer = new StreamWriter(ostream);
				Console.SetOut(writer);
			}

			for(int min = 0; min < 200; min++)
			{
				//Console.WriteLine($"Generation #{min + 1}:\n\n");
				m_part2Grid.Evolve();

				//m_part2Grid.Print();
				//Console.WriteLine("\n\n");
			}

			//m_part2Grid.Print();

			if (toFile)
			{
				Console.SetOut(oldOut);
			}

			return m_part2Grid.Count().ToString();
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_grid.Set(position.X, position.Y, (c == '#') ? 1 : 0);
			m_part2Grid.Set(position.X, position.Y, (c == '#') ? 1 : 0);
		}
	}
}
