using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Day17Strategy : DayLineLoaderBasic
	{
		int m_row = 0;

		Dictionary<Vec3, char> m_data = new Dictionary<Vec3, char>();
		Dictionary<Vec4, char> m_data4 = new Dictionary<Vec4, char>();

		public override void ParseInputLine(string line, int lineNum)
		{
			int col = 0;
			foreach(var c in line)
			{
				Vec3 pos = new Vec3(col, m_row, 0);
				m_data[pos] = c;
				Vec4 pos4 = new Vec4(col, m_row, 0, 0);
				m_data4[pos4] = c;
				col++;
			}
			m_row++;
		}

		private char GetVal(Vec3 pos)
		{
			char c = '.';
			if (m_data.ContainsKey(pos))
			{
				c = m_data[pos];
			}
			return c;
		}

		private char GetVal(Vec4 pos)
		{
			char c = '.';
			if (m_data4.ContainsKey(pos))
			{
				c = m_data4[pos];
			}
			return c;
		}

		private void Print()
		{
			int minX = m_data.Keys.Min(o => o.X);
			int maxX = m_data.Keys.Max(o => o.X);
			int minY = m_data.Keys.Min(o => o.Y);
			int maxY = m_data.Keys.Max(o => o.Y);
			int minZ = m_data.Keys.Min(o => o.Z);
			int maxZ = m_data.Keys.Max(o => o.Z);

			for(int z=minZ; z <= maxZ; z++)
			{
				Console.WriteLine($"z={z}");
				for(int y=minY; y <= maxY; y++)
				{
					for(int x = minX; x <= maxX; x++)
					{
						Vec3 pos = new Vec3(x, y, z);
						Console.Write(m_data[pos]);
					}
					Console.WriteLine();
				}
				Console.WriteLine();
			}

		}

		private void Step()
		{
			Dictionary<Vec3, char> next = new Dictionary<Vec3, char>();
			int minX = m_data.Keys.Min(o => o.X) - 1;
			int maxX = m_data.Keys.Max(o => o.X) + 1;
			int minY = m_data.Keys.Min(o => o.Y) - 1;
			int maxY = m_data.Keys.Max(o => o.Y) + 1;
			int minZ = m_data.Keys.Min(o => o.Z) - 1;
			int maxZ = m_data.Keys.Max(o => o.Z) + 1;

			for(int i=minX; i <= maxX; i++)
			{
				for(int j=minY; j <= maxY; j++)
				{
					for(int k = minZ; k <= maxZ; k++)
					{
						Vec3 pos = new Vec3(i, j, k);
						char c = GetVal(pos);

						var numNeighbors = 0;
						foreach(var offset in Vec3.Adjacent)
						{
							Vec3 neighbor = pos + offset;
							char n = GetVal(neighbor);
							if(n == '#')
							{
								numNeighbors++;
							}
						}

						if(c == '#') 
						{
							if(numNeighbors >= 2 && numNeighbors <= 3)
							{
								next[pos] = '#';
							}
							else
							{
								next[pos] = '.';
							}
						}
						else
						{
							if(numNeighbors == 3)
							{
								next[pos] = '#';
							}
							else
							{
								next[pos] = '.';
							}
						}
						
					}
				}
			}

			m_data = next;
		}

		public override string Part1()
		{
			//Print();
			//Console.ReadKey();

			for (int i=0; i < 6; i++)
			{
				Step();
				//Print();
				//Console.ReadKey();
			}

			return m_data.Values.Count(x => x == '#').ToString();
		}

		private void Step4D()
		{
			Dictionary<Vec4, char> next = new Dictionary<Vec4, char>();
			int minX = m_data4.Keys.Min(o => o.X) - 1;
			int maxX = m_data4.Keys.Max(o => o.X) + 1;
			int minY = m_data4.Keys.Min(o => o.Y) - 1;
			int maxY = m_data4.Keys.Max(o => o.Y) + 1;
			int minZ = m_data4.Keys.Min(o => o.Z) - 1;
			int maxZ = m_data4.Keys.Max(o => o.Z) + 1;
			int minW = m_data4.Keys.Min(o => o.W) - 1;
			int maxW = m_data4.Keys.Max(o => o.W) + 1;

			for (int i = minX; i <= maxX; i++)
			{
				for (int j = minY; j <= maxY; j++)
				{
					for (int k = minZ; k <= maxZ; k++)
					{
						for (int w = minW; w <= maxW; w++)
						{
							Vec4 pos = new Vec4(i, j, k, w);
							char c = GetVal(pos);

							var numNeighbors = 0;
							List<Vec3> check = new List<Vec3>(Vec3.Adjacent);
							check.Add(new Vec3(0, 0, 0));
							foreach (var offset in check)
							{
								for(var woff = -1; woff <= 1; woff++)
								{
									Vec4 off4 = new Vec4(offset.X, offset.Y, offset.Z, woff);
									Vec4 neighbor = pos + off4;

									if (neighbor == pos)
									{
										//Console.WriteLine($"Discarding {pos} + {off4} = {neighbor}");
										continue;
									}
									//Console.WriteLine($"Checking {pos} + {off4} = {neighbor}");
									char n = GetVal(neighbor);
									if (n == '#')
									{
										numNeighbors++;
									}
								}
							}

							if (c == '#')
							{
								if (numNeighbors >= 2 && numNeighbors <= 3)
								{
									next[pos] = '#';
								}
								else
								{
									next[pos] = '.';
								}
							}
							else
							{
								if (numNeighbors == 3)
								{
									next[pos] = '#';
								}
								else
								{
									next[pos] = '.';
								}
							}
						}

					}
				}
			}

			m_data4 = next;
		}

		public override string Part2()
		{

			for (int i = 0; i < 6; i++)
			{
				Step4D();
				//Print();
			}

			return m_data4.Values.Count(x => x == '#').ToString();
		}
	}
}
