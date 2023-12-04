using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Item
	{
		public List<string> Stuff { get; set; }

		public List<string> Allergens { get; set; }

		public Item()
		{
			Stuff = new List<string>();
			Allergens = new List<string>();
		}
	}

	class Day21Strategy : DayLineLoaderBasic
	{
		List<Item> m_items = new List<Item>();

		public override void ParseInputLine(string line, int lineNum)
		{
			var match = Regex.Match(line, @"(.*) \(contains (.*)\)");

			if(match.Success)
			{
				Item item = new Item();
				var stuff = match.Groups[1].Value;
				var allergens = match.Groups[2].Value;

				var splits = stuff.Split(' ');
				foreach(var s in splits)
				{
					item.Stuff.Add(s);
				}

				splits = allergens.Split(',');
				foreach(var s in splits)
				{
					item.Allergens.Add(s.Trim());
				}
				m_items.Add(item);
			}
		}

		Dictionary<string, string> m_map = new Dictionary<string, string>();

		Dictionary<string, HashSet<string>> m_possible = new Dictionary<string, HashSet<string>>();

		public void ClearIngredient(string ingred, string allergen)
		{
			foreach(var item in m_items)
			{
				item.Stuff.RemoveAll(s => s == ingred);
				item.Allergens.RemoveAll(s => s == allergen);
			}
			m_possible.Remove(allergen);
		}

		public override string Part1()
		{
			bool first = true;

			Console.WriteLine($"Number of allergens: {m_items.SelectMany(x => x.Allergens).Distinct().Count()}");

			while (first || m_possible.Count() > 0)
			{
				Console.WriteLine($"### Currently tracking {m_possible.Count} allergens...");
				first = false;
				for (int i = 0; i < m_items.Count; i++)
				{
					var item1 = m_items[i];
					for (int j = i + 1; j < m_items.Count; j++)
					{
						var item2 = m_items[j];

						var commonStuff = item1.Stuff.Intersect(item2.Stuff);
						var commonAllergens = item1.Allergens.Intersect(item2.Allergens).ToList();

						foreach(var allergen in commonAllergens)
						{
							if (!m_possible.ContainsKey(allergen))
							{
								m_possible[allergen] = new HashSet<string>(commonStuff);
							}
							else
							{
								m_possible[allergen] = m_possible[allergen].Intersect(commonStuff).ToHashSet();
							}

							Console.WriteLine($"Allergen {allergen} now has {m_possible[allergen].Count()} possible sources.");

							// Found what this is
							if (m_possible[allergen].Count() == 1)
							{
								var ingred = m_possible[allergen].First();
								Console.WriteLine($"Found that '{ingred}' must contain {allergen}!");
								m_map[allergen] = ingred;
								ClearIngredient(ingred, allergen);
							}

						}
						//Console.WriteLine($"Item {i} and item {j} share {commonStuff.Count()} ingredients and {commonAllergens.Count()} allergens.");
					}
				}
			}

			Console.WriteLine($"Found sources for {m_map.Count()} allergens.");

			var safe = m_items.SelectMany(x => x.Stuff).Distinct();

			return m_items.Sum(x => x.Stuff.Count(y => safe.Contains(y))).ToString();
		}

		public override string Part2()
		{
			return m_map.OrderBy(x => x.Key).Select(k => k.Value).Aggregate("", (s, x) => s + x + ",");
		}
	}
}
