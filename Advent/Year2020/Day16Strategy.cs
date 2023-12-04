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
	class Field
	{
		public string Name { get; set; }
		public HashSet<(int, int)> Ranges { get; set; }

		public Field(string name)
		{
			Name = name;
			Ranges = new HashSet<(int, int)>();
		}

		public bool IsValid(int val)
		{
			return Ranges.Any(x => val >= x.Item1 && val <= x.Item2);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	class Day16Strategy : DayLineLoaderBasic
	{

		bool m_stateYours = false;
		bool m_stateTickets = false;

		HashSet<Field> m_fields = new HashSet<Field>();

		List<int> m_yourTicket = new List<int>();
		List<List<int>> m_tickets = new List<List<int>>();

		public override void ParseInputLine(string line, int lineNum)
		{
			if (line == string.Empty)
				return;

			var match = Regex.Match(line, @"your ticket:");
			if (match.Success)
			{
				m_stateYours = true;
				return;
			}
			match = Regex.Match(line, @"nearby tickets:");
			if(match.Success)
			{
				m_stateTickets = true;
				m_stateYours = false;
				return;
			}

			if (!m_stateYours && !m_stateTickets)
			{
				match = Regex.Match(line, @"(.+): (\d+)-(\d+) or (\d+)-(\d+)");
				if (match.Success)
				{
					var field = match.Groups[1].Value;
					var a = (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
					var b = (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));

					Field f = new Field(field);
					f.Ranges.Add(a);
					f.Ranges.Add(b);
					m_fields.Add(f);
				}
			}
			else if (m_stateYours)
			{
				m_yourTicket = new List<int>(line.Split(',').Select(x => int.Parse(x)));
			}
			else if(m_stateTickets)
			{
				var ticket = new List<int>(line.Split(',').Select(x => int.Parse(x)));
				m_tickets.Add(ticket);
			}
		}

		List<List<int>> m_validTix = new List<List<int>>();

		private IEnumerable<int> InvalidFieldValues(List<int> ticket)
		{
			HashSet<int> invalid = new HashSet<int>();

			foreach(var val in ticket)
			{
				if(!m_fields.Any(x => x.IsValid(val)))
				{
					invalid.Add(val);
				}
			}

			return invalid;
		}

		private bool IsValid(List<int> ticket)
		{
			foreach (var val in ticket)
			{
				if (!m_fields.Any(x => x.IsValid(val)))
				{
					return false;
				}
			}

			return true;
		}

		public override string Part1()
		{
			int sum = 0;
			foreach(var ticket in m_tickets)
			{
				var invFields = InvalidFieldValues(ticket).Sum();
				sum += invFields;

				if(IsValid(ticket))
				{
					m_validTix.Add(ticket);
				}
			}

			return sum.ToString();
		}

		enum State
		{
			Valid,
			Invalid,
			Confirmed,
			Excluded
		}

		private char ConvertArr(State x)
		{
			switch (x)
			{
				case State.Confirmed: return 'X';
				case State.Excluded: return ' ';
				case State.Valid: return 'o';
				default: return '.';
			}
		}

		State[][] map;
		List<Field> orderedFields;
		Dictionary<Field, int> m_answers = new Dictionary<Field, int>();
		public override string Part2()
		{
			foreach (var tix in m_validTix)
			{
				foreach (var t in tix)
				{
					Console.Write($"{t}, ");
				}
				Console.WriteLine();
			}
			Console.WriteLine();

			orderedFields = new List<Field>(m_fields);

			ArrayUtil.Build2DArray(ref map, orderedFields.Count, orderedFields.Count, State.Invalid);

			for (int f = 0; f < orderedFields.Count; f++)
			{
				var field = orderedFields.ElementAt(f);
				for (int i = 0; i < orderedFields.Count; i++)
				{
					//if (map[f][i] == State.Excluded || map[f][i] == State.Confirmed)
					//	continue;

					//if (i == 11)
					//{
					//	for (int tick = 0; tick < m_validTix.Count; tick++)
					//	{
					//		var tix = m_validTix.ElementAt(tick);
					//		var result = field.IsValid(tix.ElementAt(i));
					//		Console.WriteLine($"  Field [{field}] index {i} for ticket {tick} value {tix.ElementAt(i)} is {result}");
					//	}
					//}

					if (m_validTix.All(x => field.IsValid(x.ElementAt(i))))
					{
						map[f][i] = State.Valid;
					}
					else
					{
						map[f][i] = State.Invalid;
					}
					//Console.WriteLine($"Field {field} index {i} was {map[f][i]}");
				}

			}

			while (map.Any(x => x.Any(y => y == State.Invalid)))
			{
				ArrayUtil.PrintArray(map, orderedFields.Count, orderedFields.Count, ConvertArr);
				//Console.ReadKey();
				Console.WriteLine("=======================");


				// Check for solved rows
				for (int f = 0; f < orderedFields.Count; f++)
				{
					int numValid = 0;
					int lastValid = 0;
					for (int i = 0; i < orderedFields.Count; i++)
					{
						if (map[f][i] == State.Valid)
						{
							numValid++;
							lastValid = i;
						}
					}

					if (numValid == 1)
					{
						ConfirmPoint(f, lastValid);
						m_answers[orderedFields.ElementAt(f)] = lastValid;
						//ArrayUtil.PrintArray(map, orderedFields.Count, orderedFields.Count, ConvertArr);
						//Console.ReadKey();
						//Console.WriteLine("=======================");
					}
				}

				// Check for solved columns
				for (int i = 0; i < orderedFields.Count; i++)
				{
					int numValid = 0;
					int lastValid = 0;
					for (int f = 0; f < orderedFields.Count; f++)
					{
						if (map[f][i] == State.Valid)
						{
							numValid++;
							lastValid = f;
						}
					}

					if (numValid == 1)
					{
						ConfirmPoint(lastValid, i);
						m_answers[orderedFields.ElementAt(lastValid)] = i;
						//ArrayUtil.PrintArray(map, orderedFields.Count, orderedFields.Count, ConvertArr);
						//Console.ReadKey();
						//Console.WriteLine("=======================");
					}
				}
			}

			long prod = 1;
			for (int i = 0; i < orderedFields.Count; i++)
			{
				var f = orderedFields.ElementAt(i);
				if (f.Name.StartsWith("departure"))
				{
					prod *= m_yourTicket.ElementAt(m_answers[f]);
				}
			}

			return prod.ToString(); ;
		}

		private void ConfirmPoint(int fieldIdx, int valIdx)
		{
			Console.WriteLine($"Field {orderedFields.ElementAt(fieldIdx)}({fieldIdx}) must be index {valIdx}.");
			for(int f = 0; f < orderedFields.Count; f++)
			{
				for(int i = 0; i < orderedFields.Count; i++)
				{
					if(fieldIdx == f && valIdx == i)
					{
						map[f][i] = State.Confirmed;
					}
					else if(fieldIdx == f || valIdx == i)
					{
						map[f][i] = State.Excluded;
					}
				}
			}
		}

		public string Part2Old()
		{ 
			Queue<Field> unfoundFields = new Queue<Field>(m_fields);
			Dictionary<Field, HashSet<int>> possibleFields = new Dictionary<Field, HashSet<int>>();
			Dictionary<Field, int> foundFields = new Dictionary<Field, int>();

			while(unfoundFields.Count > 0)
			{
				Field f = unfoundFields.Dequeue();

				HashSet<int> possible;
				if(possibleFields.ContainsKey(f))
				{
					possible = possibleFields[f];
				}
				else
				{
					possible = new HashSet<int>();
				}

				for(int i=0; i < m_fields.Count; i++)
				{
					if (!foundFields.Values.Contains(i))
					{
						// If every ticket says this value is valid for this field
						if (m_validTix.All(x => f.IsValid(x.ElementAt(i))))
						{
							possible.Add(i);
						}
					}
				}

				if(possible.Count == 1)
				{
					foundFields[f] = possible.First();
					Console.WriteLine($"Found that field [{f}] can only be index {foundFields[f]}!");
				}
				else
				{
					possibleFields[f] = possible;
					Console.Write($"  Field [{f}] can still be: ");
					foreach(var x in possible)
					{
						Console.Write($"{x}, ");
					}
					Console.WriteLine();
					unfoundFields.Enqueue(f);
				}
			}

			return "";
		}

	}
}
