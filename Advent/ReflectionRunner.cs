using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent
{
	class Record
	{
		public int Year => m_year;
		private int m_year;
		public int Day => m_day;
		private int m_day;
		public Type StrategyType => m_strategyType;
		private Type m_strategyType;
		string InputFileName => m_inputFileName;
		private string m_inputFileName;

		public static Record CreateFromStrategyType(Type t)
		{
			var matches = Regex.Match(t.Namespace, @"Year(\d+)");
			if (!matches.Success)
			{
				return null;
			}
			int year = int.Parse(matches.Groups[1].Value);

			matches = Regex.Match(t.Name, @"Day(\d+)Strategy");
			if (!matches.Success)
			{
				return null;
			}
			int day = int.Parse(matches.Groups[1].Value);

			string inputFilePath = null;
			string filePath = $"inputs\\{year}";
			//if (day < 10)
			//{
			//	string filename = Path.Combine(filePath, $"day0{day}.txt");
			//	if (File.Exists(filename))
			//	{
			//		inputFilePath = filename;
			//	}
			//}

			if (inputFilePath == null)
			{
				inputFilePath = Path.Combine(filePath, $"day{day}.txt");
			}

			Record r = new Record();
			r.m_year = year;
			r.m_day = day;
			r.m_strategyType = t;
			r.m_inputFileName = inputFilePath;
			return r;
		}

		public void Run()
		{
			Day d = new Day((IDayStrategy)Activator.CreateInstance(m_strategyType, m_inputFileName));
			d.Initialize();
			Program.RunAndProfile(d);
			Console.ReadKey();
		}
	}

	class ReflectionRunner
	{
		List<Record> m_records;

		public ReflectionRunner()
		{
			m_records = new List<Record>();

			var target = typeof(IDayStrategy);
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => target.IsAssignableFrom(p));

			foreach(var t in types)
			{
				Record r = Record.CreateFromStrategyType(t);
				if(r != null)
					m_records.Add(r);
			}
		}

		public void RunLatest()
		{
			var latest = m_records.OrderBy(x => x.Day).OrderBy(x => x.Year).Last();
			latest.Run();
		}
	}
}
