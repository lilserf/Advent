using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Bag
	{
		public string Color { get; set; }

		public IEnumerable<string> AllowedColorValues => m_allowedColors.Keys;
		public IDictionary<string, int> AllowedColors => m_allowedColors;
		private Dictionary<string, int> m_allowedColors = new Dictionary<string, int>();

		public Bag(string color)
		{
			Color = color;
		}

		public void AddAllowed(string color, int amount)
		{
			m_allowedColors.Add(color, amount);
		}
	}

	class Day07Strategy : DayInputFileStrategy
	{
		Dictionary<string, Bag> m_bags = new Dictionary<string, Bag>();

		public Day07Strategy(string file) : base (file)
		{

		}

		protected override void ParseInputLine(string line)
		{
			var match = Regex.Match(line, @"(.+) bags contain (.+)");

			if(match.Success)
			{
				var color = match.Groups[1].Value;
				var allowedList = match.Groups[2].Value;

				Bag b = new Bag(color);

				var splits = allowedList.Split(',');
				foreach(var s in splits)
				{
					var match2 = Regex.Match(s.Trim(), @"(\d+) (.+) bags?");
					if(match2.Success)
					{
						var c = match2.Groups[2].Value;
						var x = match2.Groups[1].Value;
						b.AddAllowed(c, int.Parse(x));
					}
				}
				m_bags.Add(b.Color, b);
			}

		}


		public override string Part1()
		{
			HashSet<Bag> result = new HashSet<Bag>();

			Queue<Bag> pending = new Queue<Bag>();
			pending.Enqueue(new Bag("shiny gold"));

			while (pending.Count > 0)
			{
				var thisBag = pending.Dequeue();

				foreach (var bag in m_bags.Values)
				{
					if(bag.AllowedColorValues.Contains(thisBag.Color))
					{
						//Console.WriteLine($"{bag.Color} bags can contain {thisBag.Color} bags!");
						result.Add(bag);
						pending.Enqueue(bag);
					}
				}
			}

			return result.Count().ToString();
		}

		Dictionary<string, int> m_insideCache = new Dictionary<string, int>();

		private int CountBagsInside(Bag b, int depth = 0)
		{
			if(b.AllowedColors.Count() == 0)
			{
				return 1;
			}
			else
			{
				if(m_insideCache.ContainsKey(b.Color))
				{
					return m_insideCache[b.Color];
				}

				int sum = 0;
				foreach(var x in b.AllowedColors)
				{
					int count = x.Value;
					string name = x.Key;
					Bag newBag = m_bags[name];
					for (int i = 0; i < depth; i++)
						Console.Write("  ");
					Console.WriteLine($"Adding {count} {name} bags...");
					int inside = CountBagsInside(newBag, depth+1);
					for (int i = 0; i < depth; i++)
						Console.Write("  ");
					Console.WriteLine($"{name} bags contain {inside} bags!");
					sum += count * inside;
				}

				m_insideCache[b.Color] = sum + 1;
				return sum + 1;
			}
		}

		public override string Part2()
		{
			return (CountBagsInside(m_bags["shiny gold"]) - 1).ToString();
		}
	}
}
