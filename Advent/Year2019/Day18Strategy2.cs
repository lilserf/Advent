using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class MegaRoute : IComparable<MegaRoute>
	{
		public List<char> Order { get; private set; }
		public int Length { get; private set; }
		public IList<Vec2> BotPositions { get; private set; }

		public MegaRoute(IList<Vec2> positions)
		{
			BotPositions = new List<Vec2>(positions);
			Length = 0;
			Order = new List<char>();
		}

		public void Add(int bot, char c, int distance, Vec2 newPosition)
		{
			Order.Add(c);
			Length += distance;
			BotPositions[bot] = newPosition;
		}

		public MegaRoute Clone()
		{
			MegaRoute other = new MegaRoute(BotPositions);
			other.Order.AddRange(Order);
			other.Length = Length;
			return other;
		}

		public override string ToString()
		{
			string order = Order.Aggregate("", (s, x) => s += x);
			return $"[{order}, length {Length}]";
		}

		public override int GetHashCode()
		{
			int posHash = Hash.Create(BotPositions);
			int keyHash = Hash.Create(Order.OrderBy(c => c));
			return Hash.Create(posHash, keyHash);
		}

		public override bool Equals(object obj)
		{
			var other = obj as MegaRoute;
			return (Order.Equals(other.Order) && Length == other.Length && BotPositions.Equals(other.BotPositions));
		}

		public bool Equivalent(MegaRoute other)
		{
			if (!Enumerable.SequenceEqual(BotPositions, other.BotPositions))
				return false;

			if (other.Order.Count != Order.Count)
				return false;

			if (other.Order.Intersect(Order).Count() != Order.Count)
				return false;

			return true;
		}

		public void Update(MegaRoute other)
		{
			Order = other.Order;
			Length = other.Length;
		}

		public int CompareTo(MegaRoute other)
		{
			return Length.CompareTo(other.Length);
		}
	}

	class KeyRoute
	{
		public List<char> Order { get; private set; }
		public int Length { get; private set; }
		public Vec2 CurrentPosition { get; set; }

		public KeyRoute(Vec2 startPos)
		{
			CurrentPosition = startPos;
			Length = 0;
			Order = new List<char>();
		}

		public void Add(char c, int distance, Vec2 newPosition)
		{
			Order.Add(c);
			Length += distance;
			CurrentPosition = newPosition;
		}

		public KeyRoute Clone()
		{
			KeyRoute other = new KeyRoute(CurrentPosition);
			other.Order.AddRange(Order);
			other.Length = Length;
			return other;
		}

		public override string ToString()
		{
			string order = Order.Aggregate("", (s, x) => s += x);
			return $"[{order}, length {Length}]";
		}

		public override int GetHashCode()
		{
			return Hash.Create(Order, Length, CurrentPosition); 
		}

		public override bool Equals(object obj)
		{
			var other = obj as KeyRoute;
			return (Order.Equals(other.Order) && Length == other.Length && CurrentPosition == other.CurrentPosition);
		}

		public bool Equivalent(KeyRoute other)
		{
			if (other.CurrentPosition != CurrentPosition)
				return false;

			if (other.Order.Count != Order.Count)
				return false;

			if (other.Order.Intersect(Order).Count() != Order.Count)
				return false;

			return true;
		}

		public void Update(KeyRoute other)
		{
			Order = other.Order;
			Length = other.Length;
		}

	}

	class Day18Strategy2 : DayMapInputFileStrategy
	{
		const int MAP_WIDTH = 82;
		const int MAP_HEIGHT = 82;
		char[][] m_map;
		int[][] m_bfs;
		int[][] m_bot;
		Vec2 m_startPos;
		Dictionary<char, Vec2> m_keys;

		static bool s_debug = false;
		static bool s_drawBfs = false;

		public Day18Strategy2(string inputFile) : base(inputFile)
		{
			ArrayUtil.Build2DArray(ref m_map, MAP_WIDTH, MAP_HEIGHT, '?');
			ArrayUtil.Build2DArray(ref m_bfs, MAP_WIDTH, MAP_HEIGHT, int.MaxValue);
			ArrayUtil.Build2DArray(ref m_bot, MAP_WIDTH, MAP_HEIGHT, int.MaxValue);

			m_keys = new Dictionary<char, Vec2>();
		}

		private void ClearBfs()
		{
			for(int i=0; i < MAP_WIDTH; i++)
			{
				for(int j = 0; j < MAP_HEIGHT; j++)
				{
					m_bfs[i][j] = int.MaxValue;
				}
			}
		}

		private void BuildBfsInternal(Vec2 pos, List<char> order, int bot=0)
		{
			m_bot[pos.X][pos.Y] = bot;
			m_bfs[pos.X][pos.Y] = 0;

			Queue<Vec2> pending = new Queue<Vec2>();

			pending.Enqueue(pos);

			while (pending.Count > 0)
			{
				Vec2 curr = pending.Dequeue();

				int dist = m_bfs[curr.X][curr.Y];

				foreach (var offset in Vec2.CardinalAdjacent)
				{
					Vec2 neighbor = curr + offset;

					if (m_bfs[neighbor.X][neighbor.Y] == int.MaxValue)
					{
						char c = m_map[neighbor.X][neighbor.Y];

						if (c == '#')
							continue;

						if (c >= 'A' && c <= 'Z')
						{
							char corrKey = (char)(c + 32);
							if (!order.Contains(corrKey))
							{
								continue;
							}
						}

						m_bot[neighbor.X][neighbor.Y] = bot;
						m_bfs[neighbor.X][neighbor.Y] = dist + 1;
						pending.Enqueue(neighbor);
					}
				}
			}
		}

		private void BuildBfs(MegaRoute mr)
		{
			ClearBfs();

			for(int i=0; i < mr.BotPositions.Count; i++)
			{
				BuildBfsInternal(mr.BotPositions[i], mr.Order, i);
			}
		}

		private void BuildBfs(KeyRoute kr)
		{
			ClearBfs();

			BuildBfsInternal(kr.CurrentPosition, kr.Order);
		}

		private bool HasAllKeys(KeyRoute kr)
		{
			var overlap = kr.Order.Intersect(m_keys.Keys);
			return overlap.Count() == m_keys.Count();
		}

		private bool HasAllKeys(MegaRoute mr)
		{
			var overlap = mr.Order.Intersect(m_keys.Keys);
			return overlap.Count() == m_keys.Count();
		}

		static bool skipPart1 = true;

		public override string Part1()
		{
			if (skipPart1) return "Skipped";

			KeyRoute k = new KeyRoute(m_startPos);

			Queue<KeyRoute> routes = new Queue<KeyRoute>();
			routes.Enqueue(k);

			List<KeyRoute> done = new List<KeyRoute>();

			int counter = 0;
			while (routes.Count > 0)
			{
				counter++;
				if(counter % 1000 == 0)
					Console.WriteLine($"{routes.Count} routes pending...");

				KeyRoute currRoute = routes.Dequeue();
				if(s_debug) Console.WriteLine($"Processing {currRoute}...");

				if(HasAllKeys(currRoute))
				{
					if (s_debug) Console.WriteLine($"  Route {currRoute} has all keys!");
					done.Add(currRoute);
				}

				BuildBfs(currRoute);

				foreach (var kvp in m_keys)
				{
					char c = kvp.Key;

					if (currRoute.Order.Contains(c))
						continue;

					int distance = m_bfs[kvp.Value.X][kvp.Value.Y];
					if (distance != int.MaxValue)
					{
						KeyRoute newRoute = currRoute.Clone();
						newRoute.Add(c, distance, kvp.Value);

						var equiv = routes.Where(r => r.Equivalent(newRoute)).FirstOrDefault();

						if (equiv == null)
						{
							routes.Enqueue(newRoute);
							if (s_debug) Console.WriteLine($"Added new route {newRoute}.");
						}
						else
						{
							if(s_debug) Console.WriteLine($"Found equivalent route: {equiv}");
							if(equiv.Length > newRoute.Length)
							{
								equiv.Update(newRoute);
								if (s_debug) Console.WriteLine($"    Updated to {newRoute}");
							}
						}
					}
				}
			}

			foreach(var route in done)
			{
				Console.WriteLine(route);
			}

			return done.OrderBy(kr => kr.Length).First().ToString();
		}

		Vec2[] m_startPositions;

		static ConsoleColor[] s_colors = new ConsoleColor[4]
		{
						ConsoleColor.Red,
						ConsoleColor.Yellow,
						ConsoleColor.Green,
						ConsoleColor.Blue
		};

		public void PrintBfs()
		{
			for(int j=0; j < MAP_HEIGHT; j++)
			{
				if (m_map[0][j] == '?')
					break;

				for (int i = 0; i < MAP_WIDTH; i++)
				{
					char c = m_map[i][j];
					if (c == '#')
					{
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write(c);
						continue;
					}

					if (c == '?')
						continue;

					int dist = m_bfs[i][j];

					if (dist == int.MaxValue)
					{
						Console.ForegroundColor = ConsoleColor.DarkGray;
						Console.Write('?');
						continue;
					}

					int bot = m_bot[i][j];

					Console.ForegroundColor = s_colors[bot];
					Console.Write(dist % 10);
				}
				Console.WriteLine();
			}
		}

		public override string Part2()
		{
			m_startPositions = new Vec2[] {
				(m_startPos + Vec2.UpLeft),
				(m_startPos + Vec2.UpRight),
				(m_startPos + Vec2.DownLeft),
				(m_startPos + Vec2.DownRight)
			};

			m_map[m_startPos.X-1][m_startPos.Y-1] = '@';
			m_map[m_startPos.X-1][m_startPos.Y+1] = '@';
			m_map[m_startPos.X+1][m_startPos.Y+1] = '@';
			m_map[m_startPos.X+1][m_startPos.Y-1] = '@';
			m_map[m_startPos.X][m_startPos.Y] = '#';
			m_map[m_startPos.X+1][m_startPos.Y] = '#';
			m_map[m_startPos.X-1][m_startPos.Y] = '#';
			m_map[m_startPos.X][m_startPos.Y+1] = '#';
			m_map[m_startPos.X][m_startPos.Y-1] = '#';

			MegaRoute k = new MegaRoute(m_startPositions);

			MinHeap<MegaRoute> routes = new MinHeap<MegaRoute>(200000);
			routes.Add(k);

			Dictionary<int, MegaRoute> bestRoutes = new Dictionary<int, MegaRoute>();

			List<MegaRoute> done = new List<MegaRoute>();

			int counter = 0;
			int discarded = 0;
			while (!routes.IsEmpty())
			{
				counter++;
				if (counter % 2000 == 0)
					Console.WriteLine($"{routes.Count()} routes pending ({discarded} discarded)... Best route currently {routes.Peek()}");

				MegaRoute currRoute = routes.Pop();

				if (s_debug) Console.WriteLine($"Processing {currRoute}...");
				if (bestRoutes.ContainsKey(currRoute.GetHashCode()))
				{
					var best = bestRoutes[currRoute.GetHashCode()];
					if(s_debug) Console.WriteLine($"    Found existing route {best}");
					
					if(currRoute.Length < best.Length)
					{
						bestRoutes[currRoute.GetHashCode()] = currRoute;
					}
					else
					{
						if (s_debug) Console.WriteLine($"    Skipping!");
						discarded++;
						continue;
					}
				}
				else
				{
					bestRoutes.Add(currRoute.GetHashCode(), currRoute);
				}


				if (HasAllKeys(currRoute))
				{
					if (s_debug) Console.WriteLine($"  Route {currRoute} has all keys!");
					done.Add(currRoute);
					break;
				}

				
				BuildBfs(currRoute);
				if(s_drawBfs) PrintBfs();

				foreach (var kvp in m_keys)
				{
					char c = kvp.Key;

					if (currRoute.Order.Contains(c))
						continue;

					int distance = m_bfs[kvp.Value.X][kvp.Value.Y];
					int bot = m_bot[kvp.Value.X][kvp.Value.Y];
					if (distance != int.MaxValue)
					{
						var newRoute = currRoute.Clone();
						newRoute.Add(bot, c, distance, kvp.Value);

						routes.Add(newRoute);
						if (s_debug) Console.WriteLine($"Added new route {newRoute}.");
					}
				}

				if(s_drawBfs) Console.ReadKey();

			}

			foreach (var route in done)
			{
				Console.WriteLine(route);
			}

			return done.OrderBy(kr => kr.Length).First().ToString();
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_map[position.X][position.Y] = c;

			if(c == '@')
			{
				m_startPos = position;
			}
			else if (c >= 'a' && c <= 'z')
			{
				m_keys[c] = position;
			}
		}
	}
}
