using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Rule
	{
		public static bool Print = false;
		public static bool Pause = false;
		public int Id { get; set; }
		public char TerminalChar { get; set; }
		
		public List<Rule> SubRules { get; set; }

		public List<int> SubRuleIds { get; set; }

		private string m_stringRep;

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
			return m_stringRep;
		}

		public void ResolveStringRep()
		{
			m_stringRep = "";

			if (SubRuleIds.Count == 0 && SubRules.Count == 0)
			{
				m_stringRep = $"{TerminalChar}";
			}

			foreach (var r in SubRules)
			{
				if (MatchAll)
				{
					m_stringRep += r.Id + " ";
				}
				else
				{
					m_stringRep += r.m_stringRep + " | ";
				}
			}

			m_stringRep = m_stringRep.Trim();
			if (m_stringRep.EndsWith(" |"))
			{
				m_stringRep = m_stringRep.Substring(0, m_stringRep.Length - 2);
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

			ResolveStringRep();
		}

		public (bool, List<int>) Matches(string s, int level = 0)
		{
			if (Print && Id != -1)
			{
				for (int i = 0; i < level; i++)
					Console.Write(" ");

				string list = "";

				Console.WriteLine($"Does rule {Id} ({m_stringRep}) match {s}?");
				
				if(Pause) Console.ReadKey();
			}
			if(SubRules.Count == 0)
			{
				if (s.Count() > 0 && s[0] == TerminalChar)
				{
					if (Print && Id != -1)
					{
						for (int i = 0; i < level; i++)
							Console.Write(" ");
						Console.WriteLine($"Rule {Id} matched {TerminalChar}");
					}
					return (true, new List<int>(new int[] { 1 }));
				}
			}
			else
			{
				if(MatchAll)
				{
					List<string> working = new List<string>();
					working.Add(s);

					foreach (var rule in SubRules)
					{
						var tempWorking = new List<string>(working);
						working.Clear();

						foreach (var temp in tempWorking)
						{
							(bool match, List<int> advances) = rule.Matches(temp, level + 1);
							if (match)
							{
								foreach (var adv in advances)
								{
									if (Print && Id != -1)
									{
										for (int i = 0; i < level; i++)
											Console.Write(" ");
										Console.WriteLine($"Rule {Id} matched {s.Substring(0, adv)}");
									}
									working.Add(temp.Substring(adv));
								}
							}
						}
					}

					List<int> endAdvances = new List<int>();
					foreach(string t in working)
					{
						endAdvances.Add(s.Length - t.Length);
					}

					if (endAdvances.Count > 0)
						return (true, endAdvances);
					else
						return (false, new List<int>());
				}
				else
				{
					List<int> matchingAdvances = new List<int>();
					foreach (var rule in SubRules)
					{
						(bool match, List<int> advances) = rule.Matches(s, level + 1);

						if (match)
						{
							foreach (var advance in advances)
							{
								if (Print)
								{
									for (int i = 0; i < level; i++)
										Console.Write(" ");
									Console.WriteLine($"Rule {Id} matched {s.Substring(0, advance)}");
								}
								matchingAdvances.Add(advance);
							}
						}
					}
					if(matchingAdvances.Count > 0)
					{
						return (true, matchingAdvances);
					}
					else
					{
						return (false, new List<int>());
					}
					
				}
			}

			return (false, new List<int>());
		}
	}



	class Day19Strategy : DayLineLoaderBasic
	{
		List<string> m_cases = new List<string>();

		Dictionary<int, Rule> m_rules = new Dictionary<int, Rule>();

		bool _parsingRules = true;
		public override void ParseInputLine(string line, int lineNum)
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
				(bool match, List<int> advance) = rule0.Matches(line);

				if(match && advance.Max() == line.Length)
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
				Rule newSub;
				var rule8 = m_rules[8];
				rule8.SubRules.Clear();
				rule8.MatchAll = false;

				newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.SubRules.Add(m_rules[8]);
				newSub.ResolveStringRep();
				rule8.SubRules.Add(newSub);

				newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.ResolveStringRep();
				rule8.SubRules.Add(newSub);
				rule8.ResolveStringRep();

				var rule11 = m_rules[11];
				rule11.SubRules.Clear();
				rule11.MatchAll = false;

				newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.SubRules.Add(m_rules[11]);
				newSub.SubRules.Add(m_rules[31]);
				newSub.ResolveStringRep();
				rule11.SubRules.Add(newSub);

				newSub = new Rule(-1);
				newSub.MatchAll = true;
				newSub.SubRules.Add(m_rules[42]);
				newSub.SubRules.Add(m_rules[31]);
				newSub.ResolveStringRep();
				rule11.SubRules.Add(newSub);

				rule11.ResolveStringRep();


				var rule0 = m_rules[0];
				int sum = 0;

				//Rule.Print = true;
				//Rule.Pause = false;
				//Console.WriteLine(rule0.Matches("aaaaabbaabaaaaababaa"));
				//Rule.Print = false;

				foreach (var line in m_cases)
				{
					(bool match, List<int> advance) = rule0.Matches(line);

					if (match && advance.Max() == line.Length)
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
