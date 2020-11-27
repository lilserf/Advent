using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day10Strategy : DayMapInputFileStrategy
	{
		HashSet<Vec2> m_asteroids = new HashSet<Vec2>();
		Dictionary<Vec2, HashSet<Vec2>> m_seen = new Dictionary<Vec2, HashSet<Vec2>>();

		public Day10Strategy(string file) : base(file)
		{

		}

		public int gcd(int a, int b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			while(b > 0)
			{
				int rem = a % b;
				a = b;
				b = rem;
			}
			return a;
		}

		private Vec2 GetStep(Vec2 point, Vec2 to)
		{
			Vec2 angle = to - point;
			int g = gcd(angle.X, angle.Y);
			return angle / g;
		}

		public override string Part1()
		{
			foreach(var point in m_asteroids)
			{
				foreach (var asteroid in m_asteroids)
				{
					if (point != asteroid)
					{
						Vec2 step = GetStep(point, asteroid);

						Vec2 curr = point + step;
						while (curr != asteroid)
						{
							if(m_asteroids.Contains(curr))
							{
								break;
							}
							curr += step;
						}

						if (!m_seen.ContainsKey(point))
							m_seen[point] = new HashSet<Vec2>();
						if(!m_seen[point].Contains(curr))
							m_seen[point].Add(curr);
					
					}
				}
			}

			return m_seen.Values.Select(x => x.Count).Max().ToString();
		}

		public override string Part2()
		{
			// Find the point that sees the most asteroids
			var point = m_seen.Aggregate((l, r) => l.Value.Count() > r.Value.Count() ? l : r).Key;

			List<Vec2> destroyed = new List<Vec2>();

			// Map the "step" (most simplified slope that reaches an asteroid) to the list of asteroids it reaches
			Dictionary<Vec2, List<Vec2>> stepToAsteroids = new Dictionary<Vec2, List<Vec2>>();

			foreach (var asteroid in m_asteroids)
			{
				if (point != asteroid)
				{
					Vec2 step = GetStep(point, asteroid);

					if (!stepToAsteroids.ContainsKey(step))
					{
						stepToAsteroids[step] = new List<Vec2>();
					}

					stepToAsteroids[step].Add(asteroid);
				}
			}

			// Build a list of (Step, Angle) pairs, map them to between -pi/2 and 3pi/2, and sort them by angle
			var angleToSteps = stepToAsteroids.Keys.Select(x => new { Step = x, Angle = Math.Atan2(x.Y, x.X) }).Select(x =>
				x.Angle < (-Math.PI / 2) ? new { Step = x.Step, Angle = x.Angle + (2 * Math.PI) } : x).OrderBy(x => x.Angle);

			// At each angle, build an ordered list of asteroids by distance
			Queue<Queue<Vec2>> order = new Queue<Queue<Vec2>>();
			foreach(var item in angleToSteps)
			{
				Queue<Vec2> thisQ = new Queue<Vec2>();
				foreach(var ast in stepToAsteroids[item.Step].OrderBy(x => Vec2.ManhattanDistance(point, x)))
				{
					thisQ.Enqueue(ast);
				}
				order.Enqueue(thisQ);
			}

			// While we have asteroids left to break
			while(order.Count > 0)
			{
				// Get the next angle's list of asteroids
				var next = order.Dequeue();

				// Destroy any asteroids in the list
				if(next.Count > 0)
				{
					var a = next.Dequeue();
					destroyed.Add(a);
					//Console.WriteLine($"Destroyed {a}...");
				}

				// If asteroids remain, add to the back of the queue
				if (next.Count > 0)
				{
					order.Enqueue(next);
				}
			}

			// Return the 200th destroyed asteroid
			return (destroyed[199].X * 100 + destroyed[199].Y).ToString();
		}

		protected override void ReadMapCharacter(Vec2 position, char c)
		{
			if(c == '#')
				m_asteroids.Add(position);
		}
	}
}
