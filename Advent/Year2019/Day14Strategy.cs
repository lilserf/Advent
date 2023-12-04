using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2019
{
	struct Ingredient
	{
		public string Name;
		public int Quantity;

		public Ingredient(string name, int quant)
		{
			Name = name;
			Quantity = quant;
		}

		public override string ToString()
		{
			return $"{Quantity} {Name}";
		}
	}

	class Recipe
	{
		public Ingredient Output { get; set; }

		public IEnumerable<Ingredient> Inputs { get { return m_inputs; } }
		List<Ingredient> m_inputs;

		public Recipe(string output, IEnumerable<string> inputs)
		{
			Output = ParseIngredient(output);
			m_inputs = new List<Ingredient>();
			foreach(var str in inputs)
			{
				m_inputs.Add(ParseIngredient(str));
			}
		}

		static Ingredient ParseIngredient(string ing)
		{
			var split = ing.Split(' ');
			var output = new Ingredient();
			output.Name = split[1];
			output.Quantity = int.Parse(split[0]);
			return output;
		}

		public override string ToString()
		{
			string str = "";
			foreach(var r in m_inputs)
			{
				str += $"{r}, ";
			}

			str += $" => {Output}";
			return str;
		}
	}


	class Day14Strategy : DayInputFileStrategy
	{
		static bool s_debug = false;
		List<Recipe> m_recipes = new List<Recipe>();

		public Day14Strategy(string file) : base(file)
		{

		}

		public long MakeFuel(long fuelAmount = 1)
		{
			Dictionary<string, long> needed = new Dictionary<string, long>();
			Dictionary<string, long> extra = new Dictionary<string, long>();

			needed["FUEL"] = fuelAmount;

			while (needed.Any(x => x.Value > 0 && x.Key != "ORE"))
			{
				Dictionary<string, long> newNeeded = new Dictionary<string, long>();

				foreach (var ingred in needed)
				{
					if (ingred.Key == "ORE")
					{
						if (!newNeeded.ContainsKey("ORE"))
						{
							newNeeded["ORE"] = 0;
						}

						if(s_debug) Console.WriteLine($"  Carrying forward {ingred.Value} ORE...");
						newNeeded["ORE"] += ingred.Value;
						continue;
					}

					if(s_debug) Console.WriteLine($"Need to make {ingred.Value} {ingred.Key}...");

					long amount = ingred.Value;
					long available = 0;
					if (extra.TryGetValue(ingred.Key, out available))
					{
						if(s_debug) Console.WriteLine($"  Found {available} already on hand!");
						if (available >= amount)
						{
							// We have enough on hand, just record that and move on
							extra[ingred.Key] -= amount;
							continue;
						}
						else
						{
							// We didn't have enough, but take what we have
							extra[ingred.Key] = 0;
							amount -= available;
						}
					}

					var recipe = m_recipes.Where(r => r.Output.Name == ingred.Key).FirstOrDefault();
					if(s_debug) Console.WriteLine($"  Using recipe {recipe}");

					long multiple = (long)Math.Ceiling((float)amount / recipe.Output.Quantity);
					long produced = multiple * recipe.Output.Quantity;

					if(s_debug) Console.WriteLine($"  Produced {produced} {recipe.Output.Name} (ran {multiple} times)");

					if (produced > amount)
					{
						if (!extra.ContainsKey(ingred.Key))
						{
							extra[ingred.Key] = 0;
						}

						extra[ingred.Key] += (produced - amount);
					}

					foreach (var newNeed in recipe.Inputs)
					{
						if (!newNeeded.ContainsKey(newNeed.Name))
						{
							newNeeded[newNeed.Name] = 0;
						}
						newNeeded[newNeed.Name] += multiple * newNeed.Quantity;
					}
				}

				needed = newNeeded;
				if(s_debug) Console.WriteLine("### Current needs:");
				foreach (var need in needed)
				{
					if(s_debug) Console.WriteLine($"    {need.Value} {need.Key}");
				}
			}

			return needed["ORE"];
		}

		public override string Part1()
		{
			long ore = MakeFuel();

			return ore.ToString();
		}

		public override string Part2()
		{
			long oreAvail = 1000000000000L;

			// Find an upper limit on how much fuel to make
			long prevFuelAmt = 0;
			long fuelAmt = 1000;
			long ore = MakeFuel(fuelAmt);

			while(ore < oreAvail)
			{
				prevFuelAmt = fuelAmt;
				fuelAmt *= 10;
				ore = MakeFuel(fuelAmt);
			}

			// Binary search from the last unsuccesful upper limit to the upper limit
			long upper = fuelAmt;
			long lower = prevFuelAmt;
			long next = (upper + lower) / 2; 

			while(upper != lower && next != upper && next != lower)
			{
				if(s_debug) Console.WriteLine($"Checking {next} fuel...");
				ore = MakeFuel(next);

				if(ore < oreAvail)
				{
					lower = next;
					if (s_debug) Console.WriteLine($"  Too low, narrowing to {lower} - {upper}");
				}
				else if(ore > oreAvail)
				{
					upper = next;
					if (s_debug) Console.WriteLine($"  Too high, narrowing to {lower} - {upper}");
				}
				next = (upper + lower) / 2;
			}

			return $"{next} fuel will require 1 trillion ore ({ore})";
		}


		protected override void ParseInputLine(string line, int lineNum)
		{
			var halves = line.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries);

			string output = halves[1];
			var inputs = halves[0].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

			m_recipes.Add(new Recipe(output, inputs));
		}
	}
}
