using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2020
{
	class Passport
	{
		public string byr { get; set; }
		public string iyr { get; set; }
		public string eyr { get; set; }
		public string hgt { get; set; }
		public string hcl { get; set; }
		public string ecl { get; set; }
		public string pid { get; set; }
		public string cid { get; set; }

		public string original = "";

		public override string ToString()
		{
			return $"{byr}-{iyr}-{eyr}\t{hgt}\t{ecl}\t{hcl}\t{pid}";
		}

		public bool IsValid()
		{
			if (byr == null || iyr == null || eyr == null || hgt == null || hcl == null || ecl == null || pid == null)
			{
				//Console.WriteLine("MISSING FIELD");
				return false;
			}

			int ibyr = int.Parse(byr);
			if (ibyr < 1920 || ibyr > 2002)
			{
				//Console.WriteLine($"BAD byr: {byr}");
				return false;
			}

			int iiyr = int.Parse(iyr);
			if (iiyr < 2010 || iiyr > 2020)
			{
				//Console.WriteLine($"BAD iyr: {iyr}");
				return false;
			}

			int ieyr = int.Parse(eyr);
			if (ieyr < 2020 || ieyr > 2030)
			{
				//Console.WriteLine($"BAD eyr: {eyr}");
				return false;
			}

			string unit = hgt.Substring(hgt.Length - 2);
			string val = hgt.Substring(0, hgt.Length - 2);
			if (unit == "cm")
			{
				int ival = int.Parse(val);
				if (ival < 150 || ival > 193)
				{
					//Console.WriteLine($"BAD hgt: {hgt}");
					return false;
				}
			}
			else if (unit == "in")
			{
				int ival = int.Parse(val);
				if (ival < 59 || ival > 76)
				{
					//Console.WriteLine($"BAD hgt: {hgt}");
					return false;
				}
			}
			else
			{
				//Console.WriteLine($"BAD hgt: {hgt}");
				return false;
			}

			var match = Regex.Match(hcl, @"#[0-9a-f]{6}");
			if(!match.Success)
			{
				//Console.WriteLine($"BAD hcl: {hcl}");
				return false;
			}


			IEnumerable<string> colors = new string[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
			if (!colors.Contains(ecl))
			{
				//Console.WriteLine($"BAD ecl: {ecl}");
				return false;
			}

			match = Regex.Match(pid, @"\A[0-9]{9}\Z");
			if (!match.Success)
			{
				//Console.WriteLine($"BAD pid: {pid}");
				return false;
			}


			return true;
		}
	}

	class Day04Strategy : DayInputFileStrategy
	{
		IEnumerable<string> m_fields = new string[]
		{
			"byr",
			"iyr",
			"eyr",
			"hgt",
			"hcl",
			"ecl",
			"pid",
			"cid"
		};

		HashSet<string> m_seen = new HashSet<string>();
		Passport m_curr = new Passport();
		List<Passport> m_passports = new List<Passport>();

		List<HashSet<string>> m_data = new List<HashSet<string>>();

		public Day04Strategy(string file) : base(file)
		{

		}

		protected override void ParseInputLine(string line)
		{
			if(line == "")
			{
				// finish
				m_data.Add(m_seen);
				m_seen = new HashSet<string>();

				m_passports.Add(m_curr);
				m_curr = new Passport();

			}
			else
			{
				var matches = Regex.Matches(line, @"\b(\w\w\w)\:([\w#]+)\b");
				foreach(Match match in matches)
				{
					var key = match.Groups[1].Value;
					var value = match.Groups[2].Value;
					//Console.WriteLine($"Found {match.Value} at {match.Index}");
					m_seen.Add(key);

					PropertyInfo p = m_curr.GetType().GetProperty(key);
					p.SetValue(m_curr, value);
				}
				m_curr.original += line + "\n";
			}
		}


		public override string Part1()
		{
			m_data.ForEach(x => x.Remove("cid"));

			//m_data.ForEach(x =>
			//{
			//	Console.WriteLine(x.Aggregate("Passport: ", (val, next) => val += $"{next} "));
			//}
			//);

			int valid = m_data.Count(x => x.Count == 7);
			int invalid = m_data.Count(x => x.Count < 7);
			Console.WriteLine($"Found {valid} valid passports and {invalid} invalid ones, out of {m_data.Count}");

			return valid.ToString();
		}

		public override string Part2()
		{
			//foreach(var p in m_passports)
			//{
			//	if(p.IsValid())
			//		Console.WriteLine($"VALID: {p} ");
			//}

			int valid = m_passports.Count(x => x.IsValid());

			return valid.ToString();
		}
	}
}
