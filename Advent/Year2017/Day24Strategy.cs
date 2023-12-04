using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2017
{
	class Port
	{
		public int Pins { get; private set; }
		public Port Sibling { get; set; }
		public Port Connection { get; set; }
		public List<Port> Possible { get; set; }

		public Port(int pins)
		{
			Pins = pins;
			Possible = new List<Port>();
		}

		public void Pair(Port p)
		{
			Sibling = p;
			p.Sibling = this;
		}

		public void Connect(Port p)
		{
			Connection = p;
			p.Connection = this;
		}

		public void Disconnect()
		{
			if (Connection != null)
			{
				var p = Connection;
				Connection = null;
				p.Disconnect();
			}
		}

		public override string ToString()
		{
			return Pins.ToString();
		}
	}

	class Day24Strategy : DayLineLoaderBasic
	{
		List<Port> m_ports = new List<Port>();

		List<(int, int)> m_pairs = new List<(int, int)>();

		public override void ParseInputLine(string line, int lineNum)
		{
			var splits = line.Split('/');
			int aVal = int.Parse(splits[0]);
			int bVal = int.Parse(splits[1]);
			Port a = new Port(aVal);
			Port b = new Port(bVal);
			a.Pair(b);
			m_ports.Add(a);
			m_ports.Add(b);

			m_pairs.Add((aVal, bVal));
		}


		class Bridge
		{
			public Port MostRecent { get; private set; }
			public HashSet<Port> Visited { get; private set; }

			public Bridge()
			{
				Visited = new HashSet<Port>();
			}

			public void Add(Port p)
			{
				Visited.Add(p);
				Visited.Add(p.Sibling);
				MostRecent = p.Sibling;
			}

			public override string ToString()
			{
				return Visited.Aggregate("", (s, x) => s + x.Pins + " ");
			}

			public Bridge Clone()
			{
				var p = new Bridge();
				p.Visited = new HashSet<Port>(Visited);
				p.MostRecent = MostRecent;
				return p;
			}
		}

		List<Bridge> m_finished = new List<Bridge>();

		public override string Part1()
		{
			foreach(var p in m_ports)
			{
				foreach(var q in m_ports)
				{
					if(p != q && p.Pins == q.Pins)
					{
						p.Possible.Add(q);
						q.Possible.Add(p);
					}
				}
			}

			Queue<Bridge> possible = new Queue<Bridge>();
			foreach(var p in m_ports.Where(x => x.Pins == 0))
			{
				var prog = new Bridge();
				prog.Add(p);
				possible.Enqueue(prog);
			}

			int tick = 0;
			while(possible.Count > 0)
			{
				var curr = possible.Dequeue();

				var matches = m_ports.Where(x => !curr.Visited.Contains(x) && x.Pins == curr.MostRecent.Pins);

				foreach(var m in matches)
				{
					var prog = curr.Clone();
					prog.Add(m);
					possible.Enqueue(prog);
				}

				if(matches.Count() == 0)
				{
					m_finished.Add(curr);
				}

				if(tick % 100000 == 0)
				{
					Console.WriteLine($"{m_finished.Count()} finished bridges; {possible.Count()} pending ones...");
				}

				tick++;
			}

			var sums = m_finished.Select(x => x.Visited.Sum(y => y.Pins));

			return sums.Max().ToString();
		}

		public override string Part2()
		{
			var longest = m_finished.Max(x => x.Visited.Count());

			var allLongest = m_finished.Where(x => x.Visited.Count == longest);

			var strengths = allLongest.Select(x => x.Visited.Sum(y => y.Pins));

			return strengths.Max().ToString();
		}
	}
}
