using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Particle
	{
		public int Id { get; set; }
		public Vec3 Pos { get; set; }
		public Vec3 Vel { get; set; }
		public Vec3 Acc { get; set; }
		public bool Alive { get; set; }

		public Particle(Particle p)
		{
			Id = p.Id;
			Pos = p.Pos;
			Vel = p.Vel;
			Acc = p.Acc;
			Alive = p.Alive;
		}
		public Particle(int id, Vec3 p, Vec3 v, Vec3 a)
		{
			Id = id;
			Pos = p;
			Vel = v;
			Acc = a;
			Alive = true;
		}

		public void Tick()
		{
			if (Alive)
			{
				Vel += Acc;
				Pos += Vel;
			}
		}

		public int Distance(Vec3 pt)
		{
			return Vec3.ManhattanDistance(pt, Pos);
		}

		public override string ToString()
		{
			return $"p=({Pos}), v=({Vel}), a=({Acc})";
		}
	}

	class Day20Strategy : DayLineLoaderBasic
	{
		List<Particle> m_particles = new List<Particle>();

		Vec3 GetVec3(Match matches, int index)
		{
			Vec3 vec = new Vec3();
			vec.X = int.Parse(matches.Groups[index].Value);
			vec.Y = int.Parse(matches.Groups[index+1].Value);
			vec.Z = int.Parse(matches.Groups[index+2].Value);
			return vec;
		}

		int m_currId = 0;
		public override void ParseInputLine(string line)
		{
			var matches = Regex.Match(line, @"p=<(-?\d+),(-?\d+),(-?\d+)>, v=<(-?\d+),(-?\d+),(-?\d+)>, a=<(-?\d+),(-?\d+),(-?\d+)>");
			if(matches.Success)
			{
				Vec3 pos = GetVec3(matches, 1);
				Vec3 vel = GetVec3(matches, 4);
				Vec3 acc = GetVec3(matches, 7);

				m_particles.Add(new Particle(m_currId, pos, vel, acc));
			}
			m_currId++;
		}

		public override string Part1()
		{
			List<Particle> curr = new List<Particle>();
			
			foreach(var p in m_particles)
			{
				curr.Add(new Particle(p));
			}

			int lastMinId = 0;
			int streak = 0;

			while(streak < 1000)
			{
				int minDist = int.MaxValue;
				int minId = 0;

				foreach (var p in curr)
				{
					p.Tick();

					int dist = Math.Abs(p.Distance(Vec3.Zero));
					if(dist < minDist)
					{
						minId = p.Id;
						minDist = dist;
					}
				}

				if (minId == lastMinId)
				{
					streak++;
				}
				else
				{
					//Console.WriteLine($"New closest particle {minId}: {curr[minId]}!");
					streak = 0;
					lastMinId = minId;
				}
			}

			return lastMinId.ToString();
		}

		public override string Part2()
		{
			List<Particle> curr = new List<Particle>();

			foreach (var p in m_particles)
			{
				curr.Add(new Particle(p));
			}

			int collisions = 0;
			int streak = 0;
			while (streak < 1000)
			{
				collisions = 0;
				Dictionary<Vec3, List<int>> positions = new Dictionary<Vec3, List<int>>();
				foreach (var p in curr)
				{
					if (p.Alive)
					{
						p.Tick();

						if (!positions.ContainsKey(p.Pos))
						{
							positions[p.Pos] = new List<int>();
						}
						positions[p.Pos].Add(p.Id);
					}
				}

				foreach (var list in positions.Values)
				{
					if(list.Count > 1)
					{
						collisions++;
						foreach(var p in list)
						{
							curr[p].Alive = false;
						}
					}
				}

				if(collisions == 0)
				{
					streak++;
				}
				else
				{
					streak = 0;
				}
			}

			return curr.Sum(x => x.Alive ? 1 : 0).ToString();
		}
	}
}
