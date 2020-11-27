using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class NavNode
	{
		public char Id = ' ';

		public bool IsLocked(Route r)
		{
			if (!Day18Strategy.IsLock(Id))
				return false;

			char key = (char)(Id + 32);

			if(r.Order.Select(nn => nn.Id).Contains(key))
			{
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			return $"[{Id}]";
		}

	}

	class NavEdge
	{
		public NavNode A { get; set; }
		public NavNode B { get; set; }
		public int Distance;

		public override string ToString()
		{
			return $"[{A.Id}] is {Distance} from [{B.Id}]";
		}
	}


	class BfsNode
	{
		public BfsNode()
		{
			Distance = int.MaxValue;
			Point = new Vec2(0,0);
			NavNode = null;
		}

		public BfsNode(Vec2 point, int dist, NavNode n)
		{
			Point = point;
			Distance = dist;
			NavNode = n;
		}

		public override string ToString()
		{
			return $"{Point} : {Distance} from {NavNode}";
		}

		public int Distance;
		public Vec2 Point;
		public NavNode NavNode;
	}

	class Route
	{
		public List<NavNode> Order = new List<NavNode>();

		public int Length = 0;

		public Route Clone()
		{
			var newRoute = new Route();
			newRoute.Order.AddRange(Order);
			newRoute.Length = Length;
			return newRoute;
		}

		public bool Equivalent(object obj)
		{
			Route other = obj as Route;

			var keys = Order.Where(x => Day18Strategy.IsKey(x.Id));
			var otherKeys = other.Order.Where(x => Day18Strategy.IsKey(x.Id));

			if(other != null && otherKeys.Count() == keys.Count() && other.Order.Last() == Order.Last())
			{
				if(otherKeys.Except(keys).Count() == 0)
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			string s = "";
			foreach(var nn in Order)
			{
				if (Day18Strategy.IsKey(nn.Id))
				{
					s += nn.Id;
				}
			}

			s += ", length " + Length;
			return s;
		}

		public bool HasAllKeys(List<char> keys)
		{
			var overlap = Order.Select(nn => nn.Id).Intersect(keys);
			return overlap.Count() == keys.Count();
		}
	}

	class Day18Strategy : DayMapInputFileStrategy
	{
		char[][] m_map;

		Vec2 m_startPos;
		NavNode m_startNode;
		HashSet<NavNode> m_nodes = new HashSet<NavNode>();
		HashSet<NavEdge> m_edges = new HashSet<NavEdge>();
		List<char> m_keys = new List<char>();

		public Day18Strategy(string file) : base(file)
		{
			ArrayUtil.Build2DArray<char>(ref m_map, 82, 82, '.');
		}

		public static bool IsLockOrKey(char c)
		{
			return IsLock(c) || IsKey(c);
		}

		public static bool IsKey(char c)
		{
			return (c >= 'a' && c <= 'z');
		}

		public static bool IsLock(char c)
		{
			return (c >= 'A' && c <= 'Z');
		}

		private NavNode CreateNavNode(char c)
		{
			var n = new NavNode();
			n.Id = c;

			return n;
		}

		private IEnumerable<NavNode> GetNeighbors(NavNode a)
		{
			foreach(var edge in m_edges.Where(x => x.A == a || x.B == a))
			{
				if (edge.A == a)
					yield return edge.B;
				else
					yield return edge.A;
			}
		}

		private int DistanceFrom(NavNode a, NavNode b)
		{
			var edge = m_edges.Where(x => (x.A == a && x.B == b) || (x.A == b && x.B == a)).First();

			return edge.Distance;
		}

		// Get interesting destinations from this node, with distances
		private IEnumerable<Tuple<NavNode, int>> GetDestinations(NavNode a, Route route)
		{
			IList<NavNode> skip = route.Order;

			HashSet<NavNode> visited = new HashSet<NavNode>();
			
			Queue<Tuple<NavNode, int>> resolve = new Queue<Tuple<NavNode, int>>();

			visited.Add(a);

			foreach(var neighbor in GetNeighbors(a))
			{
				if (!neighbor.IsLocked(route))
				{
					int dist = DistanceFrom(a, neighbor);
					resolve.Enqueue(new Tuple<NavNode, int>(neighbor, dist));
					visited.Add(neighbor);
				}
			}

			while(resolve.Any(x => !IsKey(x.Item1.Id)) || resolve.Any(x => skip.Contains(x.Item1)))
			{
				var next = resolve.Dequeue();

				if(!IsKey(next.Item1.Id) || skip.Contains(next.Item1))
				{
					foreach(var neighbor in GetNeighbors(next.Item1))
					{
						if (!visited.Contains(neighbor) && !neighbor.IsLocked(route))
						{
							int dist = DistanceFrom(next.Item1, neighbor);
							resolve.Enqueue(new Tuple<NavNode, int>(neighbor, dist + next.Item2));
							visited.Add(neighbor);
						}
					}
				}
				else
				{
					resolve.Enqueue(next);
				}
			}

			return resolve;
		}

		private void BuildGraph()
		{
			Queue<BfsNode> process = new Queue<BfsNode>();
			HashSet<Vec2> visited = new HashSet<Vec2>();

			process.Enqueue(new BfsNode(m_startPos, 0, m_startNode));
			visited.Add(m_startPos);


			while(process.Count > 0)
			{
				BfsNode n = process.Dequeue();

				char mapChar = m_map[n.Point.X][n.Point.Y];

				List<Vec2> navigableNeighbors = new List<Vec2>();

				foreach (var offset in Vec2.CardinalAdjacent)
				{
					Vec2 neighbor = n.Point + offset;

					char neighborChar = m_map[neighbor.X][neighbor.Y];

					if(neighborChar != '#' && !visited.Contains(neighbor))
					{
						navigableNeighbors.Add(neighbor);
					}
				}

				NavNode nextNavNode = n.NavNode;
				int nextDistance = n.Distance + 1;

				// If this position is not already represented by a NavNode
				if (n.Distance != 0)
				{
					// If this map position is a special char OR an intersection
					if (IsLockOrKey(mapChar) || navigableNeighbors.Count > 1)
					{
						// Add a NavNode for this point
						NavNode newNavNode = CreateNavNode(mapChar);
						m_nodes.Add(newNavNode);
						nextNavNode = newNavNode;

						// Add a NavEdge connecting to the previous NavNode
						NavEdge newEdge = new NavEdge();
						newEdge.A = n.NavNode;
						newEdge.B = newNavNode;
						newEdge.Distance = n.Distance;
						m_edges.Add(newEdge);

						nextDistance = 1;
					}
				}

				foreach(var neighbor in navigableNeighbors)
				{
					BfsNode b = new BfsNode(neighbor, nextDistance, nextNavNode);
					process.Enqueue(b);
					visited.Add(b.Point);
				}
			}
		}

		public override string Part1()
		{
			m_startNode = CreateNavNode('.');
			m_nodes.Add(m_startNode);

			BuildGraph();

			foreach(NavNode x in m_nodes)
			{
				Console.WriteLine($"Nav Node [{x.Id}]: ");
				foreach(NavEdge e in m_edges.Where(edge => edge.A == x || edge.B == x))
				{
					Console.WriteLine($"    Edge of distance {e.Distance} connecting {e.A.Id} to {e.B.Id}");
				}
			}

			Queue<Route> pending = new Queue<Route>();
			Route r = new Route();
			r.Order.Add(m_startNode);
			r.Length = 0;

			pending.Enqueue(r);

			HashSet<Route> succeeded = new HashSet<Route>();

			int counter = 0;

			while(pending.Count > 0)
			{
				counter++;
				if(counter % 10000 == 0)
				{
					Console.WriteLine($"{pending.Count} routes pending...");
				}

				var curr = pending.Dequeue();

				if(curr.HasAllKeys(m_keys))
				{
					Console.WriteLine($"Route [{curr}] has all keys!");
					succeeded.Add(curr);
				}
				else
				{
					//Console.WriteLine($"Processing Route [{curr}]... ({pending.Count} routes pending)");
				}
				
				var lastNode = curr.Order.Last();

				foreach(var neighbor in GetDestinations(lastNode, curr))
				{
					//Console.WriteLine($"  Destination {neighbor.Item1} is available");
					Route newRoute = curr.Clone();
					newRoute.Order.Add(neighbor.Item1);
					newRoute.Length += neighbor.Item2;

					bool found = false;
					foreach(var p in pending)
					{
						if(p.Equivalent(newRoute))
						{
							if(newRoute.Length < p.Length)
							{
								//Console.WriteLine($"    Updating route [{p}] to [{newRoute}].");
								p.Length = newRoute.Length;
								p.Order = newRoute.Order;
							}
							found = true;
						}
					}

					if (!found)
					{
						//Console.WriteLine($"    Adding route [{newRoute}].");
						pending.Enqueue(newRoute);
					}
				}
			}
			

			return succeeded.OrderBy(x => x.Length).First().ToString();
		}

		public override string Part2()
		{
			return "";
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			m_map[position.X][position.Y] = c;

			if(c == '@')
			{
				m_startPos = position;
			}

			if(IsKey(c))
			{
				m_keys.Add(c);
			}
		}
	}
}
