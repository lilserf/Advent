using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2020
{
	class Rule
	{
		public static bool Print = false;
		public int Id { get; set; }
		public char TerminalChar { get; set; }
		
		public List<Rule> SubRules { get; set; }

		public List<int> SubRuleIds { get; set; }

		public bool MatchAll { get; set; }
		public Rule(int id, char c)
		{
			Id = id;
			TerminalChar = c;
			SubRules = new List<Rule>();
			SubRuleIds = new List<int>();
			MatchAll = false;
		}

		public Rule(int id)
		{
			Id = id;
			TerminalChar = '\0';
			SubRules = new List<Rule>();
			SubRuleIds = new List<int>();
			MatchAll = false;
		}

		public override string ToString()
		{
			if(SubRules.Count == 0)
			{
				return $"{TerminalChar}";
			}
			else
			{
				if(MatchAll)
				{
					
					return SubRules.Aggregate("(", (sum, x) => sum += $"{x}", s => s + ")");
				}
				else
				{
					
					return SubRules.Aggregate("(", (sum, x) => sum += $"{x} or ", s => s + ")");
				}
			}
		}

		public void Resolve(Dictionary<int, Rule> rules)
		{
			foreach(var id in SubRuleIds)
			{
				SubRules.Add(rules[id]);
			}
			SubRuleIds.Clear();

			foreach(var r in SubRules)
			{
				r.Resolve(rules);
			}
		}

		public (bool, int) Matches(string s, int level = 0)
		{
			if (Print)
			{
				for (int i = 0; i < level; i++)
					Console.Write(" ");

				string list = "";

				if (SubRules.Count > 0)
				{
					list = $"{SubRules[0].Id}";
					for (int i = 1; i < SubRules.Count; i++)
					{
						list += " AND " + SubRules[i].Id;
					}
				}

				Console.WriteLine($"Does rule {Id} ({list}) match {s}?");
			}
			if(SubRules.Count == 0)
			{
				if (s.Count() > 0 && s[0] == TerminalChar)
				{
					if (Print)
					{
						for (int i = 0; i < level; i++)
							Console.Write(" ");
						Console.WriteLine($"Rule {Id} matched {TerminalChar}");
					}
					return (true,1);
				}
			}
			else
			{
				if(MatchAll)
				{
					int adv = 0;
					string temp = s;
					foreach (var rule in SubRules)
					{
						(bool match, int advance) = rule.Matches(temp, level+1);
						if (!match)
							return (false, 0);
						adv += advance;
						temp = temp.Substring(advance);
					}
					if (Print)
					{
						for (int i = 0; i < level; i++)
							Console.Write(" ");
						Console.WriteLine($"Rule {Id} matched {s.Substring(0,adv)}");
					}

					return (true, adv);
				}
				else
				{
					foreach(var rule in SubRules)
					{
						(bool match, int advance) = rule.Matches(s, level+1);
						if (match)
						{
							if (Print)
							{
								for (int i = 0; i < level; i++)
									Console.Write(" ");
								Console.WriteLine($"Rule {Id} matched {s.Substring(0,advance)}");
							}

							return (match, advance);
						}
					}
				}
			}

			return (false, 0);
		}
	}



	class Day19Strategy : DayLineLoaderBasic
	{
		List<string> m_cases = new List<string>();

		Dictionary<int, Rule> m_rules = new Dictionary<int, Rule>();

		bool _parsingRules = true;
		public override void ParseInputLine(string line)
		{
			if(string.IsNullOrEmpty(line))
			{
				_parsingRules = false;
				return;
			}

			if(_parsingRules)
			{
				int colon = line.IndexOf(':');

				int id = int.Parse(line.Substring(0, colon));
				Rule r = new Rule(id);

				string rest = line.Substring(colon + 1);

				var match = Regex.Match(rest, "\"(\\w)\"");
				if (match.Success)
				{
					r.TerminalChar = match.Groups[1].Value[0];
				}
				else
				{

					var splits = rest.Split('|');
					foreach (var piece in splits)
					{
						Rule anon = new Rule(-1);
						anon.MatchAll = true;
						var numSplits = piece.Trim().Split(' ');
						foreach (var num in numSplits)
						{
							anon.SubRuleIds.Add(int.Parse(num));
						}
						r.SubRules.Add(anon);
					}
				}

				m_rules[id] = r;
			}
			else
			{
				m_cases.Add(line);
			}
		}

		public override string Part1()
		{
			foreach(var x in m_rules.Values)
			{
				x.Resolve(m_rules);
			}

			var rule0 = m_rules[0];
			int sum = 0;
			foreach(var line in m_cases)
			{
				(bool match, int advance) = rule0.Matches(line);

				if(match && advance == line.Length)
				{
					sum++;
				}
			}

			return sum.ToString();
		}

		public override string Part2()
		{
			if (m_rules.ContainsKey(8) && m_rules.ContainsKey(11))
			{
				var rule8 = m_rules[8];
				var newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.SubRules.Add(m_rules[8]);
				rule8.SubRules.Add(newSub);

				var rule11 = m_rules[11];
				newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.SubRules.Add(m_rules[11]);
				newSub.SubRules.Add(m_rules[31]);
				rule11.SubRules.Add(newSub);

				var rule0 = m_rules[0];
				int sum = 0;
				Rule.Print = false;
				foreach (var line in m_cases)
				{
					(bool match, int advance) = rule0.Matches(line);

					if (match && advance == line.Length)
					{
						Console.WriteLine($"Rule 0 matched {line}");
						sum++;
					}
				}
				return sum.ToString();

			}
			return "";
		}
	}
}
