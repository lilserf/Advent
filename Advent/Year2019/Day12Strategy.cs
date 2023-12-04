using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Moon : ICloneable
	{
		int m_x;
		int m_y;
		int m_z;

		int m_vx;
		int m_vy;
		int m_vz;

		public Moon(int x, int y, int z)
		{
			m_x = x;
			m_y = y;
			m_z = z;

			m_vx = 0;
			m_vy = 0;
			m_vz = 0;
		}

		public void ApplyVelocity()
		{
			m_x += m_vx;
			m_y += m_vy;
			m_z += m_vz;
		}

		private static void GravitateOnAxis(int x1, ref int vx1, int x2, ref int vx2)
		{
			if(x1 > x2)
			{
				vx2++;
				vx1--;
			}
			else if(x1 < x2)
			{
				vx1++;
				vx2--;
			}
		}

		public void Gravitate(ref Moon other)
		{
			GravitateOnAxis(m_x, ref m_vx, other.m_x, ref other.m_vx);
			GravitateOnAxis(m_y, ref m_vy, other.m_y, ref other.m_vy);
			GravitateOnAxis(m_z, ref m_vz, other.m_z, ref other.m_vz);
		}

		public void GravitateOnAxis(ref Moon other, int axis)
		{
			switch (axis)
			{
				case 0: GravitateOnAxis(m_x, ref m_vx, other.m_x, ref other.m_vx); break;
				case 1: GravitateOnAxis(m_y, ref m_vy, other.m_y, ref other.m_vy); break;
				case 2: GravitateOnAxis(m_z, ref m_vz, other.m_z, ref other.m_vz); break;
			}
		}


		public int PotentialEnergy()
		{
			return Math.Abs(m_x) + Math.Abs(m_y) + Math.Abs(m_z);
		}

		public int KineticEnergy()
		{
			return Math.Abs(m_vx) + Math.Abs(m_vy) + Math.Abs(m_vz);
		}

		public int TotalEnergy()
		{
			return PotentialEnergy() * KineticEnergy();
		}

		public override string ToString()
		{
			return $"Pos: {m_x} {m_y} {m_z} Vel: {m_vx} {m_vy} {m_vz}";
		}

		public object Clone()
		{
			var m = new Moon(m_x, m_y, m_z);
			m.m_vx = this.m_vx;
			m.m_vy = this.m_vy;
			m.m_vz = this.m_vz;
			return m;
		}
	}

	class Day12Strategy : DayInputFileStrategy
	{
		List<Moon> m_moons = new List<Moon>();

		public Day12Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			List<Moon> working = new List<Moon>();
			m_moons.ForEach(x => working.Add(x.Clone() as Moon));

			var pairs = Permuter.Pairs(working);

			for (int step = 0; step < 1000; step++)
			{

				foreach (var pair in pairs)
				{
					Moon moon1 = pair.Item1;
					Moon moon2 = pair.Item2;
					moon1.Gravitate(ref moon2);
				}

				working.ForEach(x => x.ApplyVelocity());
			}

			return working.Aggregate(0, (sum, x) => sum += x.TotalEnergy()).ToString();
		}

		public static long gcd(long a, long b)
		{
			a = Math.Abs(a);
			b = Math.Abs(b);
			while (b > 0)
			{
				long rem = a % b;
				a = b;
				b = rem;
			}
			return a;
		}

		public static long lcm(long a, long b)
		{
			return (a / gcd(a, b)) * b;
		} 

		public override string Part2()
		{
			List<long> cycles = new List<long>();

			for (int axis = 0; axis < 3; axis++)
			{
				List<Moon> working = new List<Moon>();
				m_moons.ForEach(x => working.Add(x.Clone() as Moon));

				HashSet<string> states = new HashSet<string>();

				var pairs = Permuter.Pairs(working);

				long step = 0;
				while (true)
				{
					foreach (var pair in pairs)
					{
						Moon moon1 = pair.Item1;
						Moon moon2 = pair.Item2;
						moon1.GravitateOnAxis(ref moon2, axis);
					}

					working.ForEach(x => x.ApplyVelocity());

					string state = working.Aggregate("", (s, x) => s += x.ToString());

					if (states.Contains(state))
					{
						Console.WriteLine($"Axis {axis} cycles at step {step}");
						cycles.Add(step);
						break;
					}
					else
					{
						states.Add(state);
					}

					step++;
				}
			}

			long l2 = lcm(cycles[0], cycles[1]);
			long combo = lcm(cycles[2], l2);

			return $"{cycles[0]} * {cycles[1]} * {cycles[2]} = {combo}";
		}

		static Regex s_regex = new Regex(@"<x=(-?\d*), y=(-?\d*), z=(-?\d*)>");

		protected override void ParseInputLine(string line, int lineNum)
		{
			var match = MatchLine(line, s_regex);

			m_moons.Add(new Moon(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)));
		}
	}
}
