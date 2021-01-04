using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2017
{
	class Rule
	{
		static bool Debug = false;
		public int Id { get; private set; }
		public int Size { get; private set; }

		public char[][] Input => m_input;
		public char[][] m_input;
		public char[][] Output => m_output;
		public char[][] m_output;

		private HashSet<char[][]> m_matches = new HashSet<char[][]>();
		public Rule(int id, string line)
		{
			Id = id;
			var splits = line.Split('=');
			var input = splits[0].Trim();
			var output = splits[1].Substring(1).Trim();

			var inputSplits = input.Split('/');
			var outputSplits = output.Split('/');

			Size = inputSplits.Length;
			int outSize = outputSplits.Length;

			ArrayUtil.Build2DArray(ref m_input, Size, Size, ' ');
			ArrayUtil.Build2DArray(ref m_output, outSize, outSize, ' ');

			for(int row=0; row < Size; row++)
			{
				for(int col=0; col < Size; col++)
				{
					m_input[row][col] = inputSplits[row][col];
				}
			}

			for (int row = 0; row < outSize; row++)
			{
				for (int col = 0; col < outSize; col++)
				{
					m_output[row][col] = outputSplits[row][col];
				}
			}

			m_matches.Add(m_input);
			m_matches.Add(Rotate(m_input, 90));
			m_matches.Add(Rotate(m_input, 180));
			m_matches.Add(Rotate(m_input, 270));
			var flip = FlipHoriz();
			m_matches.Add(flip);
			m_matches.Add(Rotate(flip, 90));
			m_matches.Add(Rotate(flip, 180));
			m_matches.Add(Rotate(flip, 270));
		}

		public static char[][] Rotate(char[][] inMap, int deg)
		{
			char[][] map = null;
			int Size = inMap.Length;
			ArrayUtil.Build2DArray(ref map, Size, Size, ' ');

			Func<int, int, int> xTrans = null;
			Func<int, int, int> yTrans = null;

			switch(deg)
			{
				case 90:
					xTrans = (x, y) => Size - 1 - y;
					yTrans = (x, y) => x;
					break;
				case 180:
					xTrans = (x, y) => Size - 1 - x;
					yTrans = (x, y) => Size - 1 - y;
					break;
				case 270:
					xTrans = (x, y) => y;
					yTrans = (x, y) => Size - 1 - x;
					break;
			}

			for(int row=0; row < Size; row++)
			{
				for(int col=0; col < Size; col++)
				{
					map[yTrans(col,row)][xTrans(col,row)] = inMap[row][col];
				}
			}

			return map;
		}

		public char[][] FlipHoriz()
		{
			char[][] map = null;

			ArrayUtil.Build2DArray(ref map, Size, Size, ' ');
			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					map[row][Size-1-col] = m_input[row][col];
				}
			}

			return map;
		}

		public char[][] FlipVert()
		{
			char[][] map = null;

			ArrayUtil.Build2DArray(ref map, Size, Size, ' ');
			for (int row = 0; row < Size; row++)
			{
				for (int col = 0; col < Size; col++)
				{
					map[Size-1-row][col] = m_input[row][col];
				}
			}

			return map;
		}

		public void PrintMatches()
		{
			Console.WriteLine("------------");
			foreach(var m in m_matches)
			{
				ArrayUtil.PrintArray(m, Size, Size);
				Console.WriteLine();
			}
			Console.WriteLine("------------");
		}

		public bool IsMatch(char[][] map, int x, int y)
		{
			foreach(var m in m_matches)
			{
				bool matched = true;
				for (int row = 0; row < Size; row++)
				{
					for (int col = 0; col < Size; col++)
					{
						if(map[y+row][x+col] != m[row][col])
						{
							matched = false;
							break;
						}
					}
					if (!matched)
						break;
				}

				if(matched)
				{
					if (Debug)

					{
						Console.WriteLine($"Chunk of size {Size} at col {x}, row {y} matched Rule {Id}!");

						for (int row = 0; row < Size + 1; row++)
						{
							Console.Write("\t");
							for (int col = 0; col < Size + 1; col++)
							{
								if (row < Size && col < Size)
								{
									Console.Write(m_input[row][col]);
								}
								else
								{
									Console.Write(' ');
								}
							}

							if (row == 1)
								Console.Write(" => ");
							else
								Console.Write("    ");

							for (int col = 0; col < Size + 1; col++)
							{
								Console.Write(m_output[row][col]);
							}

							Console.WriteLine();
						}
					}

					return true;
				}
			}

			return false;
		}

		public void Write(ref char[][] map, int x, int y)
		{
			for(int row=0; row < Size+1; row++)
			{
				for(int col = 0; col < Size+1; col++)
				{
					map[y + row][x + col] = m_output[row][col];
				}
			}
		}
	}

	class Day21Strategy : DayLineLoaderBasic
	{
		List<Rule> m_rules = new List<Rule>();

		int m_count = 0;
		public override void ParseInputLine(string line)
		{
			m_rules.Add(new Rule(m_count, line));
			m_count++;
		}

		char[][] m_init = null;
		int m_size = 3;

		char[][] m_curr = null;
		char[][] m_next = null;

		int RunStep()
		{
			int div = (m_size % 2 == 0) ? 2 : 3;
			if (div == 2)
				Run2Step();
			else
				Run3Step();

			ArrayUtil.Build2DArray(ref m_curr, m_next.Length, m_next.Length, ' ');

			ArrayUtil.Copy(ref m_curr, m_next);

			return div;
		}

		void Run2Step()
		{
			int numChunks = m_size / 2;
			ArrayUtil.Build2DArray(ref m_next, numChunks * 3, numChunks * 3, ' ');

			for(int row=0; row < numChunks; row++)
			{
				for(int col=0; col < numChunks; col++)
				{
					foreach(Rule r in m_rules.Where(x => x.Size == 2))
					{
						if(r.IsMatch(m_curr, col*2, row*2))
						{
							r.Write(ref m_next, col*3, row*3);
						}
					}
				}
			}

			m_size = numChunks * 3;
		}

		void Run3Step()
		{
			int numChunks = m_size / 3;
			ArrayUtil.Build2DArray(ref m_next, numChunks * 4, numChunks * 4, ' ');

			for(int row=0; row < numChunks; row++)
			{
				for(int col=0; col < numChunks; col++)
				{
					foreach(Rule r in m_rules.Where(x => x.Size == 3))
					{
						if(r.IsMatch(m_curr, col*3, row*3))
						{
							r.Write(ref m_next, col * 4, row * 4);
						}
					}
				}
			}

			m_size = numChunks * 4;
		}

		void PrintArray(char[][] arr, int dividers)
		{
			for(int row = 0; row < arr.Length; row++)
			{
				if (row > 0 && row % dividers == 0)
				{
					for (int col = 0; col < arr.Length; col++)
					{
						if (col > 0 && col % dividers == 0)
							Console.Write(' ');
						Console.Write(' ');
					}
					Console.WriteLine();
				}
				for (int col = 0; col < arr.Length; col++)
				{
					if (col > 0 && col % dividers == 0)
						Console.Write(' ');
					Console.Write(arr[row][col]);
				}
				Console.WriteLine();

			}

		}
		public override string Part1()
		{
			// TEST
			//m_rules = new List<Rule>();
			//m_rules.Add(new Rule("../.# => ##./#../..."));
			//m_rules.Add(new Rule(".#./..#/### => #..#/..../..../#..#"));

			m_init = new char[3][];
			m_init[0] = new char[3] { '.', '#', '.' };
			m_init[1] = new char[3] { '.', '.', '#' };
			m_init[2] = new char[3] { '#', '#', '#' };

			ArrayUtil.Build2DArray(ref m_curr, 3, 3, ' ');
			ArrayUtil.Copy(ref m_curr, m_init);

			Console.WriteLine("Iteration 0:");
			PrintArray(m_curr, 3);
			Console.WriteLine();

			for (int row=0; row < 5; row++)
			{
				int div = (m_size % 2 == 0) ? 2 : 3;
				Console.WriteLine($"Iteration {row + 1} Input:");
				PrintArray(m_curr, div);
				Console.WriteLine();
				int divOut = RunStep();
				Console.WriteLine($"Iteration {row+1} Output:");
				PrintArray(m_curr, divOut+1);
				Console.WriteLine();
			}

			return m_curr.Sum(x => x.Sum(y => y == '#' ? 1 : 0)).ToString();
		}

		public override string Part2()
		{
			for(int iter = 5; iter < 18; iter++)
			{
				RunStep();
			}

			return m_curr.Sum(x => x.Sum(y => y == '#' ? 1 : 0)).ToString();
		}
	}
}
