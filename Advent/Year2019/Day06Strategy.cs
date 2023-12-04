using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	class Day06Strategy : DayInputFileStrategy
	{
		class Body
		{
			public Body(string name)
			{
				Name = name;
			}

			public string Name { get; set; }

			public int Direct
			{
				get
				{
					return Parent == null ? 0 : 1;
				}
			}

			public int Indirect
			{
				get
				{
					if(Parent == null)
					{
						return 0;
					}
					else
					{
						return Parent.Indirect + Direct;
					}
				}
			}

			public Body Parent { get; set; }
			public List<Body> Children = new List<Body>();

			public int SantaDistance { get; set; }
		}

		Dictionary<string, Body> m_bodies = new Dictionary<string, Body>();

		public Day06Strategy(string file) : base(file)
		{

		}

		public override string Part1()
		{
			return m_bodies.Values.Select(b => b.Indirect).Sum().ToString();
		}

		public override string Part2()
		{
			// BFS backwards from Santa!

			Body start = m_bodies["SAN"].Parent;
			start.SantaDistance = 0;

			List<Body> visited = new List<Body>();
			visited.Add(start);
			visited.Add(m_bodies["SAN"]);

			List<Body> queued = new List<Body>();
			queued.AddRange(start.Children.Where(b => !visited.Contains(b)));
			queued.Add(start.Parent);

			queued.ForEach(b => b.SantaDistance = 1);

			while(queued.Count > 0)
			{
				Body curr = queued.First();
				queued.RemoveAt(0);
				visited.Add(curr);

				var children = curr.Children.Where(b => !visited.Contains(b)).ToList();
				if (children.Count > 0)
				{
					children.ForEach(b => b.SantaDistance = curr.SantaDistance + 1);
					queued.AddRange(children);
				}

				if(curr.Parent != null && !visited.Contains(curr.Parent))
				{
					curr.Parent.SantaDistance = curr.SantaDistance + 1;
					queued.Add(curr.Parent);
				}
			}

			return m_bodies["YOU"].Parent.SantaDistance.ToString();
		}

		protected override void ParseInputLine(string line, int lineNum)
		{
			string[] names = line.Split(')');

			if(!m_bodies.ContainsKey(names[0]))
			{
				m_bodies[names[0]] = new Body(names[0]);
			}

			if(!m_bodies.ContainsKey(names[1]))
			{
				m_bodies[names[1]] = new Body(names[1]);
			}

			m_bodies[names[1]].Parent = m_bodies[names[0]];
			m_bodies[names[0]].Children.Add(m_bodies[names[1]]);
		}
	}
}
