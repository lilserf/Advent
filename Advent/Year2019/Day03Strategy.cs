using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Wire
	{
		List<Tuple<int, int>> Points = new List<Tuple<int, int>>();

		List<Tuple<int, int, int>> Verticals = new List<Tuple<int, int, int>>();
		List<Tuple<int, int, int>> Horizontals = new List<Tuple<int, int, int>>();

		public int NavigateTo(int x, int y)
		{
			int distance = 0;
			Tuple<int, int> p1 = new Tuple<int, int>(0, 0);

			for(int i=0; i < Points.Count; i++)
			{
				var p2 = Points[i];

				// If it's on the line
				if (p1.Item2 == y && y == p2.Item2)
				{
					if ((p1.Item1 <= x && x <= p2.Item1) ||
						(p2.Item1 <= x && x <= p1.Item1))
					{
						distance += Math.Abs(x - p1.Item1);
						break;
					}
				}
				else if(p1.Item1 == x && x == p2.Item1)
				{
					if ((p1.Item2 <= y && y <= p2.Item2) ||
						(p2.Item2 <= y && y <= p1.Item2))
					{
						distance += Math.Abs(y - p1.Item2);
						break;
					}
				}

				distance += Math.Abs(p2.Item1 - p1.Item1) + Math.Abs(p2.Item2 - p1.Item2);

				p1 = p2;
			}

			return distance;
		}

		public void AddPoint(int x, int y)
		{
			var lastPoint = Points.Count > 0 ? Points.Last() : new Tuple<int, int>(0,0);

			if(x == lastPoint.Item1)
			{
				int minY = Math.Min(lastPoint.Item2, y);
				int length = Math.Abs(lastPoint.Item2 - y);

				Verticals.Add(new Tuple<int, int, int>(x, minY, length));
			}
			else
			{
				int minX = Math.Min(lastPoint.Item1, x);
				int length = Math.Abs(lastPoint.Item1 - x);

				Horizontals.Add(new Tuple<int, int, int>(minX, y, length));
			}

			Points.Add(new Tuple<int, int>(x, y));
		}

		private Tuple<int, int> Intersection(Tuple<int,int,int> horiz, Tuple<int,int,int> vert)
		{
			if(horiz.Item1 <= vert.Item1 && horiz.Item1 + horiz.Item3 >= vert.Item1 &&
				vert.Item2 <= horiz.Item2 && vert.Item2 + vert.Item3 >= horiz.Item2)
			{
				return new Tuple<int, int>(vert.Item1, horiz.Item2);
			}

			return null;
		}

		public IEnumerable<Tuple<int,int>> Intersect(Wire other)
		{
			foreach(var h in Horizontals)
			{
				foreach(var v in other.Verticals)
				{
					var i = Intersection(h, v);
					if(i != null)
					{
						yield return i;
					}
				}
			}

			foreach(var v in Verticals)
			{
				foreach(var h in other.Horizontals)
				{
					var i = Intersection(h, v);
					if(i != null)
					{
						yield return i;
					}
				}
			}
		}
	}

	class Day03Strategy : DayInputFileStrategy
	{
		List<Wire> m_wires = new List<Wire>();

		List<Tuple<int, int>> m_crosses = new List<Tuple<int, int>>();

		public Day03Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			//m_wires = new List<Wire>();
			//ParseInputLine("R75,D30,R83,U83,L12,D49,R71,U7,L72");
			//ParseInputLine("U62,R66,U55,R34,D71,R55,D58,R83");
			//ParseInputLine("R8,U5,L5,D3");
			//ParseInputLine("U7,R6,D4,L4");

			Tuple<int, int> closest = new Tuple<int,int>(int.MaxValue/2, int.MaxValue/2);

			for(int i=1; i < m_wires.Count; i++)
			{
				for(int j=i-1; j >= 0; j--)
				{
					var points = m_wires[i].Intersect(m_wires[j]);

					foreach(var p in points)
					{
						if (p.Item1 == 0 && p.Item2 == 0)
							continue;

						m_crosses.Add(p);

						if(Math.Abs(p.Item1) + Math.Abs(p.Item2) < Math.Abs(closest.Item1) + Math.Abs(closest.Item2))
						{
							closest = p;
						}
					}
				}
			}

			return closest.ToString() + " : " + (Math.Abs(closest.Item1) + Math.Abs(closest.Item2)).ToString();
		}

		public override string Part2()
		{
			Tuple<int, int> best = null;
			int bestScore = int.MaxValue;

			foreach(var p in m_crosses)
			{
				int score = 0;
				foreach(var w in m_wires)
				{
					score += w.NavigateTo(p.Item1, p.Item2);
				}

				if (score < bestScore)
				{
					best = p;
					bestScore = score;
				}
			}

			return best.ToString() + " : " + bestScore;
		}

		protected override void ParseInputLine(string line, int lineNum)
		{
			var bits = line.Split(',');

			Wire w = new Wire();
			int x = 0, y = 0;
			foreach(var b in bits)
			{
				int dist = int.Parse(b.Substring(1));
				switch(b[0])
				{
					case 'R': x += dist; break;
					case 'L': x -= dist; break;
					case 'U': y -= dist; break;
					case 'D': y += dist; break;
				}

				w.AddPoint(x, y);
			}

			m_wires.Add(w);
		}
	}
}
